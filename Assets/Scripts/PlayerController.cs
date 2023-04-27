using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    [Header("movement variables")]
    public float maxSpeed = 60f;
    public float acceleration = 5f;
    public float decceleration = 5f;
    public float friction = 8f;
    public float walkPenalty = 0.1f;

    [Header("advanced variables")]
    public float jumpForce = 15f;
    public float baseGravity = 60f;
    public float wallJumpUpForce = 20f;
    public float wallJumpBackForce = 20f;
    [SerializeField] LayerMask groundLayer;

    [Header("indicators")]
    public bool isGrounded;
    public bool isWalking;
    public bool isJumping;
    private bool isFalling;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;

    public float horizontal_value;
    public float vertical_value;
    public float currGravity;

    [Header("dash variables")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.1f;
    public bool canDash = false;
    public bool isDashing = false;
    public float dashCooldown = 2f;

    [Header("animations")]
    Animator animator;
    public float rotationSpeed;

    private float groundDistance = 0.1f;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currGravity = baseGravity;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;

        // set up vertical and horizontal values to handle movement
        vertical_value = Input.GetAxis("Vertical");
        horizontal_value = Input.GetAxis("Horizontal");

        animationHandler();

        handleDash();

        handleJumping();

        CheckIfGrounded();

        CheckIfWalking();

        handleRotation();

        isOnWall();

        wallSlidingHandler();


    }
    private void FixedUpdate()
    {
        if (isDashing) return;
        walkHandler();
        applyGravity();
    }
    private void animationHandler()
    {
        //simple animation activation
        if (isWalking == true)
        {
            animator.SetBool("isWalking", true);
        }
        if (isWalking == false)
        {
            animator.SetBool("isWalking", false);
        }
        if (isJumping == true)
        {
            animator.SetInteger("isJumping", 1);
        }
        if (isJumping == false && isWalking == false)
        {
            animator.SetInteger("isJumping", 2);
        }
        if (isJumping == false && isWalking == true)
        {
            animator.SetInteger("isJumping", 3);
        }
    }
    private void handleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isWallSliding)
        {
            currGravity = baseGravity * 0.75f;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isJumping = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isTouchingWall && !isGrounded)
        {
            currGravity = baseGravity * 0.75f;
            Vector3 backward = transform.forward * -1f;
            backward = backward * wallJumpUpForce;
            backward.y = wallJumpUpForce;
            Input.ResetInputAxes();
            rb.velocity = backward;
            
            isJumping = true;
        }

        // change gravity in jump climax, set indicators accordingly
        if (rb.velocity.y <= 0 && isJumping)
        {
            currGravity = baseGravity;
            isJumping = false;
        }

        if (rb.velocity.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }
    private void walkHandler()
    {
        // calculate the direction vector for movement
        Vector3 dir = new Vector3(horizontal_value, 0f, vertical_value);
        Vector3 normalizedDir = dir.normalized;

        // calcute the forces applied using walkPenalty to have acceleration and decceleration
        Vector3 targetSpeed = new Vector3(normalizedDir.x, 0f, normalizedDir.z) * walkPenalty;

        // apply the calculated force
        rb.AddForce(targetSpeed * maxSpeed);

        // calculate walkPenalty
        if (dir != Vector3.zero) walkPenalty += acceleration * Time.deltaTime;
        else walkPenalty -= decceleration * Time.deltaTime;

        // make sure walk pentalty stays in its min and max values
        walkPenalty = Mathf.Clamp(walkPenalty, 0.1f, 1f);

        // apply friction
        if (!isDashing)
        {
            rb.AddForce(Vector3.right * friction * -rb.velocity.x);
            rb.AddForce(Vector3.forward * friction * -rb.velocity.z);
        }


    }
    private void applyGravity()
    {
        // apply gravity
        if (!isWallSliding && !isGrounded)
        rb.AddForce(Vector3.down * currGravity);
    }
    private void CheckIfGrounded()
    {
        // cast a box that checks if player is grounded
        Vector3 boxCenter = transform.position;
        //set the box to start from the middle of the player
        boxCenter.y = boxCenter.y + 0.5f * transform.localScale.y;

        Vector3 halfExtents = new Vector3(0.5f * transform.localScale.x, groundDistance, 0.5f * transform.localScale.z);
        isGrounded = Physics.BoxCast(boxCenter,halfExtents,Vector3.down,out RaycastHit hitInfo,Quaternion.identity,transform.localScale.y * 0.5f + groundDistance,groundLayer);

        // cast debug rays to indicate isGrounded
        Debug.DrawRay(boxCenter, Vector3.down * (transform.localScale.y + groundDistance), isGrounded ? Color.green : Color.red);
        Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.blue);
    }
    private void CheckIfWalking()
    {
        // handle isWalking indicator
        if(Mathf.Abs(horizontal_value) > 0 || Mathf.Abs(vertical_value) > 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }
    public void handleRotation()
    {
        Vector3 movementDirection = new Vector3(horizontal_value, 0, vertical_value);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);            
        }
    }
    public void isOnWall()
    {
        Vector3 playerCenter = transform.position;
        playerCenter.y = transform.position.y + transform.localScale.y * 0.5f;
        isTouchingWall = Physics.Raycast(playerCenter, transform.forward, transform.localScale.x * 0.5f + 0.1f,groundLayer);
        Debug.DrawRay(playerCenter, transform.forward, isTouchingWall ? Color.green : Color.red);
    }
    public void wallSlidingHandler()
    {
        if (isTouchingWall && !isGrounded && !isWallSliding && !isJumping)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
        if (isWallSliding && !isGrounded && !isJumping)
        {
            rb.velocity = new Vector3(0f, -5f, 0f);
        }


    }
    private void handleDash()
    {
        if (Input.GetKeyDown(KeyCode.F) && canDash) StartCoroutine(dash());

        if (isGrounded) canDash = true;
    }
    private IEnumerator dash()
    {
        float gravityHolder = currGravity;
        currGravity = 0;
        isDashing = true;
        canDash = false;
        rb.velocity = transform.forward * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        currGravity = gravityHolder;
        isDashing = false;
    }
}
