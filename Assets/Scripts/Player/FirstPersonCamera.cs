using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float lookSensitivity = 2f;   // Mouse sensitivity for looking
    private float cameraPitch = 0f;      // Vertical camera rotation

    public Transform playerBody;         // Reference to the player's body for rotation

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        LookAround();
    }

    void LookAround()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotate the player horizontally (yaw)
        playerBody.Rotate(Vector3.up * mouseX);

        // Vertical rotation (pitch) for the camera
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);  // Prevent flipping over

        // Apply the vertical rotation to the camera
        transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // void UnlockCursor()
    // {
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    // }

    // void ToggleCursorLock()
    // {
    //     if (Cursor.lockState == CursorLockMode.Locked)
    //     {
    //         UnlockCursor();
    //     }
    //     else
    //     {
    //         LockCursor();
    //     }
    // }
}
