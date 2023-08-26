using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class Agility : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item)
        {
            //put logic here to return if cooldown is already reduced (-value% of original)

            if (item is TimedBuff timedBuff)
            {
                //Needs to reduce the players mobility skill cooldown value by value(%) for buffDuration(seconds) (information can be pulled from timedBuff
                //Player.mobility.decreaseCooldown(timedBuff.value, timedBuff.buffDuration); (Example)
                Debug.Log($"-{timedBuff.value}% Agility Cooldown ({timedBuff.buffDuration}s)");
                Destroy(gameObject);
            }
        }
    }
}

