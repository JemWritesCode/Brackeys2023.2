using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using UnityEngine;

namespace _Game
{
    public class AIActionUseSkill : AIAction
    {
        [Header("Skill Settings")]
        [Tooltip("Index of the skill in the CharacterSkillHandler Skills list")]
        public int skillIndex;

        protected CharacterSkillHandler skillHandler;

        protected override void Initialization()
        {
            base.Initialization();
            skillHandler = this.gameObject.GetComponent<CharacterSkillHandler>();
            if (skillHandler == null)
            {
                Debug.LogError("No CharacterSkillHandler found on the game object");
            }
        }

        public override void PerformAction()
        {
            if (skillHandler != null && skillHandler.Skills.Count > skillIndex)
            {
                Skill skill = skillHandler.Skills[skillIndex];
                skillHandler.StartSkill(skill);
            }
            else
            {
                Debug.LogError("Skill index out of bounds or no skill handler attached to the game object");
            }
        }
    }
}
