using UnityEngine;

public class playerGunScript : MonoBehaviour
{
    //For the UI
    public Canvas canvas;
    private Camera cam;
    
    //Publicly set variables
    public float range = Mathf.Infinity;
    public float pullingSpeed = 5.0f;
    public float launchSpeed = 750.0f;
    public float flingSpeed = 750.0f;
    //Private variables
    private GameObject pulledObject;
    private Vector3 oldPos;
    private int gravitySwapState = 0;
    private float currentGravityScaleOfObject, waitTimer = 0f;
    private bool pullingStarted = false;

    //Constants
    private readonly float minDistanceToPlayer = 2f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleSwap();
        HandleShoot();
    }
    private void FixedUpdate()
    {
        if (pulledObject != null)
        {
            PullToPlayer(pulledObject);
        }
    }

    public void DropObject()
    {
        SetPulledObjectForces(Vector3.zero);
    }

    public bool IsPulling()
    {
        if(pulledObject != null)
        {
            return true;
        }
        return false;
    }


    private void HandleShoot()
    {
        if (!pullingStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, range))
                {
                    if (hit.transform.CompareTag("Interactible"))
                    {
                        SwapObjectGravity(hit.transform);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, range))
                {
                    if (hit.transform.CompareTag("Interactible"))
                    {
                        pulledObject = hit.transform.gameObject;
                        SetPulledObjectInit();
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(1))
            {
                Vector3 direction = pulledObject.transform.position - oldPos;
                Vector3 force = direction.normalized * direction.magnitude * flingSpeed * Time.deltaTime;
                SetPulledObjectForces(force);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector3 direction = cam.transform.forward;
                Vector3 force = direction.normalized * launchSpeed * Time.deltaTime;
                SetPulledObjectForces(force);
            }
        }
    }

    private void PullToPlayer(GameObject obj)
    {
        Rigidbody objRb = obj.GetComponent<Rigidbody>();
        Vector3 currentLocation = obj.transform.position;
        Vector3 targetLocation = cam.transform.position + (cam.transform.forward * minDistanceToPlayer);
        Vector3 direction = (targetLocation - currentLocation).normalized;
        float currentDistance = Vector3.Distance(currentLocation, targetLocation);
        Vector3 movement = direction * pullingSpeed * Time.deltaTime;
        Vector3 SetLocation = currentLocation + movement;
        if (currentDistance < minDistanceToPlayer)
        {
            SetLocation = targetLocation;
        }
        if (waitTimer <= 0)
        {
            oldPos = SetLocation;
            waitTimer += 0.5f;
        }
        else
        {
            waitTimer -= Time.deltaTime;
        }
        objRb.MovePosition(SetLocation);
    }

    private void HandleSwap()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            gravitySwapState++;
            if (gravitySwapState >= 3)
            {
                gravitySwapState = 0;
            }
            switch (gravitySwapState)
            {
                case 0:
                    canvas.GetComponent<UI_Manager>().UpdateGravityModeText("Downwards Gravity");
                    break;
                case 1:
                    canvas.GetComponent<UI_Manager>().UpdateGravityModeText("No Gravity");
                    break;
                case 2:
                    canvas.GetComponent<UI_Manager>().UpdateGravityModeText("Upwards Gravity");
                    break;
                default:
                    break;
            }

        }
    }

    private void SwapObjectGravity(Transform obj)
    {
        switch (gravitySwapState)
        {
            case 0:
                obj.GetComponent<materialScript>().ChangeGravityScale(1f);
                break;
            case 1:
                obj.GetComponent<materialScript>().ChangeGravityScale(0f);
                break;
            case 2:
                obj.GetComponent<materialScript>().ChangeGravityScale(-0.25f);
                break;
            default:
                break;
        }
    }

    private void SetPulledObjectInit()
    {
        pullingStarted = true;
        Rigidbody tempRb = pulledObject.GetComponent<Rigidbody>();
        materialScript objScript = pulledObject.GetComponent<materialScript>();
        tempRb.velocity = Vector3.zero;
        tempRb.mass = 0;
        tempRb.angularVelocity = Vector3.zero;
        currentGravityScaleOfObject = objScript.ReadGravityScale();
        objScript.ChangeGravityScale(0);
        objScript.shouldMove = false;
    }

    private void SetPulledObjectForces(Vector3 flingForce)
    {
        Rigidbody tempRb = pulledObject.GetComponent<Rigidbody>();
        materialScript objScript = pulledObject.GetComponent<materialScript>();
        tempRb.mass = 100;
        objScript.ChangeGravityScale(currentGravityScaleOfObject);
        objScript.shouldMove = true;
        tempRb.AddForce(flingForce, ForceMode.VelocityChange);
        pullingStarted = false;
        pulledObject = null;
    }

}
