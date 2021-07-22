using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    [SerializeField] float speed = 2.5f;
    [SerializeField] float gravity = -12.0f;
    [SerializeField] float moveSmoothTime = 0.3f;
    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private bool isJumping = false;

    //Avoid Bouncing on Slopes (Aracia)
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    [SerializeField] private LayerMask slopeLayermask;

    Vector2 currentDirection = Vector2.zero;
    Vector2 currentDirectionVelocity = Vector2.zero;
    float currentSpeed;
    float velocityY = 0.0f;
    bool sprinting = false;


    //public Rigidbody rb;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //moveDirection = transform.TransformDirection(moveDirection);
        //Legacy
        //moveDirection = transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));

        //New with customized smooth options
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();

        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirectionVelocity, moveSmoothTime);

        if (controller.isGrounded)
        {
            velocityY = 0.0f;

            //Sprinting
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }
        }

        velocityY += gravity * Time.deltaTime;
        currentSpeed = sprinting ? speed * 1.5f : speed;
        Vector3 velocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x) * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        //Prevent Jittering
        if(currentDirection.magnitude != 0f && OnSlope())
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        //New jump and Jitter fix(inspired by Acacia)
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }

        IEnumerator JumpEvent()
        {
            //fix jittering
            controller.slopeLimit = 90f;
            float timeInAir = 0f;
            do
            {
                float jumpForce = jumpFallOff.Evaluate(timeInAir);
                controller.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
                timeInAir += Time.deltaTime;
                yield return null;
            } while (!controller.isGrounded && controller.collisionFlags != CollisionFlags.Above);

            controller.slopeLimit = 45f;
            isJumping = false;
        }

        bool OnSlope()
        {
            if (isJumping)
                return false;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength, slopeLayermask))
                if (hit.normal != Vector3.up)
                    return true;
            return false;
        }

        //Legacy
        //moveDirection.y = speedVertical;
        //controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        //controller.Move(moveDirection * currentSpeed * Time.deltaTime);
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
