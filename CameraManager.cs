using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTransform; // the  object  the taget follows
    public Transform cameraPivot; //object the camera pivots on
    public Transform cameraTransform;  //transform of the  actual camera
    public LayerMask collisionLayers;
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 2;
    public float cameraFollowspeed = 0.2f; //follow target speed
    public float cameraLookSpeed = 2f; //up and down speed
    public float cameraPivotSpeed = 2f; //left and  right speed

    public float lookAngle; //up and down
    public float pivotAngle; //left and right

    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;

    public float shoulderMinimumPivotAngle = -35;
    public float shoulderMaximumPivotAngle = 35;

    public float topDownMinimumPivotAngle = 50;
    public float topDownMaximumPivotAngle = 75;

    public GameObject[] cameras;
    public string currentCamera;

    private void Awake()
    {
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        inputManager = FindObjectOfType<InputManager>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;

        SwitchCameras("FollowCam");
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void HandleAllCameraMovement()
    {
        if (currentCamera == "ShoulderCam")
        {
            HandleShoulderCam();
        } else if (currentCamera == "TopDownCam")
        {
            HandleTopDownCam();
        }
        else
        {
            FollowTarget();
            RotateCamera();
            HandleCameraCollisions();
        }
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowspeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffset);

        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = targetPosition - minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    private void HandleShoulderCam()
    {
        GameObject camera = GetCurrentCamera();

        lookAngle = lookAngle + inputManager.cameraInputX * cameraPivotSpeed;
        pivotAngle = pivotAngle - inputManager.cameraInputY * cameraLookSpeed;

        lookAngle = Mathf.Clamp(lookAngle, minimumPivotAngle * 2, maximumPivotAngle * 2);
        pivotAngle = Mathf.Clamp(pivotAngle, shoulderMinimumPivotAngle, shoulderMaximumPivotAngle);
        camera.transform.localRotation = Quaternion.Euler(pivotAngle, lookAngle, 0f);
    }
    private void HandleTopDownCam()
    {
        GameObject camera = GetCurrentCamera();

        lookAngle = lookAngle + inputManager.cameraInputX * cameraPivotSpeed;
        pivotAngle = pivotAngle - inputManager.cameraInputY * cameraLookSpeed;

        lookAngle = Mathf.Clamp(lookAngle, minimumPivotAngle, maximumPivotAngle);
        pivotAngle = Mathf.Clamp(pivotAngle, topDownMinimumPivotAngle, topDownMaximumPivotAngle);
        camera.transform.localRotation = Quaternion.Euler(pivotAngle, lookAngle, 0f);
    }

    public void SwitchCameras(string cameraName)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false);

            if (cameras[i].name == cameraName)
            {
                cameras[i].SetActive(true);
                currentCamera = cameras[i].name;
            }
        }
    }

    public GameObject GetCurrentCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].name == currentCamera)
            {
                return cameras[i];
            }
        }
        return null;
    }
}
