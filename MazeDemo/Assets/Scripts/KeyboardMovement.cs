using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMovement : MonoBehaviour
{
    public CharacterController controller;

    // Jump variables

    private Vector3 verticalMovement;
    private float jumpHeight = 1f;
    private float gravity = -9.81f;

    // Movement variables

    private Vector3 horizontalMovement;
    private float speed = 5f;
    private float sprintMultiplier = 2f;

    // Update is called once per frame
    void Update()
    {
        // Only calculate movement when not paused
        if (!PauseMenu.paused)
        {
            // Receive directional input from the player
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            horizontalMovement = transform.right * x + transform.forward * z;
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Debug.Log("Sprinting");
                horizontalMovement *= sprintMultiplier;
            }

            if (controller.isGrounded)
            {

                // Apply constant downward force to stay grounded
                if (verticalMovement.y < 0)
                {
                    verticalMovement.y = -2f;
                }

                if (Input.GetButton("Jump"))
                {
                    verticalMovement.y = Mathf.Sqrt(jumpHeight * -2 * gravity); 
                }
            }
            Debug.Log(horizontalMovement);
            verticalMovement.y += gravity * Time.deltaTime;
            // Move the controller in the direction specified above (position is relative to the direction the user is facing)
            controller.Move(horizontalMovement * Time.deltaTime);
            controller.Move(verticalMovement * Time.deltaTime);
        }
    }
}
