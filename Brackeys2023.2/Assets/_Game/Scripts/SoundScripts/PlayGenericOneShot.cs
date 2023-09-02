using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayGenericOneShot : MonoBehaviour
{
    [SerializeField] private EventReference soundEvent;

    public void PlaySoundEvent()
    {
        if(soundEvent.Path.Length > 0)
        {
            RuntimeManager.PlayOneShot(soundEvent);
        }
    }
}
