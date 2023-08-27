using _Game;
using JadePhoenix.Gameplay;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/Event/SkillHandlerOff")]
    [System.Serializable]
    public class SkillHandlerOff : DialogueEventSO
    {
        
        public GameObject playerObject;
        public GameObject inputManager;

        public override void RunEvent()
        {
            Debug.Log("Attempting to SkillHandlerOff");

            playerObject = GameObject.FindGameObjectWithTag("Player");
            inputManager = GameObject.FindGameObjectWithTag("InputManager");
            if (playerObject != null)
            {
                playerObject.GetComponent<CharacterSkillHandler>().enabled = false;
                inputManager.SetActive(false);
            }

            base.RunEvent();
        }
    }
}
