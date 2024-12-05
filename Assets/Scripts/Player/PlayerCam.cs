using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sens = 100f;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        // Update yRotation for horizontal look
        yRotation += mouseX;
        // Update xRotation for vertical look, and clamp it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 50f);

        // Apply rotations to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
