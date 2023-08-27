using UnityEngine;
using JadePhoenix.Tools;
using JadePhoenix.Gameplay;

namespace _Game
{
    public class RotateTowardsTarget : AIAction
    {
        private CharacterAiming characterAiming;

        protected override void Initialization()
        {
            base.Initialization();
            characterAiming = this.gameObject.GetComponent<CharacterAiming>();
        }

        public override void PerformAction()
        {
            RotateTowards(_brain.Target);
        }

        private void RotateTowards(Transform target)
        {
            if (target == null)
            {
                return;
            }

            Vector3 direction = target.transform.position - transform.position;
            characterAiming.RotateTowardsDirection(direction);
        }
    }
}
