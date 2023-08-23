using UnityEngine;

namespace octr.Loot
{
    /// <summary>
    /// This is the base Item scriptable object class that all other items inherit from
    /// </summary>
    [CreateAssetMenu(fileName = "Drop", menuName = "_Loot/Drops/Default")]
    public class Drop : ScriptableObject
    {
        public ItemRarity rarity = ItemRarity.Common;
        public string description;
        public Sprite icon;
        public int value;
        public GameObject prefab;
    }
}
