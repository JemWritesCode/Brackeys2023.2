using _Game;
using JadePhoenix.Gameplay;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/Event/SkillHandlerOn")]
    [System.Serializable]
    public class SkillHandlerOn : DialogueEventSO
    {
        
        public GameObject playerObject;

        public override void RunEvent()
        {
            Debug.Log("Attempting to SkillHandlerOff");

            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerObject.GetComponent<CharacterSkillHandler>().enabled = true;
                playerObject.GetComponent<CharacterMovement>().enabled = true;
            }

            base.RunEvent();
        }
    }
}
