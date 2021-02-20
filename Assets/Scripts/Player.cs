using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform cam;
    private World world;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;       // Earth gravity makes things fall at a rate of 9.8 meters per second
    public float playerWidth = 0.15f;

    public bool isGrounded;
    public bool isSprinting;

    private void Start()
    {
        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void Update()
    {
        GetPlayerInput();

        // Handle rotation of the camera in the regular update loop, otherwise it slows down considerably
        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
    }

    private void FixedUpdate()
    {
        // Handle velocity calculation and movement in the fixed update loop, so that time calculations
        // don't change as framerates fluctuate
        CalculateVelocity();
        if (jumpRequest)
            Jump();

        transform.Translate(velocity, Space.World);
    }

    private void GetPlayerInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;
    }

    private void CalculateVelocity()
    {
        /*velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.deltaTime * walkSpeed;
        velocity += Vector3.up * gravity * Time.deltaTime;

        velocity.y = checkDownSpeed(velocity.y);*/
        
        // Affect vertical momentum with velocity.
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        // If the player is sprinting, use the sprint multipler; otherwise use the walk multiplier.
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        // Apply vertical momentum (falling/jumping)
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        // Check for collision along the Z axis; if there is any, halt Z-axis movement.
        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;

        // Check for collision along the X axis; if there is any, halt X-axis movement.
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        // Check for collision along the Y axis; if there is any, halt Y-axis movement.
        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);
    }

    void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    private float checkDownSpeed(float downSpeed)
    {
        // Check for collision in four effective places - the four corners of the bottom plane of the player's "hitbox".
        if(world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) ||
            world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float checkUpSpeed(float upSpeed)
    {
        // Check for collision in four effective places - the four corners of the top plane of the player's "hitbox". 2f is added to the Y
        // value to account for the player's effective height.
        if (world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth) ||
            world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth))
            return 0;
        else
            return upSpeed;
    }

    public bool front
    {
        // FRONT checks for collision at the player's front face at the current block height and one block above that.
        // Again, front is positive Z, so Z PLUS hitbox width
        get
        {
            if (world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z + playerWidth) ||
                world.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
                return true;
            else
                return false;
        }
    }

    public bool back
    {
        // BACK checks for collision at the player's back face at the current block height and one block above that.
        // Again, back is negative Z, so Z MINUS hitbox width
        get
        {
            if (world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z - playerWidth) ||
                world.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                return true;
            else
                return false;
        }
    }

    public bool right
    {
        // RIGHT checks for collision at the player's right face at the current block height and one block above that.
        // Again, right is positive X, so X PLUS hitbox width
        get
        {
            if (world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y, transform.position.z) ||
                world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                return true;
            else
                return false;
        }
    }

    public bool left
    {
        // LEFT checks for collision at the player's left face at the current block height and one block above that.
        // Again, left is negative X, so X MINUS hitbox width
        get
        {
            if (world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y, transform.position.z) ||
                world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
                return true;
            else
                return false;
        }
    }
}

