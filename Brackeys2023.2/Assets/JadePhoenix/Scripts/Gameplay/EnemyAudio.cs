using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    public class EnemyAudio : MonoBehaviour
    {
        [SerializeField] private EventReference enemyDefeated;
        [SerializeField] private EventReference enemyAttacking;
        [SerializeField] private EventReference enemyTaunt;
        [SerializeField] private EventReference enemyTakeHit;



        public void PlayEnemyDefeatedSound()
        {
            if (enemyDefeated.Path.Length > 0)
            {
                RuntimeManager.PlayOneShot(enemyDefeated);
            }
        }

        public void PlayEnemyAttackingSound()
        {
            if (enemyDefeated.Path.Length > 0)
            {
                RuntimeManager.PlayOneShot(enemyAttacking);
            }
        }

        public void PlayEnemyTauntSound()
        {
            if (enemyDefeated.Path.Length > 0)
            {
                RuntimeManager.PlayOneShot(enemyTaunt);
            }
        }

        public void PlayEnemyTakeHitSound()
        {
            if (enemyDefeated.Path.Length > 0)
            {
                RuntimeManager.PlayOneShot(enemyTakeHit);
            }
        }




    }
}




