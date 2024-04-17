using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMovement : MonoBehaviour
{
    public CharacterController controller;
    public float gravity = -9.81f;

    public float speed = 12f;

    private Vector3 velocity;
    public float jumpHeight = 1f;

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.paused)
        {
            // Receive directional input from the player - these are decided by AWSD
            // A and D
            float x = Input.GetAxis("Horizontal");
            // W and S (vertical refers to forward and backwards rather than up and down)
            float z = Input.GetAxis("Vertical");

            // Generates a vector that gets directional input based on the above
            // Note: transform is a relative 
            Vector3 motionVector = transform.right * x + transform.forward * z;
            motionVector *= speed;

            // Jump

            if(controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if(Input.GetButton("Jump") && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
            else {
                Debug.Log("Not grounded");
            }
            velocity.y += gravity * Time.deltaTime;
            // Move the controller in the direction specified above (position is relative to the direction the user is facing)
            controller.Move(motionVector * Time.deltaTime);
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
