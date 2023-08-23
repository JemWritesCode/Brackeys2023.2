using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    /// <summary>
    /// Allows a character to aim based on the mouse position in a 3D environment.
    /// </summary>
    public class CharacterAiming : CharacterAbility
    {
        [Tooltip("The speed at which the character rotates to face the aim direction.")]
        public float RotationSpeed = 10f;
        [Tooltip("A model to use instead of the owner's CharacterModel. Can be safely left null.")]
        public GameObject OverrideModel;

        protected Camera _mainCamera;
        protected Vector3 _mousePosition;
        protected Vector3 _direction;
        protected Vector3 _currentAim;

        /// <summary>
        /// Initialization function for setting up character aiming.
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _mainCamera = Camera.main;

            if (OverrideModel == null) { return; }
            _model = OverrideModel;
        }

        /// <summary>
        /// Processes the aiming ability.
        /// </summary>
        public override void ProcessAbility()
        {
            GetMouseAim();
            RotateTowardsMouse();
        }

        /// <summary>
        /// Gets the mouse position and calculates the direction in which the character should aim.
        /// </summary>
        public virtual void GetMouseAim()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                _direction = hit.point;
            }

            // We get a flat direction (ignoring Y differences)
            Vector3 flatDirection = _direction - transform.position;
            flatDirection.y = 0;  // Reset Y difference to ensure flat direction
            _currentAim = flatDirection.normalized;
        }

        /// <summary>
        /// Rotates the character model to face the direction of the mouse.
        /// </summary>
        public virtual void RotateTowardsMouse()
        {
            Quaternion targetRotation = Quaternion.LookRotation(_currentAim);
            _model.transform.rotation = Quaternion.Slerp(_model.transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

            // Reset rotations on X and Z axes
            _model.transform.eulerAngles = new Vector3(0, _model.transform.eulerAngles.y, 0);
        }
    }
}
