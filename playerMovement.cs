using UnityEngine;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour
{
    //Camera
    private Camera cam;

    //Movement and jumping variables
    public float gravityScale = 1.0f;
    public float playerSpeed = 10.0f;
    public float highJump = 15.0f;
    public float airThrusters = 0.01f;
    private bool isGrounded, isJumping;
    private float jumpTimerCounter = 0f;
    private float distToGround;
    private float jumpTimerCounterMax = 1f;
    private readonly float gravityValue = -9.81f;
    private Rigidbody rb;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Renderer>().bounds.size.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private void Update()
    {
        CheckIfGrounded();
        HandleMovement();
        RotatePlayerToCamera();
        RestartSceneOnR();
    }

    private void RestartSceneOnR()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void HandleMovement()
    {
        HandleXZMovement();
        HandleYMovement();
    }

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

    private void HandleYMovement()
    {
        if (!isGrounded)
        {
            rb.AddForce(new Vector3(0, gravityValue * gravityScale * Time.deltaTime, 0), ForceMode.Impulse);
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            jumpTimerCounter = jumpTimerCounterMax; ;
            float jumpForce = Mathf.Sqrt(highJump * gravityScale * -0.75f * gravityValue);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimerCounter > 0)
            {
                float thrustForce = Mathf.Sqrt(airThrusters * gravityScale * gravityValue * -1f);
                rb.AddForce(new Vector3(0, thrustForce, 0), ForceMode.Force);
                jumpTimerCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
                jumpTimerCounter = 0;
            }
        }
        if (Input.GetButtonUp("Jump") && isJumping)
        {
            isJumping = false;
        }
    }

    private void CheckIfGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround - 0.7f);
        if (!isGrounded && !isJumping)
        {
            isJumping = true;
        }
    }

    private void RotatePlayerToCamera()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, cam.transform.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}



