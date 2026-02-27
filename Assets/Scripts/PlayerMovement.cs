using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 20f;  // slightly stronger so you stay grounded
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Check movement speeds
        float speed = isRunning ? runSpeed : walkSpeed;

        // ---- Horizontal Movement ----
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        move *= speed;

        // ---- Preserve Vertical Velocity ----
        float yVelocity = moveDirection.y;

        moveDirection = move;

        // ---- Gravity & Jump ----
        if (controller.isGrounded)
        {
            if (Input.GetButton("Jump"))
                yVelocity = jumpPower;
            else
                yVelocity = -2f;  // keeps controller grounded
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = yVelocity;

        // ---- Crouching ----
        if (Input.GetKey(KeyCode.R))
        {
            controller.height = Mathf.Lerp(controller.height, crouchHeight, Time.deltaTime * 10);
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, defaultHeight, Time.deltaTime * 10);
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        // ---- Movement ----
        controller.Move(moveDirection * Time.deltaTime);

        // ---- Mouse Look ----
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}
