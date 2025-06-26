using NUnit.Framework;
using PurrNet;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Look Settings")]
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private List<Renderer> renderers;

    private Camera playerCamera;
    private CharacterController characterController;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    public static PlayerMovement localPlayerMovement;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;

        if (!isOwner)
            return;

        localPlayerMovement = this;
        playerCamera = Camera.main;
        
        characterController = GetComponent<CharacterController>();
        SetShadowsOnly();
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = cameraOffset;
        if (playerCamera == null)
        {
            enabled = false;
            return;
        }
    }

    private void SetShadowsOnly()
    {
        foreach (var renderer in renderers)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        if (!isOwner)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    private void Update()
    {
        if (!isFullySpawned)
            return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        bool isGrounded = IsGrounded();
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);


        if (horizontal == 0 && vertical == 0)
            animator.SetInteger("State", 0);
        else if (horizontal == 0 && vertical == 1)
            animator.SetInteger("State", 1);
        else if (horizontal == 0 && vertical == -1)
            animator.SetInteger("State", 2);
        else if (horizontal == 1 && vertical == 0)
            animator.SetInteger("State", 3);
        else if (horizontal == -1 && vertical == 0)
            animator.SetInteger("State", 4);
        else if ((horizontal == 1 && vertical == 1) || (horizontal == -1 && vertical == 1))
            animator.SetInteger("State", 1);
        else if ((horizontal == 1 && vertical == -1) || (horizontal == -1 && vertical == -1))
            animator.SetInteger("State", 2);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.03f, Vector3.down * groundCheckDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(cameraOffset), radius: 0.1f);
    }
#endif
}