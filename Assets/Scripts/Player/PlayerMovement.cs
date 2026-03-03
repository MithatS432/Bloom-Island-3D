using UnityEngine;
using UnityEngine.InputSystem;

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
    private bool isMoving = false;

    private Camera mainCamera;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
        mainCamera = Camera.main;
    }

    void Update()
    {
        MovePlayer();
    }

    public void OnClick()
    {
        Vector2 screenPos;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            screenPos = Mouse.current.position.ReadValue();
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPosition = hit.point;
            isMoving = true;
            Debug.Log("Hedef: " + targetPosition);
        }
    }

    void MovePlayer()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance < 0.1f)
        {
            isMoving = false;
            animator.SetFloat("Speed", 0f);
            return;
        }

        direction.Normalize();
        transform.forward = direction;

        if (controller.isGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = direction * speed + Vector3.up * verticalVelocity;
        controller.Move(move * Time.deltaTime);

        animator.SetFloat("Speed", speed);
    }
}