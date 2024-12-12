using UnityEngine;

public class GunSway : MonoBehaviour
{
    public float swayAmount = 0.05f; // The maximum amount of sway
    public float swaySpeed = 5f; // The speed of the sway effect
    private Vector3 initialPosition; // Initial position of the gun
    private Quaternion initialRotation; // Initial rotation of the gun

    void Start()
    {
        // Save the initial position and rotation of the gun
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Get mouse input for the sway effect
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate the target position based on mouse movement
        Vector3 targetPosition = initialPosition + new Vector3(-mouseX * swayAmount, -mouseY * swayAmount, 0);

        // Smoothly interpolate the gun's position to the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * swaySpeed);

        // Optionally add a rotation sway effect (uncomment if desired)
        // Quaternion targetRotation = initialRotation * Quaternion.Euler(new Vector3(-mouseY * swayAmount, mouseX * swayAmount, 0));
        // transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * swaySpeed);
    }
}
