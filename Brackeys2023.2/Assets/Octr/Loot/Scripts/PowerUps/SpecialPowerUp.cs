using octr.Loot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPowerUp : MonoBehaviour, ILootable
{
    public void Collect<T>(T item)
    {
        //put logic here to return if special skill is ready (100%)
        //if(specialSkill.value == 100) return; (Example)

        if (item is Drop drop)
        {
            //Needs to restore the players special skill cooldown by value% (information can be pulled from drop)
            //Player.special.restoreCooldown(drop.value); (Example)
            Debug.Log($"{drop.value}% Special Cooldown Restored");
            Destroy(gameObject);
        }
    }
}
