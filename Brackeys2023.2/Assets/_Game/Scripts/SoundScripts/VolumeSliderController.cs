using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using TMPro;

public class VolumeSliderController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField field = null;

    [SerializeField]
    private Slider slider = null;

    [SerializeField]
    private string busPath = "";

    private FMOD.Studio.Bus bus;

    private void Start()
    {
        if(busPath != "")
        {
            bus = RuntimeManager.GetBus(busPath);
        }

        bus.getVolume(out float volume);
        slider.value = volume * slider.maxValue;

        UpdateSliderOutput();
    }

    public void UpdateSliderOutput()
    {
        if (field != null && slider != null)
        {
            field.text = slider.value.ToString("#0%");
            bus.setVolume(slider.value / slider.maxValue);
        }
    }
}
