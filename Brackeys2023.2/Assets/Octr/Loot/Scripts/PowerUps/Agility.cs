using _Game;
using JadePhoenix.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class Agility : MonoBehaviour, ILootable
    {
        protected List<Skill> _skills = new List<Skill>();
        protected TimedBuff _timedBuff;

        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item, Character character)
        {
            //put logic here to return if cooldown is already reduced (-value% of original)

            if (item is TimedBuff timedBuff)
            {
                _timedBuff = timedBuff;

                EnablePowerup(character);

                Invoke(nameof(DisablePowerup), _timedBuff.buffDuration);

                Debug.Log($"-{timedBuff.value}% Agility Cooldown ({timedBuff.buffDuration}s)");
            }
        }

        protected virtual void EnablePowerup(Character character)
        {
            CharacterSkillHandler skillHandler = character.GetAbility<CharacterSkillHandler>();
            _skills = skillHandler.GetActiveSkillsOfType(Skill.SkillTypes.Mobility);
            foreach (Skill skill in _skills)
            {
                skill.ModifyCooldown(_timedBuff.value);
            }
        }

        protected virtual void DisablePowerup()
        {
            foreach (Skill skill in _skills)
            {
                skill.ModifyCooldown(-_timedBuff.value);
            }
        }
    }
}

