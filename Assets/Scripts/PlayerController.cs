using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static GameObject player;
    public float moveSpeed = 5f; // Speed of the character
    public float jumpForce = 10f; // Jump force
    public float mouseSensitivity = 2f; // Mouse sensitivity
    public float dragFactor = 0.1f; // Horizontal drag factor (adjust as needed)

    private Rigidbody rb;
    private Camera playerCamera;
    private float xRotation = 0f; // To limit vertical rotation
    private bool isGrounded;

    private Vector3 targetVelocity;

    void Start()
    {
        player = this.gameObject;
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the player object.");
        }
        if (playerCamera == null)
        {
            Debug.LogError("Camera component is missing from the player object.");
        }

        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
        Cursor.visible = false; // Hides the cursor
    }

    void Update()
    {
        // Handle mouse look
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically and clamp it to avoid flipping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Handle movement input
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right Arrow keys
        float verticalInput = Input.GetAxisRaw("Vertical"); // W/S or Up/Down Arrow keys

        // Calculate movement direction based on the camera's rotation
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // Flatten forward and right to keep the movement on the horizontal plane
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Create move direction
        Vector3 moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

        // Apply gravity and jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Set the vertical velocity to simulate a jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        // Apply acceleration for movement
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 desiredVelocity = moveDirection * moveSpeed;

        // Smoothly accelerate to the desired velocity
        targetVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, Time.deltaTime * 10f); // Adjust acceleration rate here

        // Apply the target velocity to the horizontal plane (keep vertical velocity unchanged)
        targetVelocity.y = currentVelocity.y; // Maintain vertical velocity (gravity and jump)

        rb.linearVelocity = targetVelocity;

        // Apply horizontal drag while grounded, always decreasing the velocity over time
        if (isGrounded)
        {
            // Apply drag to the horizontal velocity components (x and z)
            rb.linearVelocity = new Vector3(
                Mathf.MoveTowards(rb.linearVelocity.x, 0, dragFactor * Time.deltaTime),
                rb.linearVelocity.y, // keep vertical velocity unchanged
                Mathf.MoveTowards(rb.linearVelocity.z, 0, dragFactor * Time.deltaTime)
            );
        }
    }

    void FixedUpdate()
    {
        // Check if the character is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
