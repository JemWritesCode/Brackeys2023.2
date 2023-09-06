using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    public class EnemyAudio : MonoBehaviour
    {
        [SerializeField] private EventReference enemyDefeated;

        public void PlayEnemyDefeatedSound()
        {
            if (enemyDefeated.Path.Length > 0)
            {
                // jem: this is still playing somewhere in space. looking at the function it takes a vector 3 in some of the overloads
                // but without that shouldn't it be 2D...? s:
               
                RuntimeManager.PlayOneShot(enemyDefeated);
            }
        }
    }
}




