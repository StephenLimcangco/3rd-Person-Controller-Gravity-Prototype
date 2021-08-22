using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    AnimationManager animationManager;
    PlayerLocomotion playerLocomotion;
    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;
    public float cameraInputX;
    public float cameraInputY;

    private bool sprintPressed;
    private bool jumpPressed;
    private bool shootPressed;
    public bool aimPressed;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    public void HandleAllInputs()
    {
        ReadInput();
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleShootInput();
        HandleAimInput();
    }

    private void ReadInput()
    {
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        cameraInput.x = Input.GetAxis("Mouse X");
        cameraInput.y = Input.GetAxis("Mouse Y");

        jumpPressed = Input.GetButton("Jump");
        sprintPressed = Input.GetButton("Fire1");
        shootPressed = Input.GetButtonDown("Fire2");
        aimPressed = Input.GetButton("Fire3");
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        #region temporary walk
        //Temp
        if (Input.GetKey(KeyCode.O))
        {
            verticalInput = 0.5f;
        }
        if (Input.GetKey(KeyCode.L))
        {
            verticalInput = -0.5f;
        }
        if (Input.GetKey(KeyCode.K))
        {
            horizontalInput = -0.5f;
        }
        if (Input.GetKey(KeyCode.Semicolon))
        {
            horizontalInput = 0.5f;
        }
        #endregion

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animationManager.UpdateAnimator(0, moveAmount);
    }

    private void HandleSprintingInput()
    {
        if (sprintPressed && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jumpPressed)
        {
            jumpPressed = false;
            playerLocomotion.HandleJump();
        }
    }

    private void HandleShootInput()
    {
        if (shootPressed)
        {
            shootPressed = false;
            playerLocomotion.HandleShoot();
        }
    }

    private void HandleAimInput()
    {
        playerLocomotion.HandleAim();

        if (Input.GetButtonUp("Fire3"))
        {
            aimPressed = false;
        }
    }
}
