using UnityEngine;

public class playerCamera : MonoBehaviour
{
    //Camera control variables
    private readonly float camRotateX = 10.0f;
    private readonly float camRotateY = 10.0f;
    private Vector3 rotation = new Vector3(0, 0, 0);
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCamera();
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * camRotateX;
        float mouseY = Input.GetAxis("Mouse Y") * camRotateY;
        rotation.y += mouseX;
        rotation.x -= mouseY;
        rotation.x = Mathf.Clamp(rotation.x, -90, 90);
        cam.transform.eulerAngles = rotation;
    }
}
