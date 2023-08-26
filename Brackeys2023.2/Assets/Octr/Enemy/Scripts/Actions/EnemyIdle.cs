using JadePhoenix.Tools;
using UnityEngine;

namespace octr.Enemy
{
    /// <summary>
    /// AI Action to stop the movement of an enemy.
    /// </summary>
    public class EnemyIdle : AIAction
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
            StopMovement();
        }

        /// <summary>
        /// Stops the movement of the character.
        /// </summary>
        protected virtual void StopMovement()
        {
            // Add Stop Moving Logic
            Debug.Log("I am not walking");
        }

        /// <summary>
        /// Allows the character to move again when exiting the state.
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            // isMoving = true;
        }
    }
}