using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    private Rigidbody rb;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private float horizontalMovement;
    private float verticalMovement;
    private bool onGround = true;
    // private string PLAYER_TAG = "Player";
    private string GROUND_TAG = "Ground";
    private string OBSTACLE_TAG = "Obstacle";
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {   
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(horizontalMovement * movementSpeed, rb.velocity.y, verticalMovement * movementSpeed);

        if (Input.GetButtonDown("Jump") && onGround)
        {
            PlayerJump();
        }
        
    }

    void PlayerJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        onGround = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            onGround = true;
        }

        if (collision.gameObject.CompareTag(OBSTACLE_TAG))
        {
            Debug.Log("DEATH");
        }
    }

}
