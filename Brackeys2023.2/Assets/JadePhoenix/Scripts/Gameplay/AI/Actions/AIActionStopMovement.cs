using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    [RequireComponent(typeof(CharacterMovement))]
    /// <summary>
    /// AI Action to stop the movement of a character.
    /// </summary>
    public class AIActionStopMovement : AIAction
    {
        protected CharacterMovement _characterMovement;

        /// <summary>
        /// Initialize variables.
        /// </summary>
        protected override void Initialization()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }

        /// <summary>
        /// Perform the action to stop the movement.
        /// </summary>
        public override void PerformAction()
        {
            StopMovement();
        }

        /// <summary>
        /// Stops the movement of the character.
        /// </summary>
        protected virtual void StopMovement()
        {
            _characterMovement.MovementForbidden = true;
        }

        /// <summary>
        /// Allows the character to move again when exiting the state.
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement.MovementForbidden = false;
        }
    }
}
