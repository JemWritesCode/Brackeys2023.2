using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class WaveManager : Singleton<WaveManager>
    {
        public List<GameObject> WaveList = new List<GameObject>();

        [ReadOnly]
        public int CurrentWaveIndex = 0;

        protected override void Awake()
        {
            base.Awake();

            // Loop over each child of the parent object
            for (int i = 0; i < transform.childCount; i++)
            {
                // Get the current child GameObject
                GameObject child = transform.GetChild(i).gameObject;

                // Add the child to the list
                WaveList.Add(child);
            }

            CurrentWaveIndex = 0;
            WaveList[CurrentWaveIndex].SetActive(true);
        }

        public virtual void EnableNextWave()
        {
            // Increment CurrentWaveIndex
            CurrentWaveIndex++;

            // Check if CurrentWaveIndex is within the bounds of WaveList
            if (CurrentWaveIndex >= 0 && CurrentWaveIndex < WaveList.Count)
            {
                // Disable the previous wave if it's not null
                if (CurrentWaveIndex > 0 && WaveList[CurrentWaveIndex - 1] != null)
                {
                    WaveList[CurrentWaveIndex - 1].SetActive(false);
                }

                // Enable the current wave if it's not null
                if (WaveList[CurrentWaveIndex] != null)
                {
                    WaveList[CurrentWaveIndex].SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("No more waves!", gameObject);
            }
        }
    }
}
