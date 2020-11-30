using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;

    Vector3 velocity;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float jumpHeight = 3f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVector = new Vector3();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // X, Z movement
        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1)
            move = move.normalized;

        move = move * speed;

        moveVector += move * Time.deltaTime;

        // Y velocity reset
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Jump
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        moveVector += velocity * Time.deltaTime;

        // Update movement
        controller.Move(moveVector);
    }
}
