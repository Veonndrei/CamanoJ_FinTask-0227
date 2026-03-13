using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float crouchSpeed = 3f;

    [Header("Jump / Gravity")]
    [SerializeField] private float jumpPower = 7f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float groundedStickForce = 2f; // keeps you grounded on slopes

    [Header("Mouse Look")]
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;

    [Header("Crouch")]
    [SerializeField] private KeyCode crouchKey = KeyCode.R;
    [SerializeField] private float defaultHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float heightLerpSpeed = 10f;

    [Header("Power-Up Settings")]
    public float speedBoostAmount = 3f;
    public float jumpBoostAmount = 3f;

    private float originalWalkSpeed;
    private float originalRunSpeed;
    private float originalJumpPower;

    // Optional: camera crouch feel
    [SerializeField] private float defaultCameraY = 0.8f;
    [SerializeField] private float crouchCameraY = 0.4f;

    private CharacterController controller;
    private Vector3 velocity;          // we store vertical velocity here
    private float rotationX;
    private bool isCrouching;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = Camera.main;
    }

    private void Start()
    {
        if (Time.timeScale == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Initialize heights from current settings
        controller.height = defaultHeight;
        isCrouching = false;

        controller = GetComponent<CharacterController>();

        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;
        originalJumpPower = jumpPower;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleCrouch();
        HandleMovement();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // ---- Input ----
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        bool wantsRun = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = wantsRun && !isCrouching;

        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        // ---- Move direction (horizontal) ----
        Vector3 move = (transform.right * inputX + transform.forward * inputZ);
        if (move.sqrMagnitude > 1f) move.Normalize(); // avoid faster diagonal movement

        Vector3 horizontalMove = move * currentSpeed;

        // ---- Ground check / Jump / Gravity ----
        if (controller.isGrounded)
        {
            // stick to ground
            if (velocity.y < 0f) velocity.y = -groundedStickForce;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpPower;
            }
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        // ---- Apply ----
        Vector3 finalMove = horizontalMove + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        bool crouchHeld = Input.GetKey(crouchKey);
        isCrouching = crouchHeld;

        float targetHeight = isCrouching ? crouchHeight : defaultHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * heightLerpSpeed);

        // Optional camera height smooth (recommended)
        if (playerCamera != null)
        {
            Vector3 camLocal = playerCamera.transform.localPosition;
            float targetCamY = isCrouching ? crouchCameraY : defaultCameraY;
            camLocal.y = Mathf.Lerp(camLocal.y, targetCamY, Time.deltaTime * heightLerpSpeed);
            playerCamera.transform.localPosition = camLocal;
        }
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null) return;

        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        transform.Rotate(0f, Input.GetAxis("Mouse X") * lookSpeed, 0f);
    }

    public void ActivateSpeedBoost(float duration)
    {
        StopCoroutine("SpeedBoostCoroutine");
        StartCoroutine(SpeedBoostCoroutine(duration));
    }

    IEnumerator SpeedBoostCoroutine(float duration)
    {
        walkSpeed = originalWalkSpeed + speedBoostAmount;
        runSpeed = originalRunSpeed + speedBoostAmount;

        yield return new WaitForSeconds(duration);

        walkSpeed = originalWalkSpeed;
        runSpeed = originalRunSpeed;
    }

    public void ActivateJumpBoost(float duration)
    {
        StopCoroutine("JumpBoostCoroutine");
        StartCoroutine(JumpBoostCoroutine(duration));
    }

    IEnumerator JumpBoostCoroutine(float duration)
    {
        jumpPower = originalJumpPower + jumpBoostAmount;

        yield return new WaitForSeconds(duration);

        jumpPower = originalJumpPower;
    }
}