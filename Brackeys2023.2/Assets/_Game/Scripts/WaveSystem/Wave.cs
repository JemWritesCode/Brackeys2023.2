using UnityEngine;

namespace _Game
{
    public abstract class Wave : MonoBehaviour
    {
        // Update is called once per frame
        protected virtual void Update()
        {
            // Check the condition defined in the derived class
            if (CheckCondition())
            {
                // If the condition is met, enable the next wave
                WaveManager.Instance.EnableNextWave();
            }
        }

        // Abstract method to be implemented by the derived class
        // This method should return true when the condition to enable the next wave is met
        protected abstract bool CheckCondition();
    }
}
