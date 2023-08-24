using SUPERCharacter;
using UnityEngine;

namespace octr.Loot
{
    public class AttackPowerUp : MonoBehaviour, ILootable
    {
        public void Collect<T>(T item)
        {
            if (item is TimedBuff timedBuff)
            {
                //Needs to increase the players attack value by value(%) for buffDuration(seconds) (information can be pulled from timedBuff
                //Player.attack.increaseDamage(timedBuff.value, timedBuff.buffDuration); (Example)
                Debug.Log($"+{timedBuff.value}% Attack ({timedBuff.buffDuration})");
                Destroy(gameObject);
            }
        }
    }
}