using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEET_AND_TALK
{
    [System.Serializable]
    public class DialogueEventSO : ScriptableObject
    {
        public virtual void RunEvent()
        {
            //Debug.Log("Event called");
        }
    }
}