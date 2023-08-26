using JadePhoenix.Tools;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    [RequireComponent(typeof(CharacterMovement))]
    public class AIActionDirectToTarget : AIAction
    {
        protected TopDownController _characterMovement;

        protected override void Initialization()
        {
            _characterMovement = GetComponent<TopDownController>();
        }

        public override void PerformAction()
        {
            MoveToTarget();
        }

        protected virtual void MoveToTarget()
        {
            Vector3 direction = (_brain.Target.position - transform.position);
            _characterMovement.SetMovement(direction);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement.SetMovement(Vector2.zero);
        }
    }
}
