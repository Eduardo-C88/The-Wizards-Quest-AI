using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed (editable in the Inspector)

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Ensure the Rigidbody is assigned
    }

    private void Update()
    {
        // Get input from the player (WASD or arrow keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate the direction the player will move based on input
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Apply the movement using Rigidbody's MovePosition
        // This allows smooth, physics-based movement
        if (moveDirection.magnitude > 0) // Only move if there's input
        {
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
