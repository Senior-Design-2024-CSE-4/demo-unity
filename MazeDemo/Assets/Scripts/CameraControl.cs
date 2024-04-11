using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Control how fast the camera moves relative to mouse movement. Higher = faster
    public float mouseSensitivity = 300f;

    // This is defined in the Unity development interface
    public Transform playerBody;

    // Represents vertical rotation
    float xRotation = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Inverting the vertical movement of the mouse to make moving it up correlate with up
        xRotation -= mouseY;
        // Restrict the player's ability to look up and down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotates the camera vertically
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rotates the camera horizontally
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
