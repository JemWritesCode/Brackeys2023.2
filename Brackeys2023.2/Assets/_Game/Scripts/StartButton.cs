using JadePhoenix.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    GameObject player;

    [SerializeField] public void clickedStart()
    {
        player = GameObject.FindWithTag("Player");
        player.GetComponentInChildren<InputManager>().enabled = true;
        
        //hide the start canvas
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
