using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    [RequireComponent(typeof(CharacterMovement))]
    /// <summary>
    /// AI Action to move the character to a target using a pathfinder.
    /// </summary>
    public class AIActionPathfinderToTarget : AIAction
    {
        protected CharacterMovement _characterMovement;
        protected CharacterPathfinder _characterPathfinder;

        /// <summary>
        /// Initialize variables.
        /// </summary>
        protected override void Initialization()
        {
            _characterMovement = GetComponent<CharacterMovement>();
            _characterPathfinder = GetComponent<CharacterPathfinder>();
        }

        /// <summary>
        /// Perform the action to move the character.
        /// </summary>
        public override void PerformAction()
        {
            Move();
        }

        /// <summary>
        /// Moves the character to the target.
        /// </summary>
        protected virtual void Move()
        {
            if (_brain.Target == null)
            {
                _characterPathfinder.SetNewDestination(null);
                return;
            }
            else
            {
                _characterPathfinder.SetNewDestination(_brain.Target.transform);
            }
        }

        /// <summary>
        /// Stops the character movement when exiting the state.
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement.SetMovement(Vector2.zero);
        }
    }
}
