using JadePhoenix.Gameplay;
using octr.Audio;
using octr.Loot;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    public Drop item;
    public TextMeshPro itemTMP;

    public ILootable powerUp;
    public RandomizePitch randomizePitch;

    public Collider _collider;

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
        Character collidedCharacter = other.GetComponent<Character>();

        if (collidedCharacter == null) { return; }

        if(collidedCharacter.PlayerID == "Player")
        {
            randomizePitch.PlayRandom();
            powerUp.Collect(item, collidedCharacter);
            _collider.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnValidate()
    {
        if (gameObject.TryGetComponent(out _collider))
        {
            if (!_collider.isTrigger)
            {
                Debug.LogError($"Please set {_collider.name} isTrigger to True");
            }
        }
        else
        {
            Debug.LogError($"Collider Component Not Found");
        }        
    }
}
