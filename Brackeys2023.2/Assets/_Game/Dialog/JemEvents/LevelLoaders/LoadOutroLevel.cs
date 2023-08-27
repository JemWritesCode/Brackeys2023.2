using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/Event/LoadTherapistOutro")]
    [System.Serializable]
    public class LoadTherapistOutro : DialogueEventSO
    {
        public override void RunEvent()
        {
            Debug.Log("Attempting to load next scene: 3-TherapistOutro");
            SceneManager.LoadScene("3-TherapistOutro");

            base.RunEvent();
        }
    }
}
