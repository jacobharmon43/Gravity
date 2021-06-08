using UnityEngine;

public class playerGunScript : MonoBehaviour
{
    /* This class handles the following
     * All player mouse inputs.
     * All shooting related raycasts
     * All object moving related to the gun 
     ***** MIGHT MOVE THE OBJECT MOVING TO THE MATERIAL SCRIPT *****
     */


    //For the UI
    public Canvas canvas;

    //For reading the cameras angle
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
    private float currentGravityScaleOfObject;

    //Timer
    [SerializeField] private ActionOnTimer timer;

    //Constants
    private readonly float minDistanceToPlayer = 2f;

    // Variable Init
    void Start()
    {
        cam = Camera.main;
    }

    //Input Updates go here
    void Update()
    {
        HandleSwap();
        HandleShoot();
    }

    //Physics updates go here
    private void FixedUpdate()
    {
        if (IsPulling())
        {
            PullToPlayer(pulledObject);
        }
    }

    //Used externally in materialScript for release.
    public void DropObject(Vector3 force)
    {
        SetPulledObjectForces(force);
    }

    //Used internally here, and externally to force the player to drop the object.
    public bool IsPulling()
    {
        return (pulledObject != null);
    }

    //Before pulling is started
        //Left click changes gravity state
        //right click starts pulling
    //After pulling is started
        //Left click fires the object forwards
        //Releasing right click drops the object where it is, and gives it a slight launch.
    private void HandleShoot()
    {
        if (!IsPulling())
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
                SetPulledObjectToRaycast();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(1))
            {
                float newSpeed = Vector3.Distance(pulledObject.transform.position, oldPos) * flingSpeed;
                SetPulledObjectForces(GetForceOfThrow(newSpeed));
            }
            else if (Input.GetMouseButtonDown(0))
            {
                
                SetPulledObjectForces(GetForceOfThrow(launchSpeed));
            }
        }
    }

    //Takes the passed in object
    //Adjusts position towards player.
    private void PullToPlayer(GameObject obj)
    {
        if(obj == null)
        {
            return;
        }
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
        timer.SetTimer(1f, () => { oldPos = SetLocation; }); // Runs the function, after the timer expires. See ActionOnTimer.cs
        objRb.MovePosition(SetLocation);
    }

    //Swaps weapon state, and canvas text. Might set this out to
    //Somehow breaking the switch will set the text to none.
    private void HandleSwap()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            gravitySwapState++;
            if (gravitySwapState >= 3)
            {
                gravitySwapState = 0;
            }
            string S = "None";
            switch (gravitySwapState)
            {
                case 0:
                    S = "Downwards";
                    break;
                case 1:
                    break;
                case 2:
                    S = "Upwards";
                    break;
                default:
                    break;
            }
            canvas.GetComponent<UI_Manager>().UpdateGravityModeText(S);
        }
    }

    //Sets the passing in objects gravity based on the current gun state
    //Somehow breaking the switch just sets gravity to none.
    private void SwapObjectGravity(Transform obj)
    {
        float change = 0f;
        switch (gravitySwapState)
        {
            case 0:
                change = 1f;
                break;
            case 1:
                break;
            case 2:
                change = -0.3f;
                break;
            default:
                break;
        }
        obj.GetComponent<materialScript>().ChangeGravityScale(change);
    }


    //Reused code compartmentalized
    private void SetPulledObjectInit()
    {
        Rigidbody tempRb = pulledObject.GetComponent<Rigidbody>();
        materialScript objScript = pulledObject.GetComponent<materialScript>();
        tempRb.velocity = Vector3.zero;
        tempRb.mass = 0;
        tempRb.angularVelocity = Vector3.zero;
        currentGravityScaleOfObject = objScript.ReadGravityScale();
        objScript.ChangeGravityScale(0);
        objScript.SetShouldMove(false);
    }

    private void SetPulledObjectForces(Vector3 flingForce)
    {
        Rigidbody tempRb = pulledObject.GetComponent<Rigidbody>();
        materialScript objScript = pulledObject.GetComponent<materialScript>();
        tempRb.mass = 100;
        objScript.ChangeGravityScale(currentGravityScaleOfObject);
        objScript.SetShouldMove(true);
        tempRb.AddForce(flingForce, ForceMode.VelocityChange);
        pulledObject = null;
    }

    private void SetPulledObjectToRaycast()
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

    private Vector3 GetForceOfThrow(float speed)
    {
        Vector3 direction = cam.transform.forward;
        Vector3 force = direction.normalized * speed * Time.deltaTime;
        return force;
    }

}
