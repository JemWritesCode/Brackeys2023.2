using JadePhoenix.Gameplay;
using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class UltChargeKO : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item, Character character)
        {
            //put logic here to return if ultimate buff is already active

            if (item is TimedBuff timedBuff)
            {
                //Needs to increase the players ultimate charge rate by value(%) for buffDuration(seconds) when killing enemies (information can be pulled from timedBuff
                //Player.mobility.decreaseCooldown(timedBuff.value, timedBuff.buffDuration); (Example)
                Debug.Log($"+{timedBuff.value}% Ultimate KO Charge Rate ({timedBuff.buffDuration}s)");
                Destroy(gameObject);
            }
        }
    }
}

