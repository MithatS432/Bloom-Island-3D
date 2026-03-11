using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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
    public AudioClip tiredSound;
    public GameObject tiredEffect;
    [SerializeField] private float effectInterval = 2.5f;
    private float effectTimer = 0f;
    private bool isFatigued = false;

    private bool isWaving = false;
    private float waveTimer = 0f;
    [SerializeField] private float waveInterval = 5f;

    public TextMeshProUGUI woodCountText;
    private int woodCount = 0;
    public TextMeshProUGUI stoneCountText;
    private int stoneCount = 0;


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
        if (!isWaving)
        {
            HandleInput();

            if (canMove)
            {
                MovePlayer();
            }
        }
        if (staminaCount <= 0f && !isFatigued)
        {
            isFatigued = true;
            canMove = false;
            animator.SetFloat("Speed", 0f);
            targetPosition = transform.position;
            effectTimer = 0f;
        }

        if (isFatigued)
        {
            effectTimer -= Time.deltaTime;
            if (effectTimer <= 0f)
            {
                PlayTiredEffects();
                effectTimer = effectInterval;
            }

            RegenerateStamina();

            if (staminaCount >= maxStamina)
            {
                staminaCount = maxStamina;
                isFatigued = false;
                canMove = true;
            }
        }

        if (!isWaving && distance <= 0.1f && !IsCollecting())
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= waveInterval)
            {
                StartCoroutine(WaveAnimation());
                waveTimer = 0f;
            }
        }
        else if (distance > 0.1f || IsCollecting())
        {
            waveTimer = 0f;
        }

        if (staminaBar != null)
            staminaBar.fillAmount = staminaCount / maxStamina;

        if (distance > 0.1f) CheckWater();
    }
    IEnumerator WaveAnimation()
    {
        isWaving = true;
        animator.SetTrigger("Wave");

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

        float timer = 0f;
        while (timer < animLength)
        {
            Vector3 lookDir = mainCamera.transform.position - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isWaving = false;
    }
    private bool IsCollecting()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var hit in hits)
        {
            Collectible collectible = hit.GetComponent<Collectible>();
            if (collectible != null && collectible.isCollecting)
            {
                return true;
            }
        }
        return false;
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

        if (controller.isGrounded) verticalVelocity = 0f;
        else verticalVelocity += gravity * Time.deltaTime;

        if (distance > 0.1f)
        {
            direction.Normalize();
            transform.forward = direction;
            Vector3 move = direction * speed + Vector3.up * verticalVelocity;
            controller.Move(move * Time.deltaTime);
            animator.SetFloat("Speed", controller.velocity.magnitude);

            staminaCount -= staminaDecreaseRate * Time.deltaTime;
        }
        else
        {
            animator.SetFloat("Speed", 0f);
            RegenerateStamina();
        }

        staminaCount = Mathf.Clamp(staminaCount, 0f, maxStamina);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -xRange, xRange);
        pos.z = Mathf.Clamp(pos.z, -zRange, zRange);
        transform.position = pos;
    }

    void RegenerateStamina()
    {
        if (staminaCount < maxStamina)
        {
            staminaCount += staminaRecoverRate * Time.deltaTime;
        }
    }
    public bool HasStamina()
    {
        return staminaCount > 0f && !isFatigued;
    }
    void PlayTiredEffects()
    {
        if (audioSource != null && tiredSound != null)
        {
            audioSource.PlayOneShot(tiredSound);
        }

        if (tiredEffect != null)
        {
            GameObject effect = Instantiate(tiredEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
            effect.transform.SetParent(this.transform);
            Destroy(effect, effectInterval);
        }
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

    public void UpdateCollectibleUI(Collectible collectibleType, int amount)
    {
        if (collectibleType.type == CollectibleType.Wood)
        {
            woodCount += amount;
            if (woodCountText != null)
                woodCountText.text = woodCount.ToString();
        }
        else if (collectibleType.type == CollectibleType.Stone)
        {
            stoneCount += amount;
            if (stoneCountText != null)
                stoneCountText.text = stoneCount.ToString();
        }
    }
}