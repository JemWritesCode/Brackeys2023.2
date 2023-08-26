using _Game;
using JadePhoenix.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class DamageIncrease : MonoBehaviour, ILootable
    {
        protected List<Skill> _skills = new List<Skill>();
        protected TimedBuff _timedBuff;
        CharacterSkillHandler _skillHandler;

        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item, Character character)
        {
            //put logic here to return if attack buff is active

            if (item is TimedBuff timedBuff)
            {
                _timedBuff = timedBuff;

                EnablePowerup(character);

                Invoke(nameof(DisablePowerup), _timedBuff.buffDuration);

                Debug.Log($"+{timedBuff.value}% Attack ({timedBuff.buffDuration}s)");
                Destroy(gameObject);
            }
        }

        protected virtual void EnablePowerup(Character character)
        {
            _skillHandler = character.GetAbility<CharacterSkillHandler>();
            if (_skillHandler != null)
            {
                _skillHandler.DamageBonusPercentage += _timedBuff.value;
            }
        }

        protected virtual void DisablePowerup()
        {
            if (_skillHandler != null)
            {
                _skillHandler.DamageBonusPercentage -= _timedBuff.value;
            }
        }
    }
}