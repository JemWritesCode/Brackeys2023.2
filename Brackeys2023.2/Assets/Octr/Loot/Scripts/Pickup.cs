using octr.Loot;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    public Drop item;
    public TextMeshPro itemTMP;

    public ILootable powerUp;

    private void Start()
    {
        ILootable lootable;

        if (gameObject.TryGetComponent(out lootable))
        {
            powerUp = lootable;
            Debug.Log($"{gameObject.name} Assigned: {powerUp.GetType().Name}");
        }
        else
        {
            Debug.LogError($"Please attach an ILootable to {gameObject.name}");
        }

        itemTMP.text = item.name;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            powerUp.Collect(item);
        }
    }

    private void OnValidate()
    {
        Collider collider;

        if (gameObject.TryGetComponent(out collider))
        {
            if (!collider.isTrigger)
            {
                Debug.LogError($"Please set {collider.name} isTrigger to True");
            }
        }
        else
        {
            Debug.LogError($"Collider Component Not Found");
        }        
    }
}