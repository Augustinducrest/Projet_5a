using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementCamera : MonoBehaviour
{
    private Rigidbody rb;
    public Camera playerCamera; 
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public float mouseSensitivity = 3.0f;
    public float maxLookAngle = 90.0f;
    public float moveSpeed =10.0f;
    public float maxVelocityChange = 7f;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch = pitch - Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -maxLookAngle,maxLookAngle);

        transform.localEulerAngles = new Vector3(0,yaw,0);
        playerCamera.transform.localEulerAngles = new Vector3(pitch,0,0);
    }
    void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0 ,Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity)*moveSpeed;

        Vector3 velocityChange = targetVelocity -rb.velocity;
        velocityChange.x =Mathf.Clamp(velocityChange.x , -maxVelocityChange,maxVelocityChange);
        velocityChange.z =Mathf.Clamp(velocityChange.z , -maxVelocityChange,maxVelocityChange);
        velocityChange.y =0;

        rb.AddForce(velocityChange,ForceMode.VelocityChange);

        Vector3 gravity = new Vector3(0,-9.81f,0);
        float gravityMultiplier =10f;
        gravity *= gravityMultiplier;
        if(velocityChange.y < 0)
        {
            rb.AddForce(gravity , ForceMode.Force);
        }

    }
    
}
