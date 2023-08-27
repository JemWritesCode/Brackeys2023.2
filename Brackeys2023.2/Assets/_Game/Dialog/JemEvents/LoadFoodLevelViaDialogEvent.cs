using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/Event/LoadFoodLevel")]
    [System.Serializable]
    public class SceneManagerEvent : DialogueEventSO
    {
        public override void RunEvent()
        {
            Debug.Log("Attempting to load next scene: 1-FoodLevel");
            SceneManager.LoadScene("1-FoodLevel");

            base.RunEvent();
        }
    }
}
