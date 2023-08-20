using UnityEngine;

namespace JadePhoenix.Gameplay
{
    /// <summary>
    /// TopDownController is responsible for controlling the physics-based movement of a character.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(CharacterController))]
    public class TopDownController : MonoBehaviour
    {
        #region PUBLIC VARIABLES

        [ReadOnly, Tooltip("The speed at which the character is moving.")]
        public Vector3 Speed;

        [ReadOnly, Tooltip("The current velocity of the character.")]
        public Vector3 Velocity;

        [ReadOnly, Tooltip("The velocity of the character in the last frame.")]
        public Vector3 VelocityLastFrame;

        [ReadOnly, Tooltip("The current acceleration of the character.")]
        public Vector3 Acceleration;

        [ReadOnly, Tooltip("The current movement direction and magnitude.")]
        public Vector3 CurrentMovement;

        [ReadOnly, Tooltip("The current direction of the character's movement.")]
        public Vector3 CurrentDirection;

        [Tooltip("The friction affecting the character's movement.")]
        public float Friction;

        [ReadOnly, Tooltip("Additional force applied to the character.")]
        public Vector3 AddedForce;

        [Tooltip("If true, the character can move freely; otherwise, movement is restricted.")]
        public bool FreeMovement = true;

        [Header("Layer Masks")]
        [Tooltip("Layer mask to define what layers are considered as obstacles.")]
        public LayerMask ObstaclesLayerMask;

        public enum UpdateModes { Update, FixedUpdate }

        [Header("Settings")]
        public UpdateModes UpdateMode = UpdateModes.FixedUpdate;

        public Rigidbody Rigidbody { get { return _rigidBody; } }
        public Collider Collider { get { return _collider; } }

        #endregion

        protected Vector3 _positionLastFrame;
        protected Vector3 _impact;
        protected Rigidbody _rigidBody;
        protected Collider _collider;
        protected CharacterController _characterController;

        // char movement
        protected CollisionFlags _collisionFlags;
        protected Vector3 _hitPoint = Vector3.zero;
        protected Vector3 _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

        // velocity
        protected Vector3 _newVelocity;
        protected Vector3 _lastHorizontalVelocity;
        protected Vector3 _newHorizontalVelocity;
        protected Vector3 _motion;
        protected Vector3 _idealVelocity;
        protected Vector3 _horizontalVelocityDelta;
        protected float _stickyOffset;

        // move position
        protected RaycastHit _movePositionHit;
        protected Vector3 _capsulePoint1;
        protected Vector3 _capsulePoint2;
        protected Vector3 _movePositionDirection;
        protected float _movePositionDistance;

        #region UNITY LIFECYCLE

        /// <summary>
        /// Initialization code for Awake.
        /// </summary>
        protected virtual void Awake()
        {
            CurrentDirection = transform.forward;

            Initialization();
        }

        /// <summary>
        /// Updates the character's velocity and acceleration.
        /// </summary>
        protected virtual void Update()
        {
            DetermineDirection();
            if (UpdateMode == UpdateModes.Update)
            {
                ProcessUpdate();
            }
        }

        /// <summary>
        /// Computes the character's speed.
        /// </summary>
        protected virtual void LateUpdate()
        {
            ComputeSpeed();
            VelocityLastFrame = _rigidBody.velocity;
        }

        /// <summary>
        /// Handles the physics-based movement of the character.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            //CheckGround();
            ApplyImpact();
            if (UpdateMode == UpdateModes.FixedUpdate)
            {
                ProcessUpdate();
            }
        }

        /// <summary>
        /// Draws Ground check gizmos
        /// </summary>
        //private void OnDrawGizmos()
        //{
        //    Vector3 rayOrigin = transform.position + GroundCheckStartOffset;
        //    Vector3 rayOriginLeft = rayOrigin - new Vector3(GroundCheckWidth / 2, 0);
        //    Vector3 rayOriginRight = rayOrigin + new Vector3(GroundCheckWidth / 2, 0);
        //    Vector3 rayEndLeft = rayOriginLeft + GroundCheckDirection * GroundCheckLength;
        //    Vector3 rayEndRight = rayOriginRight + GroundCheckDirection * GroundCheckLength;

        //    Gizmos.color = _isGrounded ? Color.green : Color.red;
        //    Gizmos.DrawLine(rayOriginLeft, rayEndLeft);
        //    Gizmos.DrawLine(rayOriginRight, rayEndRight);
        //}

        /// <summary>
        /// Resets all values for the character's movement.
        /// </summary>
        public virtual void Reset()
        {
            _impact = Vector3.zero;
            Speed = Vector3.zero;
            Velocity = Vector3.zero;
            VelocityLastFrame = Vector3.zero;
            Acceleration = Vector3.zero;
            CurrentMovement = Vector3.zero;
            AddedForce = Vector3.zero;
        }

        #endregion

