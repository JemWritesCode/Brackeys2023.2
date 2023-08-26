using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class SpecialRestore : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item)
        {
            //put logic here to return if special skill is ready (100%)
            //if(specialSkill.value == 100) return; (Example)

            if (item is Drop drop)
            {
                //Needs to restore the players special skill cooldown by value% (information can be pulled from drop)
                //Special.restoreCooldown(drop.value); (Example)
                Debug.Log($"{drop.value}% Special Cooldown Restored");
                Destroy(gameObject);
            }
        }
    }
}

