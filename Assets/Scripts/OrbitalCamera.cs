
using System;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Distance"), Tooltip("Current distance the camera is from the target")]
    public float distance = 15.0f;
    [Tooltip("Minimum distance the camera can be from the target")]
    public float minDistance = 5.0f;
    [Tooltip("Maximum distance the camera can be from the target")]
    public float maxDistance = 25.0f; 
    
    [Header("Rotation")]
    public float rotationSpeed = 2.0f;
    public float verticalSpeed = 2.0f;
    
    [Header("Vertical Limits")]
    public float minVerticalAngle = 20.0f;
    public float maxVerticalAngle = 80.0f;
    
    [Header("Zoom")]
    public float zoomSpeed = 2.0f;
    
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float currentDistance;
    private Vector3 velocity = Vector3.zero;
    
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        if (target == null)
        {
            Debug.LogError("OrbitalCamera: No target assigned.");
            enabled = false;
            return;
        }
        
        currentDistance = distance;
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0.0f);
        Vector3 direction = rotation * Vector3.forward;
        Vector3 targetPosition = target.position - direction * currentDistance;
        transform.position = targetPosition;
        transform.LookAt(target.position);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("OrbitalCamera: No target assigned.");
            enabled = false;
            return;
        }

        HandleInput();
        UpdateCameraPosition();
    }

    private void HandleInput()
    {
        // Rotation
        if (Input.GetMouseButton(1)) 
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * verticalSpeed;
            // Clamp vertical angle (clamp == limit a value to a range min and max)
            currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        }

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //Abs = absolute value
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }
    }
    
}
