using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace JadePhoenix.Gameplay
{
    [RequireComponent(typeof(TopDownController))]
    public class CharacterMovement : CharacterAbility
    {
        #region PUBLIC VARIABLES

        [Tooltip("The base speed for movement.")]
        public float MovementSpeed;

        [Tooltip("Determines if movement is currently allowed.")]
        public bool MovementForbidden;

        [Header("Settings")]
        [Tooltip("If true, input from the user is recognized.")]
        public bool InputAuthorized = true;

        [Header("Speed")]
        [Tooltip("The speed at which the character walks.")]
        public float WalkSpeed = 6f;

        [Tooltip("Determines if movement properties should be set.")]
        public bool ShouldSetMovement = true;

        [Tooltip("Threshold below which speed is considered idle.")]
        public float IdleThreshold = 0.05f;

        [Header("Acceleration")]
        [Tooltip("The acceleration to apply to the current speed / 0f : no acceleration, instant full speed.")]
        public float Acceleration = 10f;

        [Tooltip("The deceleration to apply to the current speed / 0f : no deceleration, instant stop.")]
        public float Deceleration = 10f;

        [Tooltip("If true, smoothly interpolates movement speed.")]
        public bool InterpolateMovementSpeed = false;

        [Tooltip("Multiplier applied to the movement speed.")]
        public float MovementSpeedMultiplier;

        #endregion

        protected float _movementSpeed;
        protected float _horizontalMovement;
        protected float _verticalMovement;
        protected Vector3 _movementVector;
        protected Vector2 _currentInput = Vector2.zero;
        protected Vector2 _normalizedInput;
        protected Vector2 _lerpedInput = Vector2.zero;
        protected float _acceleration = 0f;
        protected Camera _mainCamera;

        protected const string _verticalAnimationParameterName = "vel_fbw";
        protected const string _horizontalAnimationParameterName = "vel_lat";
        protected int _verticalAnimationParameter;
        protected int _horizontalAnimationParameter;

        #region UNITY LIFECYCLE

        protected virtual void FixedUpdate()
        {
            HandleMovement();
        }

        #endregion

        protected override void Initialization()
        {
            base.Initialization();

            _mainCamera = Camera.main;
            MovementSpeed = WalkSpeed;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            MovementSpeedMultiplier = 1f;
            MovementForbidden = false;
        }

        protected override void HandleInput()
        {
            if (InputAuthorized)
            {
                _horizontalMovement = _horizontalInput;
                _verticalMovement = _verticalInput;
            }
            else
            {
                _horizontalMovement = 0f;
                _verticalMovement = 0f;
            }
        }

        /// <summary>
        /// On Respawn, resets the speed
        /// </summary>
        protected override void OnRespawn()
        {
            ResetSpeed();
            MovementForbidden = false;
        }

        protected virtual void HandleMovement()
        {
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) { return; }

            if (MovementForbidden)
            {
                _horizontalMovement = 0f;
                _verticalMovement = 0f;
            }

            if ((_controller.CurrentMovement.magnitude > IdleThreshold)
            && (_movement.CurrentState == CharacterStates.MovementStates.Idle))
            {
                _movement.ChangeState(CharacterStates.MovementStates.Walking);
            }

            if (_movement.CurrentState == CharacterStates.MovementStates.Walking
            && _controller.CurrentMovement.magnitude <= IdleThreshold)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }

            if (ShouldSetMovement)
            {
                SetMovement();
            }
        }

        #region PUBLIC METHODS

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            HandleMovement();
        }

        /// <summary>
        /// Resets this character's speed
        /// </summary>
        public virtual void ResetSpeed()
        {
            MovementSpeed = WalkSpeed;
        }

        /// <summary>
        /// Moves the controller
        /// </summary>
        public virtual void SetMovement()
        {
            // Reset the movement vector and current input
            _movementVector = Vector3.zero;
            _currentInput = Vector2.zero;

            // Store the horizontal and vertical input values
            _currentInput.x = _horizontalMovement;
            _currentInput.y = _verticalMovement;

            // Normalize the input vector
            _normalizedInput = _currentInput.normalized;

            // Check if acceleration or deceleration is zero
            if ((Acceleration == 0) || (Deceleration == 0))
            {
                // If either acceleration or deceleration is zero, use the current input directly
                _lerpedInput = _currentInput;
            }
            else
            {
                // If either acceleration and deceleration are non-zero
                if (_normalizedInput.magnitude == 0)
                {
                    // If the normalized input magnitude is zero, the input is not active
                    // Gradually decrease acceleration and interpolate the input vector towards zero
                    _acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
                    _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
                }
                else
                {
                    // If the normalized input magnitude is non-zero, the input is active
                    // Gradually increase acceleration and clamp the input vector based on acceleration
                    _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
                    _lerpedInput = Vector2.ClampMagnitude(_normalizedInput, _acceleration);
                }
            }

            if (_mainCamera != null && _character.PlayerID == "Player")
            {
                // Adjust the movement vector according to the camera's orientation
                Vector3 cameraForward = Vector3.Scale(_mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 cameraRight = Vector3.Scale(_mainCamera.transform.right, new Vector3(1, 0, 1)).normalized;

                _movementVector = (_lerpedInput.y * cameraForward + _lerpedInput.x * cameraRight);
            }
            else
            {
                // Assign the x and z components of the lerped input to the movement vector
                _movementVector.x = _lerpedInput.x;
                _movementVector.y = 0;
                _movementVector.z = _lerpedInput.y;
            }

            // Adjust the movement speed based on interpolation and movement speed multiplier
            if (InterpolateMovementSpeed)
            {
                _movementSpeed = Mathf.Lerp(_movementSpeed, MovementSpeed * MovementSpeedMultiplier, _acceleration * Time.deltaTime);
            }
            else
            {
                _movementSpeed = MovementSpeed * MovementSpeedMultiplier;
            }

            // Scale the movement vector by the movement speed
            _movementVector *= _movementSpeed;

            // Clamp the movement vector magnitude to the maximum movement speed
            if (_movementVector.magnitude > MovementSpeed)
            {
                _movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed);
            }

            // Check if both current input and current movement are below the idle threshold
            if ((_currentInput.magnitude <= IdleThreshold) && (_controller.CurrentMovement.magnitude < IdleThreshold))
            {
                // If so, set the movement vector to zero to indicate no movement
                //_movementVector = Vector3.zero;
            }

            // Pass the final movement vector to the controller
            _controller.SetMovement(_movementVector);
        }

        public virtual void SetMovement(Vector2 value)
        {
            _horizontalMovement = value.x;
            _verticalMovement = value.y;
        }

        /// <summary>
        /// Sets the horizontal part of the movement
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetHorizontalMovement(float value)
        {
            _horizontalMovement = value;
        }

        /// <summary>
        /// Sets the vertical part of the movement
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetVerticalMovement(float value)
        {
            _verticalMovement = value;
        }

        #endregion

        protected Vector2 CalculateMovementParameter(Vector2 input)
        {
            Vector2 movementParameter = Vector2.zero;

            // Forward-backward motion
            if (input.y > 0.5f)
            {
                movementParameter.y = 2; // Running
            }
            else if (input.y < -0.5f)
            {
                movementParameter.y = -2; // Running backwards
            }

            // Lateral motion
            if (input.x > 0.5f)
            {
                movementParameter.x = 2; // Run right
            }
            else if (input.x < -0.5f)
            {
                movementParameter.x = -2; // Run left
            }

            return movementParameter;
        }

        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_horizontalAnimationParameterName, AnimatorControllerParameterType.Float, out _horizontalAnimationParameter);
            RegisterAnimatorParameter(_verticalAnimationParameterName, AnimatorControllerParameterType.Float, out _verticalAnimationParameter);
        }

        public override void UpdateAnimator()
        {
            Vector2 movementParameter = CalculateMovementParameter(new Vector2(_horizontalInput, _verticalInput));

            AnimatorExtensions.UpdateAnimatorFloat(_animator, _horizontalAnimationParameter, movementParameter.x, _character.AnimatorParameters);
            AnimatorExtensions.UpdateAnimatorFloat(_animator, _verticalAnimationParameter, movementParameter.y, _character.AnimatorParameters);
        }
    }
}
