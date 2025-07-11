using System.Collections;
using UnityEngine;

/// <summary>
/// Simple player controller for testing purposes. Not final product
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputData inputData;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform cameraPivot;

    // Player Movement
    private CharacterController cc;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    private bool isSprinting = false;
    private bool isJumping = false;

    private float walkSpeed = 4f;
    private float sprintSpeed = 7f;
    private float jumpForce = 4f;
    private float gravity = -13f;
    private float verticalVelocity = 0f;

    // Movement Velocity
    private Vector3 currentVelocity = Vector3.zero;
    private float acceleration = 10f;
    private float deceleration = 8f;

    // Camera Movement
    private float lookSensitivity = 0.1f;
    private float verticalRotation = 0f;

    private Vector3 standingCameraPos;
    private Vector3 highCrouchCameraPos;
    private Vector3 lowCrouchCameraPos;

    // Crouch Movement
    private enum Stance
    {
        Standing,
        HighCrouch,
        LowCrouch
    }
    private Stance currentStance = Stance.Standing;
    private float crouchSpeed = 0.35f;
    private bool isCrouching = false;
    
    private float standHeight = 2f;
    private float highCrouchHeight = 0.75f;
    private float lowCrouchHeight = 0.35f;
    
    private float crouchTransitionSpeed = 5f;
    private float crouchMoveSpeed = 2f;
    private float currentSpeed;

    #region Enable/Disable

    private void OnEnable()
    {
        // Character Controller
        cc = GetComponent<CharacterController>();
        cc.height = standHeight;

        // Input Data Events
        inputData.Initialise();
        inputData.OnMoveEvent += OnInputMove;
        inputData.OnLookEvent += OnInputLook;
        inputData.OnSprintEvent += OnInputSprint;
        inputData.OnJumpEvent += OnInputJump;
        inputData.OnCrouchEvent += OnInputCrouch;

        // Mouse Lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Crouch Setup
        standingCameraPos = cameraPivot.localPosition;
        highCrouchCameraPos = new Vector3(0, highCrouchHeight - 0.2f, 0);
        lowCrouchCameraPos = new Vector3(0, lowCrouchHeight - 0.2f, 0);

        // Player Position
        PlayerHandler.OnPlayerTransform += GetPosition;
    }

    private void OnDisable()
    {
        // Unsubscribe from event
        inputData.OnMoveEvent -= OnInputMove;
        inputData.OnLookEvent -= OnInputLook;
        inputData.OnSprintEvent -= OnInputSprint;
        inputData.OnJumpEvent -= OnInputJump;
        inputData.OnCrouchEvent -= OnInputCrouch;
        inputData.Cleanup();

        // Unlock Mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Player Position
        PlayerHandler.OnPlayerTransform -= GetPosition;
    }

    private Transform GetPosition() => this.transform;

    #endregion

    #region Runtime

    private void Update()
    {
        MovePlayer();
        MoveCamera();
    }

    private void MovePlayer()
    {
        float target_speed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move_direction = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Apply acceleration and deceleration
        if (move_direction.magnitude > 0)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, move_direction * target_speed, acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        // Handle Gravity
        if (cc.isGrounded)
        {
            verticalVelocity = -4f;

            // Jumping logic
            if (isJumping)
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        currentVelocity.y = verticalVelocity;

        // Final Movement
        cc.Move(currentVelocity * Time.deltaTime);
    }

    private void MoveCamera()
    {
        // Get mouse input
        float mouse_x = lookInput.x * lookSensitivity;
        float mouse_y = lookInput.y * lookSensitivity;

        // Rotate player (Yaw)
        transform.Rotate(Vector3.up * mouse_x);

        // Rotate camera (Pitch)
        verticalRotation -= mouse_y;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private IEnumerator PlayerCrouch(Stance stance)
    {
        isCrouching = true;
        float targetHeight = standHeight;
        float targetSpeed = walkSpeed;
        Vector3 targetCameraPos = standingCameraPos;

        switch (stance)
        {
            case Stance.HighCrouch:
                targetHeight = highCrouchHeight;
                targetSpeed = crouchMoveSpeed * 1.2f;
                targetCameraPos = highCrouchCameraPos;
                break;

            case Stance.LowCrouch:
                targetHeight = lowCrouchHeight;
                targetSpeed = crouchMoveSpeed;
                targetCameraPos = lowCrouchCameraPos;
                break;
        }

        float initialHeight = cc.height;
        Vector3 initialCameraPos = cameraPivot.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < crouchSpeed)
        {
            cc.height = Mathf.Lerp(initialHeight, targetHeight, elapsedTime / crouchSpeed);
            cameraPivot.localPosition = Vector3.Lerp(initialCameraPos, targetCameraPos, elapsedTime / crouchSpeed);

            elapsedTime += Time.deltaTime * crouchTransitionSpeed;
            yield return null;
        }

        cc.height = targetHeight;
        cameraPivot.localPosition = targetCameraPos;
        currentSpeed = targetSpeed;
        isCrouching = false;
    }

    private bool CanStandUp()
    {
        float checkHeight = standHeight;
        Vector3 origin = transform.position + Vector3.up * cc.height;
        return !Physics.Raycast(origin, Vector3.up, checkHeight - cc.height);
    }


    #endregion

    #region Input Events

    private void OnInputMove(Vector2 input) => moveInput = input;
    private void OnInputLook(Vector2 input) => lookInput = input;
    private void OnInputSprint(bool input) => isSprinting = input;
    private void OnInputJump(bool input)
    {
        if (cc.isGrounded && input)
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }
    }
    private void OnInputCrouch()
    {
        if (isCrouching)
            return;

        switch (currentStance)
        {
            case Stance.Standing:
                currentStance = Stance.HighCrouch;
                break;
            case Stance.HighCrouch:
                if (CanStandUp())
                    currentStance = Stance.Standing;
                break;
        }

        StartCoroutine(PlayerCrouch(currentStance));
    }

    #endregion
}