using UnityEngine;

namespace octr.Loot
{
    public class HealthPowerUp : MonoBehaviour, ILootable
    {
        public void Collect(Drop item)
        {
            Debug.Log($"{item.value}% Health Restored");
            Destroy(gameObject);
        }
    }
}