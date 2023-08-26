using JadePhoenix.Tools;
using UnityEngine;

namespace octr.Enemy
{
    /// <summary>
    /// AI Action to stop the movement of an enemy.
    /// </summary>
    public class EnemyWalk : AIAction
    {
        protected EnemyAI _enemy;

        /// <summary>
        /// Initialize variables.
        /// </summary>
        protected override void Initialization()
        {
            _enemy = GetComponentInParent<EnemyAI>();
        }

        /// <summary>
        /// Perform the action to stop the movement.
        /// </summary>
        public override void PerformAction()
        {
            StartMovement();
        }

        /// <summary>
        /// Stops the movement of the character.
        /// </summary>
        protected virtual void StartMovement()
        {
            // Add Stop Moving Logic
            Debug.Log("I am walking");
        }

        /// <summary>
        /// Allows the character to move again when exiting the state.
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            // isMoving = false;
        }
    }
}