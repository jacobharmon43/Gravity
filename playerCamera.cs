using UnityEngine;

public class playerCamera : MonoBehaviour
{
    //Camera control variables
    private readonly float camRotateX = 10.0f;
    private readonly float camRotateY = 10.0f;
    private Vector3 rotation = new Vector3(0, 0, 0);
    private Camera cam;

    //Variable Init
    void Start()
    {
        cam = Camera.main;
    }

    //No physics here.
    void Update()
    {
        HandleCamera();
        RotatePlayerToCamera();
    }

    //Grabs mouse Inputs, does mathemagic to rotate the camera to the desired angles.
    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * camRotateX;
        float mouseY = Input.GetAxis("Mouse Y") * camRotateY;
        rotation.y += mouseX;
        rotation.x -= mouseY;
        rotation.x = Mathf.Clamp(rotation.x, -90, 60);
        cam.transform.eulerAngles = rotation;
    }

    //Rotates player character to cameras rotation.
    private void RotatePlayerToCamera()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, cam.transform.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
