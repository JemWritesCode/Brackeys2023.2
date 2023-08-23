using System;
using UnityEngine;

namespace octr.Loot
{
    [CreateAssetMenu(fileName = "New Table", menuName = "_Loot/Table")]
    public class DropTable : ScriptableObject
    {
        /// <summary>
        /// This class handles the creation of a Loot Table Scriptable Object
        /// </summary>
        public DropTableElement[] elements;
        public bool isSingular = false;
        
        public void OnValidate()
        {
            ValidateName();
        }

        #region Validate
        public void ValidateName()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].drop != null)
                {
                    elements[i].elementName = $"{elements[i].drop.name} ({i})";
                }
                else
                {
                    elements[i].elementName = $"Empty ({i})";
                }
            }
        }
        #endregion
    }
}
