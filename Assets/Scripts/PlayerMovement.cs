using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMovementMode { Player, Puppet, None };
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

    public PlayerMovementMode mode;

    // Inputs
    public float x;
    public float z;
    public bool jump;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    float GetXInput()
    {
        float value = 0.0f;

        switch(mode)
        {
            case PlayerMovementMode.Player:
                value = Input.GetAxis("Horizontal");
                break;

            case PlayerMovementMode.Puppet:
                value = x;
                break;
        }

        return value;
    }

    float GetYInput()
    {
        float value = 0.0f;

        switch (mode)
        {
            case PlayerMovementMode.Player:
                value = Input.GetAxis("Vertical");
                break;

            case PlayerMovementMode.Puppet:
                value = z;
                break;
        }

        return value;
    }

    bool GetJumpInput()
    {
        bool value = false;

        switch (mode)
        {
            case PlayerMovementMode.Player:
                value = Input.GetButtonDown("Jump");
                break;

            case PlayerMovementMode.Puppet:
                value = jump;
                break;
        }

        return value;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVector = new Vector3();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        x = GetXInput();
        z = GetYInput();
        jump = GetJumpInput();

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
        if(jump && isGrounded)
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
