using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    Animator animator;
    PlayerLocomotion playerLocomotion;
    ChargeManager chargeManager;

    public bool isInteracting;
    public bool isUsingRootMotion;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        animator = GetComponentInChildren<Animator>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        chargeManager = GetComponent<ChargeManager>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        chargeManager.UpdateChargeCounter(playerLocomotion.isGrounded);
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();

        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }

}
