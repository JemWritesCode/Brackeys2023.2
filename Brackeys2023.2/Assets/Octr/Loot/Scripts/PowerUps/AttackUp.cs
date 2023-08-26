using UnityEngine;

namespace octr.Loot.PowerUps
{
    public class AttackUp : MonoBehaviour, ILootable
    {
        /// <summary>
        /// Generic Method Implementation From Interface (ILootable) executed by Pickup.OnTriggerEnter()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Collect<T>(T item)
        {
            //put logic here to return if attack is max (eg: 999)
            //if(Player.attack == 100) return; (Example)

            if (item is Drop drop)
            {
                //Needs to increase the players damage value by value% permanently (information can be pulled from drop)

                //Player.attack.increaseMaxAttack(drop.value); (Example)
                Debug.Log($"+{drop.value}% Permanent Attack Increase");
                Destroy(gameObject);
            }
        }
    }
}