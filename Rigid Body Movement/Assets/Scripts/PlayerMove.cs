using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float playerHeight = 2f;

    [SerializeField] Transform Orientation;

    [Header("Movement")]
    public float movespeed = 6f;
    public float movementMultiplier = 8f;
    [SerializeField] float airMultiplier = 0.15f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Jumping")]
    public float jumpForce = 10f;

    [Header("Drag")]
    float groundDrag = 6f;
    float jumpDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    float groundDistance = 0.4f;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 05f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundMask);
        print(isGrounded);
        MyInput();
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        //.forward is forward or blue, .right is left and right or red
        moveDirection = Orientation.forward * verticalMovement + Orientation.right * horizontalMovement;
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else if (!isGrounded)
        {
            rb.drag = jumpDrag;
        }
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
        rb.AddForce(Physics.gravity * (rb.mass * rb.mass));
    }

    
    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            //uses moveDirection, which is vector3 * movespeed to move player in that direction
            // Normal movement is 60 because speed is 6f and multiplier is 10f
            rb.AddForce(moveDirection.normalized * movespeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * movespeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * movespeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
        
    }
}
