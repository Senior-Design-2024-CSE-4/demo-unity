using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;

    // Update is called once per frame
    void Update()
    {
        // Receive directional input from the player - these are decided by AWSD
        // A and D
        float x = Input.GetAxis("Horizontal");
        // W and S (vertical refers to forward and backwards rather than up and down)
        float z = Input.GetAxis("Vertical");

        // Generates a vector that gets directional input based on the above
        // Note: transform is a relative 
        Vector3 motionVector = transform.right * x + transform.forward * z;

        // Move the controller in the direction specified above (position is relative to the direction the user is facing)
        controller.Move(motionVector * speed * Time.deltaTime);
    }
}
