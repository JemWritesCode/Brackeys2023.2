using UnityEngine;

namespace octr.Loot
{
    public class DropSpawner : MonoBehaviour
    {
        [SerializeField] private DropTable table;

        public void GenerateDrops()
        {
            foreach (DropTableElement element in table.elements)
            {
                int seed = Random.Range(1, 101);
                Debug.Log($"[GenerateDrops] Seed: {seed}");
                if (seed >= element.dropRate) continue;
                DropItem(element.drop);
                if (table.isSingular) return;
            }
        }

        public void DropItem(Drop drop)
        {
            Debug.Log($"Dropped Item: {drop.name}");

            //Spawn The Item (In World)
            GameObject newDrop = Instantiate(drop.prefab, gameObject.transform);
            newDrop.name = drop.name;

            Pickup pickup;

            if (newDrop.TryGetComponent(out pickup))
            {
                pickup.item = drop;
                Debug.Log($"{newDrop.name} Assigned: {drop.name}");
            }
            else
            {
                Debug.LogError($"Please attach a Pickup Component to {newDrop.name}");
            }
        }
    }
}
