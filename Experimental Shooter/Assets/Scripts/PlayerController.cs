using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 2.5f;
    public float jumpSpeed = 2.5f;
    public float gravity = 10.0F;
    
    public CharacterController controller;
    //public Rigidbody rb;
    private float speedVertical = 0.0f;
    private int count;
    private Vector3 moveDirection = Vector3.zero;
    private bool sprinting = false;
    private float currentSpeed;

    void Start()
    {
        //controller = GetComponent<CharacterController>();
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //moveDirection = transform.TransformDirection(moveDirection);

        moveDirection = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        if (controller.isGrounded)
        {
            speedVertical = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                speedVertical = jumpSpeed;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }
        }
        speedVertical -= gravity * Time.deltaTime;
        moveDirection.y = speedVertical;
        currentSpeed = sprinting ? speed * 1.5f : speed;
        controller.Move(moveDirection * Time.deltaTime * currentSpeed);
        controller.Move(moveDirection * Time.deltaTime * currentSpeed);
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
