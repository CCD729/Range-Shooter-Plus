using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
            //public float speed = 2.5f;
    //public float jumpSpeed = 2.5f;
    //public float gravity = 10.0F;
            //private Vector3 moveDirection = Vector3.zero;
            //CharacterController controller;
    //Rigidbody rb;
    //private float speedVertical = 0.0f;
    //private int count; //(Could put score in a separate script)

    void Start()
    {
                //controller = GetComponent<CharacterController>();
        //rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // moveDirection = transform.TransformDirection(moveDirection);

        //moveDirection = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        /*if (controller.isGrounded)
        {
            speedVertical = 0;
            if (Input.GetButtonDown("Jump"))
            {
                speedVertical = jumpSpeed;
            }
        }*/
        //speedVertical -= gravity * Time.deltaTime;
        //moveDirection.y = speedVertical;
        //controller.Move(moveDirection * Time.fixedDeltaTime * speed);
        //if (Input.GetKeyDown("escape"))
        //{
        //    if (Cursor.lockState == CursorLockMode.Locked)
        //    {
        //        Cursor.lockState = CursorLockMode.None;
       //         Cursor.visible = true;
        //    }
        //    else if (Cursor.lockState == CursorLockMode.None)
        //    {
        //        Cursor.lockState = CursorLockMode.Locked;
        //        Cursor.visible = false;
        //    }
       // }

    }
    
}
