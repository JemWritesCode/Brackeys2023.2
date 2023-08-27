using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/Event/DisableNPCAttachedTo")]
    [System.Serializable]
    public class DisableNPCAttachedTo : DialogueEventSO
    {
        public override void RunEvent()
        {
            Debug.Log("Attempting to DisableNPCAttachedTo");
            
            // whatever npc has this dialog tree attached. although I probably have to go up another level too

            base.RunEvent();
        }
    }
}