        /// <summary>
        /// Initialization of variables and components.
        /// </summary>
        protected virtual void Initialization()
        {
            _characterController = GetComponent<CharacterController>();
            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        protected virtual void ProcessUpdate()
        {
            if (transform == null) return;
            if (!FreeMovement) return;

            _newVelocity = Velocity;
            _positionLastFrame = transform.position;

            AddInput();
            ComputeVelocityDelta();
            HandleMovement();
            ComputeNewVelocity();
        }

        protected virtual void AddInput()
        {
            Debug.Log($"{this.GetType()}.AddInput: Started.", gameObject);

            _idealVelocity = CurrentMovement;

            Debug.Log($"{this.GetType()}.AddInput: _idealVelocity set to {_idealVelocity}.", gameObject);

            _newVelocity = _idealVelocity;
            _newVelocity.y = Mathf.Min(_newVelocity.y, 0);

            Debug.Log($"{this.GetType()}.AddInput: _newVelocity set to {_newVelocity}.", gameObject);
        }

        /// <summary>
        /// Computes the motion vector to apply to the character controller 
        /// </summary>
        protected virtual void ComputeVelocityDelta()
        {
            _motion = _newVelocity * Time.deltaTime;
            _horizontalVelocityDelta.x = _motion.x;
            _horizontalVelocityDelta.y = 0f;
            _horizontalVelocityDelta.z = _motion.z;
            _stickyOffset = Mathf.Max(_characterController.stepOffset, _horizontalVelocityDelta.magnitude);
        }

        protected virtual void HandleMovement()
        {
            _collisionFlags = _characterController.Move(_motion); // controller move

            _lastHitPoint = _hitPoint;
        }

        /// <summary>
        /// Determines the new Velocity value based on our position and our position last frame
        /// </summary>
        protected virtual void ComputeNewVelocity()
        {
            _lastHorizontalVelocity.x = _newVelocity.x;
            _lastHorizontalVelocity.y = 0;
            _lastHorizontalVelocity.z = _newVelocity.z;

            Velocity = (transform.position - _positionLastFrame) / Time.deltaTime;

            _newHorizontalVelocity.x = Velocity.x;
            _newHorizontalVelocity.y = 0;
            _newHorizontalVelocity.z = Velocity.z;

            if (_lastHorizontalVelocity == Vector3.zero)
            {
                Velocity.x = 0f;
                Velocity.z = 0f;
            }
            else
            {
                float newVelocity = Vector3.Dot(_newHorizontalVelocity, _lastHorizontalVelocity) / _lastHorizontalVelocity.sqrMagnitude;
                Velocity = _lastHorizontalVelocity * Mathf.Clamp01(newVelocity) + Velocity.y * Vector3.up;
            }
            if (Velocity.y < _newVelocity.y - 0.001)
            {
                if (Velocity.y < 0)
                {
                    Velocity.y = _newVelocity.y;
                }
            }
        }

        /// <summary>
        /// Computes the direction of the controller based on the CurrentMovement variable.
        /// </summary>
        protected virtual void DetermineDirection()
        {
            if (CurrentMovement != Vector3.zero)
            {
                CurrentDirection = CurrentMovement.normalized;
            }
        }

        /// <summary>
        /// Computes the character's speed.
        /// </summary>
		protected virtual void ComputeSpeed()
        {
            Speed = (this.transform.position - _positionLastFrame) / Time.deltaTime;
            // we round the speed to 2 decimals
            Speed.x = Mathf.Round(Speed.x * 100f) / 100f;
            Speed.y = Mathf.Round(Speed.y * 100f) / 100f;
            Speed.z = Mathf.Round(Speed.z * 100f) / 100f;
            _positionLastFrame = this.transform.position;
        }

        /// <summary>
        /// Checks if the character is on the ground using a raycast.
        /// </summary>
        //protected virtual void CheckGround()
        //{
        //    Vector3 rayOrigin = transform.position + GroundCheckStartOffset;
        //    Vector3 rayOriginLeft = rayOrigin - new Vector3(GroundCheckWidth / 2, 0);
        //    Vector3 rayOriginRight = rayOrigin + new Vector3(GroundCheckWidth / 2, 0);

        //    Physics.Raycast(rayOriginLeft, GroundCheckDirection, out RaycastHit hitLeft, GroundCheckLength, GroundLayerMask);
        //    Physics.Raycast(rayOriginRight, GroundCheckDirection, out RaycastHit hitRight, GroundCheckLength, GroundLayerMask);

        //    _isGrounded = hitLeft.collider != null || hitRight.collider != null;
        //}

        /// <summary>
        /// Applies an impact to the character's movement.
        /// </summary>
        protected virtual void ApplyImpact()
        {
            if (_impact.magnitude > 0.2f)
            {
                _rigidBody.AddForce(_impact);
            }
            _impact = Vector3.Lerp(_impact, Vector3.zero, 5f * Time.deltaTime);
        }

        #region PUBLIC METHODS

        /// <summary>
        /// Applies an impact to the character, moving it in the specified direction with the specified force.
        /// </summary>
        /// <param name="direction">Direction of the impact.</param>
        /// <param name="force">Magnitude of the impact force.</param>
        public virtual void Impact(Vector3 direction, float force)
        {
            direction = direction.normalized;
            _impact += direction * force;
        }

        /// <summary>
        /// Sets the current movement direction and magnitude.
        /// </summary>
        /// <param name="movement">Desired movement vector.</param>
        public virtual void SetMovement(Vector3 movement)
        {
            CurrentMovement = movement;
        }

        /// <summary>
        /// Adds a force of the specified vector.
        /// </summary>
        /// <param name="movement">The force to be added.</param>
        public virtual void AddForce(Vector3 movement)
        {
            Impact(movement.normalized, movement.magnitude);
        }

        /// <summary>
        /// Tries to move the character to the specified position.
        /// </summary>
        /// <param name="newPosition">The desired position.</param>
        public virtual void MovePosition(Vector3 newPosition)
        {
            _rigidBody.MovePosition(newPosition);
        }

        /// <summary>
        /// Sets the character's Rigidbody as kinematic or non-kinematic.
        /// </summary>
        /// <param name="state">The desired kinematic state.</param>
        public virtual void SetKinematic(bool state)
        {
            _rigidBody.isKinematic = state;
        }

        /// <summary>
        /// Enables the character's collider.
        /// </summary>
        public virtual void CollisionsOn()
        {
            _collider.enabled = true;
        }

        /// <summary>
        /// Disables the character's collider.
        /// </summary>
        public virtual void CollisionsOff()
        {
            _collider.enabled = false;
        }

        #endregion
    }
}
