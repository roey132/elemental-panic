using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    [Header("movement variables")]
    public float maxSpeed = 15f;
    public float acceleration = 3f;
    public float decceleration = 3f;
    public float friction = 5f;
    public float walkPenalty = 0.1f;

    [Header("advanced variables")]
    public float jumpForce = 40f;
    public float baseGravity = 20f;
    [SerializeField] LayerMask groundLayer;

    [Header("indicators")]
    public bool isGrounded;
    private bool isWalking;
    public bool isJumping;
    private bool isFalling;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;

    public float horizontal_value;
    public float vertical_value;
    public float currGravity;

    private float groundDistance = 0.1f;


    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currGravity = baseGravity;
    }

    // Update is called once per frame
    void Update()
    {
        // set up vertical and horizontal values to handle movement
        vertical_value = Input.GetAxis("Vertical");
        horizontal_value = Input.GetAxis("Horizontal");

        handleJumping();

        CheckIfGrounded();

        CheckIfWalking();

        handleRotation();

        isOnWall();

        wallSlidingHandler();

    }
    private void FixedUpdate()
    {

        walkHandler();
        applyGravity();
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
            backward = backward * jumpForce * 3;
            backward.y = jumpForce * 1.1f;
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
        rb.AddForce(Vector3.right * friction * -rb.velocity.x);
        rb.AddForce(Vector3.forward * friction * -rb.velocity.z);

    }
    private void applyGravity()
    {
        // apply gravity
        if (!isWallSliding)
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
        // calculate the angle to face forward
        float angle = Mathf.Atan2(horizontal_value, vertical_value) * Mathf.Rad2Deg;

        // apply the angle to the player rotation
        if (horizontal_value != 0 && vertical_value != 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
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
}
