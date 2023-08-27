using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using UnityEngine;

namespace _Game
{
    [CreateAssetMenu(fileName = "New BoundedProjectileSkill", menuName = "Skills/BoundedProjectile")]
    public class BoundedProjectileSkill : ProjectileSkill
    {
        public enum BoxCenter
        {
            OwnerTransform,
            WorldOrigin
        }

        [Header("Bounded Spawn")]
        [Tooltip("The center of the box boundary.")]
        public BoxCenter BoxBoundaryCenter = BoxCenter.OwnerTransform;

        [Tooltip("The offset of the box boundary from its center.")]
        public Vector3 BoxOffset = Vector3.zero;

        [Tooltip("The dimensions of the box boundary within which the projectile can spawn.")]
        public Vector3 BoxBoundary = new Vector3(10, 10, 10);

        [Tooltip("The radius of the circle centered on Owner.transform where the projectile cannot spawn.")]
        public float NoSpawnRadius = 3;

        [Tooltip("The height at which the projectile will spawn.")]
        public float SpawnHeight = 1;

        public override void SkillUse()
        {
            for (int i = 0; i < ProjectilesPerShot; i++)
            {
                SpawnProjectile(GetBoundedSpawnPosition(), i, ProjectilesPerShot, true);
            }
        }

        public virtual Vector3 GetBoundedSpawnPosition()
        {
            Vector3 center = (BoxBoundaryCenter == BoxCenter.OwnerTransform) ? Owner.transform.position : Vector3.zero;
            Vector3 randomPosition = center + BoxOffset + new Vector3(
                UnityEngine.Random.Range(-BoxBoundary.x / 2, BoxBoundary.x / 2),
                SpawnHeight,
                UnityEngine.Random.Range(-BoxBoundary.z / 2, BoxBoundary.z / 2)
            );
            Vector3 heightVector = new Vector3(0, SpawnHeight, 0);

            while (Vector3.Distance(randomPosition, Owner.transform.position + heightVector) < NoSpawnRadius)
            {
                randomPosition = center + BoxOffset + new Vector3(
                    UnityEngine.Random.Range(-BoxBoundary.x / 2, BoxBoundary.x / 2),
                    SpawnHeight,
                    UnityEngine.Random.Range(-BoxBoundary.z / 2, BoxBoundary.z / 2)
                );
            }
            Debug.Log(randomPosition);
            return randomPosition;
        }

        //public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
        //{
        //    GameObject spawnedProjectile = base.SpawnProjectile(spawnPosition, projectileIndex, totalProjectiles, triggerObjectActivation);

        //    if (spawnedProjectile != null)
        //    {
        //        float randomYRotation = UnityEngine.Random.Range(0, 360);
        //        spawnedProjectile.transform.rotation *= Quaternion.Euler(0, randomYRotation, 0);
        //    }

        //    return spawnedProjectile;
        //}

        public override void DrawGizmos(GameObject owner)
        {
            Vector3 center = BoxBoundaryCenter == BoxCenter.OwnerTransform && owner != null ? owner.transform.position : Vector3.zero;
            Vector3 boxCenter = center + BoxOffset;

            // Draw the box boundary
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boxCenter, new Vector3(BoxBoundary.x, 0, BoxBoundary.z));

            // Draw the no spawn circle
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(owner.transform.position, NoSpawnRadius);

            // Draw the spawn height
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(boxCenter, boxCenter + Vector3.up * SpawnHeight);
        }
    }
}
