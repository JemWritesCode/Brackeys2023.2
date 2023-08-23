using System;
using UnityEngine;

namespace octr.Loot
{
    [Serializable]
    public class DropTableElement
    {
        /// <summary>
        /// This is a class for the loot in a loot table to determine the items, their drop count and their drop rate
        /// </summary>

        [HideInInspector] public string elementName; //Used to name Inspector Elements

        [Tooltip("Place scriptable object item here")]
        public Drop drop;

        [Tooltip("How likely is it to drop? 0 = Never Drops, 100 = Always Drops")]
        [Range(0, 100)] public int dropRate;
    }
}

