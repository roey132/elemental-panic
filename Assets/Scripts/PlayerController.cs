using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 15f;
    public float acceleration = 3f;
    public float decceleration = 3f;
    public float friction = 5f;
    public float walkPenalty = 0.1f;

    public float jumpForce = 40f;
    public float baseGravity = 20f;

    private float horizontal_value;
    private float vertical_value;
    public float currGravity = 0f;


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
        vertical_value = Input.GetAxis("Vertical");
        horizontal_value = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.Space))
        {
            currGravity = baseGravity* 0.75f;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
        if (rb.velocity.y <= 0) currGravity = baseGravity;

    }
    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(horizontal_value, 0f, vertical_value);
        Vector3 normalizedDir = dir.normalized;
        Vector3 targetSpeed = new Vector3(normalizedDir.x, 0f, normalizedDir.z) * walkPenalty;

        rb.AddForce(targetSpeed * maxSpeed);

        if (dir != Vector3.zero) walkPenalty += acceleration * Time.deltaTime;
        else walkPenalty -= decceleration * Time.deltaTime;

        walkPenalty = Mathf.Clamp(walkPenalty, 0.1f, 1f);
        Debug.Log(walkPenalty);

        rb.AddForce(Vector3.right * friction * -rb.velocity.x);
        rb.AddForce(Vector3.forward * friction * -rb.velocity.z);

        rb.AddForce(Vector3.down * currGravity);
    }

    private void movementHandler()
    {

    }
}
