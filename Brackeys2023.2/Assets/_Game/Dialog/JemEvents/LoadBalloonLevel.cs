using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/Event/LoadBalloonLevel")]
    [System.Serializable]
    public class LoadBalloonLevelEvent : DialogueEventSO
    {
        public override void RunEvent()
        {
            Debug.Log("Attempting to load next scene: 2-BalloonLevel");
            SceneManager.LoadScene("2-BalloonLevel");

            base.RunEvent();
        }
    }
}
