using FMODUnity;

using UnityEngine;

namespace JadePhoenix.Gameplay
{
    public class EnemyAudio : MonoBehaviour
    {
        [SerializeField] private EventReference enemyDefeated;
        [SerializeField] private EventReference enemyAttacking;
        [SerializeField] private EventReference enemyTaunt;
        [SerializeField] private EventReference enemyTakeHit;

        private void Start()
        {
            if (TryGetComponent(out Health health))
            {
                Debug.Log($"Adding EnemyAudio OnDeath.OnHit delegates to Health component on: {health.name}");
                health.OnDeath += PlayEnemyDefeatedSound;
                health.OnHit += PlayEnemyTakeHitSound;
            }
        }

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

        // Wrapper method for Health.OnHit delegate that passes an instigator param.
        public void PlayEnemyTakeHitSound(GameObject instigator)
        {
            PlayEnemyTakeHitSound();
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




