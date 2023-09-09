using JadePhoenix.Gameplay;

using UnityEngine;

namespace _Game {
  public class HealthExtended : Health {
    [field: SerializeField]
    public EnemyAudio EnemyAudio { get; private set; }

    protected override void Initialization() {
      base.Initialization();

      if (!EnemyAudio) {
        EnemyAudio = GetComponent<EnemyAudio>();
      }

      OnDeath += PlaySoundOnEnemyDefeated;
      OnHit += PlaySoundOnEnemyHit;
    }

    public void PlaySoundOnEnemyDefeated() {
      if (EnemyAudio) {
        EnemyAudio.PlayEnemyDefeatedSound();
      }
    }

    public void PlaySoundOnEnemyHit(GameObject instigator) {
      if (EnemyAudio) {
        EnemyAudio.PlayEnemyTakeHitSound();
      }
    }
  }
}