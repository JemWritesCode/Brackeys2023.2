using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class UltimateRestore : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item)
        {
            //put logic here to return if ultimate is full (100%)
            //if(Player.ultValue == 100) return; (Example)

            if (item is Drop drop)
            {
                //Needs to restore the players ultimate by value% (information can be pulled from drop)

                //Ult.Restore(drop.value); (Example)
                Debug.Log($"{drop.value}% Ultimate Restored");
                Destroy(gameObject);
            }
        }
    }
}