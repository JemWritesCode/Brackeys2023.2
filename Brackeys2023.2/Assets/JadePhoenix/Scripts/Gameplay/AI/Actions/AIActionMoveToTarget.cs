using JadePhoenix.Tools;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    [RequireComponent(typeof(CharacterMovement))]
    public class AIActionMoveToTarget : AIAction
    {
        protected CharacterMovement _characterMovement;

        protected override void Initialization()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }

        public override void PerformAction()
        {
            MoveToTarget();
        }

        protected virtual void MoveToTarget()
        {
            Vector3 direction = (_brain.Target.position - transform.position);
            Vector2 convertedDirection = new Vector2(direction.x, direction.z);
            _characterMovement.SetMovement(convertedDirection);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement.SetMovement(Vector2.zero);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (_brain == null || _brain.Target == null)
                return;

            // Draw a yellow line from the AI character to its target
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _brain.Target.position);

            // Draw a blue line in the direction the AI character is attempting to move
            Gizmos.color = Color.blue;
            Vector3 direction = (_brain.Target.position - transform.position).normalized;
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }
    }
}
