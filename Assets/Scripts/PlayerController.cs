using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 100;
    public float maxHSpeed = 5;
    public float jumpForce = 100;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(rb.velocity.x) < maxHSpeed)
        {
            float dir = Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;
            rb.AddForce(new Vector2(dir, 0));
        }

        if(Mathf.Abs(Input.GetAxis("Horizontal")) < 0.4f)
        {
            rb.AddForce(new Vector2(-rb.velocity.x * 10, 0));
        }

        if(Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
