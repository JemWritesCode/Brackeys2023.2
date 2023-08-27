using JadePhoenix.Gameplay;
using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class HealthUp : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item, Character character)
        {
            //put logic here to return if health is max value (eg: 999)
            //if(Player.health == 999) return; (Example)

            if (item is Drop drop)
            {
                //Needs to increase the players max health value by value% permanently (information can be pulled from drop)

                Health targetHealth = character.Health;
                targetHealth.IncreaseMaxHealth(drop.value, this.gameObject);

                //Player.health.MaxIncrease(drop.value); (Example)
                Debug.Log($"+{drop.value}% Permanent Health Increase");
            }
        }
    }
}