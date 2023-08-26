using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class ArmorIncrease : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item)
        {
            //put logic here to return if armor is already increased (+value% of original)

            if (item is TimedBuff timedBuff)
            {
                //Needs to increase the players armor value by value(%) for buffDuration(seconds) (information can be pulled from timedBuff)
                //Armor.Increase(timedBuff.value, timedBuff.buffDuration); (Example)
                Debug.Log($"+{timedBuff.value}% Armor ({timedBuff.buffDuration}s)");
                Destroy(gameObject);
            }
        }
    }
}