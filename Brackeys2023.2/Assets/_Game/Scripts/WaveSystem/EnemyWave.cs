using UnityEngine;
using System.Collections.Generic;

namespace _Game
{
    public class EnemyWave : Wave
    {
        private List<GameObject> enemies;

        // Initialize the enemies list with all active child objects
        void Start()
        {
            enemies = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child.activeSelf)
                {
                    enemies.Add(child);
                }
            }
        }

        protected override bool CheckCondition()
        {
            // Check if all enemies are disabled
            foreach (GameObject enemy in enemies)
            {
                if (enemy.activeSelf)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
