using UnityEngine;

public class materialScript : MonoBehaviour
{
    private bool shouldMove = true;
    private GameObject player;
    private playerGunScript playerScript;
    private float gravityScale = 0.0f;
    private readonly float gravity = -9.81f;
    private Rigidbody rb;

    // Variable Init
    void Start()
    {
        player = GameObject.Find("PlayerModel");
        playerScript = player.GetComponent<playerGunScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            MoveByGrav();
        }
    }

    //This is turned off when an object is being traveled to the player
    //May not be necessary if I move the movement to here.
    public void SetShouldMove(bool set)
    {
        this.shouldMove = set;
    }

    //Sets an objects gravity scale, used in physics
    public void ChangeGravityScale(float change)
    {
        this.gravityScale = change;
    }

    //Just for other functions, obviously part of the get/set functions
    //Purpose is just so I can set the gravity scale to what it was after grabbing
    public float ReadGravityScale()
    {
        return gravityScale;
    }

    //Adds a gravity force multiplied by the scale, only runs if it shouldMove
    private void MoveByGrav()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(new Vector3(0, gravityScale * gravity * Time.deltaTime, 0), ForceMode.VelocityChange);
    }

    //Drops itself if it hits a wall, this is a method to avoid clipping
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Interactible"))
        {
            if (playerScript.IsPulling())
            {
                playerScript.DropObject(-collision.relativeVelocity);
            }
        }
    }
}
