using UnityEngine;

public class materialScript : MonoBehaviour
{
    public bool shouldMove = true;
    private GameObject player;
    private playerGunScript playerScript;
    private float gravityScale = 0.0f;
    private readonly float gravity = -9.81f;
    private Rigidbody rb;
    // Start is called before the first frame update
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

    public void ChangeGravityScale(float change)
    {
        gravityScale = change;
    }

    public float ReadGravityScale()
    {
        return gravityScale;
    }

    private void MoveByGrav()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(new Vector3(0, gravityScale * gravity * Time.deltaTime, 0), ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Interactible"))
        {
            if (playerScript.IsPulling())
            {
                playerScript.DropObject();
            }
        }
    }
}
