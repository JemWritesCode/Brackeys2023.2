using JadePhoenix.Tools;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    public class AIDecisionTargetIsNull : AIDecision
    {
        /// <summary>
        /// On Decide we check if our target is null
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return CheckIfTargetIsNull();
        }

        /// <summary>
        /// Returns true if the current target is null
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckIfTargetIsNull()
        {
            return _brain.Target == null;
        }
    }
}
