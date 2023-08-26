using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class HealthRestore : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item)
        {
            //put logic here to return if health is full (100%)
            //if(Player.health == 100) return; (Example)

            if (item is Drop drop)
            {
                //Needs to restore the players health value by value% (information can be pulled from drop)

                //Player.health.restoreHealth(drop.value); (Example)
                Debug.Log($"{drop.value}% Health Restored");
                Destroy(gameObject);
            }
        }
    }
}