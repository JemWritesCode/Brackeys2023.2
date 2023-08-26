using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    [CreateAssetMenu(fileName = "New MeleeParrySkill", menuName = "Skills/MeleeParry")]
    public class MeleeParrySkill : MeleeSkill
    {
        [Header("Parry Skill")]
        [Tooltip("Bonus damage percentage when hit by an attack.")]
        public float ParryBonus = 0f;

        protected bool _wasHit = false;

        public override void OnHit(GameObject instigator)
        {
            base.OnHit(instigator);

            _wasHit = true;
            ImpactPercentageModifier += ParryBonus;
        }

        public override void SkillStop()
        {
            base.SkillStop();

            _wasHit = false;
            ImpactPercentageModifier -= ParryBonus;
        }
    }
}
