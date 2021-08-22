using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimationManager animationManager;
    InputManager inputManager;
    ChargeManager chargeManager;
    public CameraManager cameraManager;

    Vector3 moveDirection;
    Transform cameraObject;
    public Rigidbody rb;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float raycastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isShooting;
    public float shootCheckDistance = 1000f;
    public GravityOrbit currentGravity;
    public Vector3 localUp;

    [Header("Movement Speeds")]
    public float walkingspeed = 5f;
    public float runningspeed = 10;
    public float sprintingSpeed = 14;
    public float rotationSpeed = 15;

    [Header("Jump Speeds")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;


    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        animationManager = GetComponentInChildren<AnimationManager>();
        chargeManager = GetComponent<ChargeManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        rb = GetComponent<Rigidbody>();

        cameraObject = Camera.main.transform;
    }


    private void Update()
    {
        Debug.Log(isShooting);
    }
    public void HandleAllMovement()
    {
        HandleGravity();

        if (!isShooting)
        {
            HandleFallingAndLanding();
        }

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (playerManager.isInteracting)
        {
            return;
        }

        if (isJumping)
        {
            return;
        }

        if (currentGravity != null && !inputManager.aimPressed) // planet movement
        {
            {
                moveDirection = new Vector3(inputManager.horizontalInput, 0, inputManager.verticalInput).normalized;
                if (moveDirection != Vector3.zero)
                {
                    RotateOnPlanet(moveDirection);
                }
                transform.Translate(moveDirection);
            }
        } else 
        {
            Transform targetObject;

            if (!inputManager.aimPressed)
            {
                targetObject = cameraObject;
            }
            else
            {
                targetObject = transform;
            }

            moveDirection = targetObject.forward * inputManager.verticalInput;
            moveDirection = moveDirection + targetObject.right * inputManager.horizontalInput;
            moveDirection.y = 0;
            moveDirection.Normalize();
            Vector3 movementVelocity = ChangeSpeed(moveDirection);
            rb.velocity = new Vector3(movementVelocity.x, rb.velocity.y, movementVelocity.z);
        }
    }

    private Vector3 ChangeSpeed(Vector3 direction)
    {
        if (isSprinting)
        {
            direction = direction * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount > 0.5f)
            {
                direction = direction * runningspeed;
            }
            else
            {
                direction = direction * walkingspeed;
            }
        }
        return direction;
    }

    private void HandleRotation()
    {
        if (playerManager.isInteracting)
        {
            return;
        }

        if (isJumping)
        {
            return;
        }

        if (currentGravity != null || inputManager.aimPressed)
        {
            return;
        }


        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y = raycastOrigin.y + raycastHeightOffset;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animationManager.PlayTargetAnimation("fall", true);
            }

            animationManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer = inAirTimer + Time.deltaTime;
            rb.AddForce(transform.forward * leapingVelocity);
            rb.AddForce(-localUp * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(raycastOrigin, 0.2f, -localUp, out hit, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animationManager.PlayTargetAnimation("land", true);
            }

            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }

    private void HandleGravity()
    {
        if (currentGravity != null)
        {
            if (!isShooting)
            {
                Vector3 gravityUp = (transform.position - currentGravity.transform.position).normalized;
                localUp = transform.up;

                Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * transform.rotation;
                transform.rotation = targetRotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

                rb.AddForce((-gravityUp * currentGravity.gravity));
            }
        }
        else
        {
            localUp = Vector3.up;
        }
    }

    public void HandleJump()
    {
        if (isGrounded)
        {
            animationManager.animator.SetBool("isJumping", true);
            animationManager.PlayTargetAnimation("jump", false);

            float jumpVelocity = Mathf.Sqrt(-4 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpVelocity;
            rb.velocity = playerVelocity;
        }
    }

    public void HandleShoot()
    {
        if (isGrounded)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, shootCheckDistance, groundLayer))
        {
            //add  min distance
            StartCoroutine(ShootTowards(hit.point));
        }
    }

    public void HandleAim()
    {
        if (inputManager.aimPressed && !isShooting)
        {
            if (cameraManager.currentCamera != "ShoulderCam")
            {
                cameraManager.pivotAngle = 0;
                cameraManager.lookAngle = 0;
                cameraManager.SwitchCameras("ShoulderCam");
            }
        }
        else
        {

            if (currentGravity != null)
            {
                if (!isShooting)
                {
                    if (cameraManager.currentCamera != "TopDownCam")
                    {
                        cameraManager.SwitchCameras("TopDownCam");
                    }
                } else if (cameraManager.currentCamera != "FollowCam")
                {
                    cameraManager.SwitchCameras("FollowCam");
                }
            }
            else
            {
                if (cameraManager.currentCamera != "FollowCam")
                {
                    cameraManager.SwitchCameras("FollowCam");
                }
            }
        }
    }

    private void RotateOnPlanet(Vector3 MoveDirection)
    {
        float rotAmount ;

         if (moveDirection.x > 0f && moveDirection.z > 0)
        {
            rotAmount = 45;
        }
        else if (moveDirection.x > 0f && moveDirection.z == 0)
        {
            rotAmount = 90;
        }
        else if (moveDirection.x > 0f && moveDirection.z < 0)
        {
            rotAmount = 135;
        }
        else if (moveDirection.x == 0f && moveDirection.z < 0)
        {
            rotAmount = 180;
        }
        else if (moveDirection.x < 0f && moveDirection.z < 0)
        {
            rotAmount = -135;

        }
        else if (moveDirection.x < 0f && moveDirection.z == 0)
        {
            rotAmount = -90;
        }
        else if (moveDirection.x < 0f && moveDirection.z > 0)
        {
            rotAmount = -45;
        } else
        {
            rotAmount = 0;
        }
        transform.GetChild(0).localRotation = Quaternion.Euler(0, rotAmount, 0);

    }

    public IEnumerator ShootTowards(Vector3 point)
    {
        transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
        animationManager.PlayTargetAnimation("shoot", true, true);
        inputManager.aimPressed = false;
        isShooting = true;
        RaycastHit hit;

        Vector3 direction = (point - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.3f);
        rb.velocity = (direction * 50);

        while (chargeManager.chargeCounter > 0)
        {
            if (isGrounded)
            {
                break;
            }

            chargeManager.chargeCounter -= chargeManager.drainSpeed * Time.deltaTime;

            if (Physics.SphereCast(transform.position, 5, transform.forward, out hit, groundLayer))
            {
                isGrounded = (hit.distance < 2) ? true : false;
            }

            yield return null;
        }

        isShooting = false;
        transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        yield return null;
    }
}
