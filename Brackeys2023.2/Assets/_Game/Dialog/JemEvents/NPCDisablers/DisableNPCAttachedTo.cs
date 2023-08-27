using JetBrains.Annotations;
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
        public string tagName;
        [SerializeField] public GameObject objectToDisable;

        public override void RunEvent()
        {
            Debug.Log("Attempting to DisableNPCAttachedTo");

            objectToDisable = GameObject.FindGameObjectWithTag(tagName);
            if (objectToDisable != null)
            {
                objectToDisable.SetActive(false);
            }

            base.RunEvent();
        }
    }
}
