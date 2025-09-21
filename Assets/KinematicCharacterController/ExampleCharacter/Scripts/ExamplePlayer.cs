using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        [Header("References")]
        public ExampleCharacterController Character;
        public ExampleCharacterCamera CharacterCamera;
        
        [Header("Input Settings")]
        public PlayerInput playerInput;
        public float mouseSensitivity = 1f;
        
        // Input Actions
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction crouchAction;
        private InputAction scrollAction;
        private InputAction rightClickAction;
        private InputAction leftClickAction;
        
        // Input values
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpPressed;
        private bool crouchPressed;
        private bool crouchReleased;
        private float scrollInput;

        private void Awake()
        {
            // Get PlayerInput component if not assigned
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            
            // Get input actions
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            jumpAction = playerInput.actions["Jump"];
            crouchAction = playerInput.actions["Crouch"];
            scrollAction = playerInput.actions["Scroll"];
            rightClickAction = playerInput.actions["RightClick"];
            leftClickAction = playerInput.actions["LeftClick"];
        }

        private void OnEnable()
        {
            // Subscribe to input events
            jumpAction.performed += OnJump;
            crouchAction.performed += OnCrouchStart;
            crouchAction.canceled += OnCrouchEnd;
            leftClickAction.performed += OnLeftClick;
            
            // Enable input actions
            playerInput.actions.Enable();
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            jumpAction.performed -= OnJump;
            crouchAction.performed -= OnCrouchStart;
            crouchAction.canceled -= OnCrouchEnd;
            leftClickAction.performed -= OnLeftClick;
            
            // Disable input actions
            playerInput.actions.Disable();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            ReadInputs();
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void ReadInputs()
        {
            // Read continuous inputs
            moveInput = moveAction.ReadValue<Vector2>();
            lookInput = lookAction.ReadValue<Vector2>();
            scrollInput = scrollAction.ReadValue<float>();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            Vector3 lookInputVector = new Vector3(lookInput.x, lookInput.y, 0f) * mouseSensitivity;

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float processedScrollInput = -scrollInput;
#if UNITY_WEBGL
            processedScrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, processedScrollInput, lookInputVector);

            // Handle toggling zoom level
            if (rightClickAction.WasPressedThisFrame())
            {
                CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = moveInput.y;
            characterInputs.MoveAxisRight = moveInput.x;
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.JumpDown = jumpPressed;
            characterInputs.CrouchDown = crouchPressed;
            characterInputs.CrouchUp = crouchReleased;

            // Reset one-frame inputs
            jumpPressed = false;
            crouchPressed = false;
            crouchReleased = false;

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        // Input event handlers
        private void OnJump(InputAction.CallbackContext context)
        {
            jumpPressed = true;
        }

        private void OnCrouchStart(InputAction.CallbackContext context)
        {
            crouchPressed = true;
        }

        private void OnCrouchEnd(InputAction.CallbackContext context)
        {
            crouchReleased = true;
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Optional: Method to change mouse sensitivity at runtime
        public void SetMouseSensitivity(float sensitivity)
        {
            mouseSensitivity = sensitivity;
        }

        // Optional: Method to get current input values for debugging
        public Vector2 GetMoveInput() => moveInput;
        public Vector2 GetLookInput() => lookInput;
        public bool IsJumpPressed() => jumpPressed;
        public bool IsCrouchPressed() => crouchPressed;
    }
}