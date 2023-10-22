using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    private float horizontalMovement;
    private float verticalMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");
        
        transform.Translate(Vector3.right * Time.deltaTime * movementSpeed * horizontalMovement);
        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed * verticalMovement);
        
    }
}
