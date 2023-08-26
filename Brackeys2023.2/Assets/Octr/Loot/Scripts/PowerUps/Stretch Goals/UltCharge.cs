using JadePhoenix.Gameplay;
using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class UltCharge : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item, Character character)
        {
            //put logic here to return if armor is already increased (+value% of original)

            if (item is TimedBuff timedBuff)
            {
                //Needs to increase the ultmate charge speed by value(%) for buffDuration(seconds) (information can be pulled from timedBuff)
                //Ult.IncreaseSpeed(timedBuff.value, timedBuff.buffDuration); (Example)
                Debug.Log($"+{timedBuff.value}% Ultimate Charge Speed ({timedBuff.buffDuration}s)");
                Destroy(gameObject);
            }
        }
    }
}