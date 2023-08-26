using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class DamageIncrease : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item)
        {
            //put logic here to return if attack buff is active

            if (item is TimedBuff timedBuff)
            {
                //Needs to increase the players attack value by value(%) for buffDuration(seconds) (information can be pulled from timedBuff
                //Player.attack.increaseDamage(timedBuff.value, timedBuff.buffDuration); (Example)
                Debug.Log($"+{timedBuff.value}% Attack ({timedBuff.buffDuration}s)");
                Destroy(gameObject);
            }
        }
    }
}