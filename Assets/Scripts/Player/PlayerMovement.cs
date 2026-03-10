using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private CharacterController controller;
    private Animator animator;
    private float gravity = -9.81f;
    private float verticalVelocity = 0f;
    private Vector3 targetPosition;
    private Camera mainCamera;

    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip waterWalkSound;
    public GameObject waterStepEffect;
    private bool isInWater = false;
    private float distance;

    public float xRange = 49f;
    public float zRange = 49f;

    public Image staminaBar;
    private float maxStamina = 200f;
    private float staminaCount;
    [SerializeField] private float staminaDecreaseRate = 10f;
    [SerializeField] private float staminaRecoverRate = 10f;
    private bool canMove = true;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        targetPosition = transform.position;
    }

    private void Start()
    {
        staminaCount = maxStamina;
        InvokeRepeating(nameof(CheckCollectible), 0f, 0.2f);
    }

    void Update()
    {
        if (staminaCount <= 0f)
        {
            canMove = false;
            animator.SetFloat("Speed", 0f);

            staminaCount += staminaRecoverRate * Time.deltaTime;
            if (staminaCount > maxStamina)
            {
                staminaCount = maxStamina;
                canMove = true;
            }

            if (staminaBar != null)
                staminaBar.fillAmount = staminaCount / maxStamina;

            return;
        }

        HandleInput();
        MovePlayer();

        if (distance > 0.1f)
        {
            CheckWater();
        }
    }

    void HandleInput()
    {
        if (!canMove) return;

        bool inputReceived = false;
        Vector2 screenPos = Vector2.zero;

        // Mouse input
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            screenPos = Mouse.current.position.ReadValue();
            inputReceived = true;
        }
        // Touch input
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            int touchId = (int)Touchscreen.current.primaryTouch.touchId.ReadValue();

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
                return;

            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            inputReceived = true;
        }

        if (inputReceived)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point;
            }
        }
    }

    void MovePlayer()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        distance = direction.magnitude;

        if (distance > 0.1f)
        {
            direction.Normalize();
            transform.forward = direction;
        }

        if (controller.isGrounded)
            verticalVelocity = 0f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        if (distance > 0.1f)
        {
            staminaCount -= staminaDecreaseRate * Time.deltaTime;
            if (staminaCount < 0f) staminaCount = 0f;
        }
        else
        {
            staminaCount += staminaRecoverRate * Time.deltaTime;
            if (staminaCount > maxStamina) staminaCount = maxStamina;
        }

        if (distance > 0.1f && staminaCount > 0f)
        {
            Vector3 move = direction * speed + Vector3.up * verticalVelocity;
            controller.Move(move * Time.deltaTime);
            animator.SetFloat("Speed", controller.velocity.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (staminaBar != null)
            staminaBar.fillAmount = staminaCount / maxStamina;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -xRange, xRange);
        pos.z = Mathf.Clamp(pos.z, -zRange, zRange);
        transform.position = pos;
    }

    public void PlayFootstep()
    {
        if (!controller.isGrounded || !canMove) return;

        if (isInWater)
        {
            if (audioSource != null && waterWalkSound != null)
                audioSource.PlayOneShot(waterWalkSound);

            if (waterStepEffect != null)
            {
                GameObject effectInstance = Instantiate(waterStepEffect, transform.position, Quaternion.identity);
                ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
                if (ps != null)
                    ps.Play();
                if (ps != null)
                {
                    Destroy(effectInstance, ps.main.duration);
                }
                else
                {
                    Destroy(effectInstance, 2f);
                }
            }
        }
        else
        {
            if (audioSource != null && walkSound != null)
                audioSource.PlayOneShot(walkSound);
        }
    }

    void CheckWater()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            isInWater = hit.collider.CompareTag("Lake");
        }
        else
        {
            isInWater = false;
        }
    }

    private void CheckCollectible()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var hit in hits)
        {
            Collectible collectible = hit.GetComponent<Collectible>();
            if (collectible != null && !collectible.isCollecting)
            {
                collectible.StartCollect(this);
            }
        }
    }
}