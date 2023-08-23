using System.Xml.Linq;
using UnityEngine;

namespace octr.Loot
{
    public class DropSpawner : MonoBehaviour
    {
        [SerializeField] private DropTable table;
        [SerializeField] private float spawnRadius = 5f;
        [SerializeField] private int maxSpawnAttempts = 10;
        [SerializeField] private float overlapRadius = 1f;

        public void GenerateDrops()
        {
            foreach (DropTableElement element in table.elements)
            {
                int seed = Random.Range(1, 101);
                Debug.Log($"[GenerateDrops] Seed: {seed}");
                if (seed >= element.dropRate) continue;

                // Spawn The Item (In World) using the SpawnZone
                SpawnObject(element.drop.prefab, element);

                if (table.isSingular) return;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }

        public void SpawnObject(GameObject spawnObject, DropTableElement element)
        {
            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
                randomPoint.y = 0f; // If you want objects to spawn at ground level
                Collider[] colliders = Physics.OverlapSphere(randomPoint, overlapRadius); // Adjust the radius for overlap check

                if (colliders.Length == 0)
                {
                    GameObject newDrop = Instantiate(spawnObject, randomPoint, Quaternion.identity);
                    newDrop.transform.parent = gameObject.transform;
                    newDrop.name = element.drop.name;

                    Pickup pickup;

                    if (newDrop.TryGetComponent(out pickup))
                    {
                        pickup.item = element.drop;
                        Debug.Log($"{newDrop.name} Assigned: {element.drop.name}");
                    }
                    else
                    {
                        Debug.LogError($"Please attach a Pickup Component to {newDrop.name}");
                    }
                    break;
                }
            }
        }
    }
}
