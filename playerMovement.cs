using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class playerMovement : MonoBehaviour
{
    //Camera
    private Camera cam;

    //Movement and jumping variables
    public float gravityScale = 1.0f;
    public float playerSpeed = 10.0f;
    public float highJump = 15.0f;
    public float airThrusters = 0.01f;

    //Private variables
    private bool isGrounded;
    private float jumpTimerCounter = 0f;
    private float jumpTimerCounterMax = 1f;
    private CapsuleCollider col;
    private readonly float gravityValue = -9.81f;
    private Rigidbody rb;

    //Variable init
    private void Start()
    {
        col = GetComponent<CapsuleCollider>();
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    //Would move physics updates to FixedUpdate(), messes with the jumping for now.
    //Seems to function the same at various framerates, so it may not be necessary
    private void Update()
    {
        HandleMovement();
        RestartSceneOnR();
    }

    //Runs raycasts every fixed timestep, likely helps performance
    private void FixedUpdate()
    {
        CheckGrounded();
    }

    //Temporary Function for testing convenience
    private void RestartSceneOnR()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    //Combines the XZ and Y movement functions
    private void HandleMovement()
    {
        HandleXZMovement();
        HandleYMovement();
    }

    //Grabs the WASD Inputs, checks if sprinting, adjusts direction to camera, moves rigidbody.
    //May swap to MovePosition over AddForce, might be cleaner. Will test whenever we try to polish the game.
    private void HandleXZMovement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float moveSpeed = playerSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed *= 2f;
        }
        move = cam.transform.TransformDirection(move);
        move = new Vector3(move.x, 0, move.z);
        rb.AddForce(move * Time.deltaTime * moveSpeed, ForceMode.VelocityChange);
    }

    //Handles the jump
        //If the player is not grounded
            //adds force of gravity.
            //If player is already jumping, allows jumpTimerCounterMax amount of time to boost in the air through the entire jump
            //If jumpTimerCounter < 0, does nothing.
            //Returns
        //If the player tries to jump, and the code gets past the !isGrounded part, allows the player to jump.
    private void HandleYMovement()
    {
        if (!isGrounded)
        {
            rb.AddForce(new Vector3(0, gravityValue * gravityScale * Time.deltaTime, 0), ForceMode.Impulse);
            if (jumpTimerCounter > 0)
            {
                if (Input.GetButton("Jump"))
                {
                    float thrustForce = Mathf.Sqrt(airThrusters * gravityScale * gravityValue * -1f);
                    rb.AddForce(new Vector3(0, thrustForce, 0), ForceMode.Force);
                    jumpTimerCounter -= Time.deltaTime;
                }
            }
            return;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpTimerCounter = jumpTimerCounterMax;
            float jumpForce = Mathf.Sqrt(highJump * gravityScale * -0.75f * gravityValue);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    //Casts a rectangular prism at the feet of the player, if any part of this is touching the ground, sets isGrounded to true.
    //Otherwise false.
    private void CheckGrounded()
    {
        isGrounded = Physics.BoxCast(col.bounds.center, new Vector3(col.radius, 0.2f, col.radius) , new Vector3(0, -1, 0), transform.rotation, col.height /2);
    }
}



