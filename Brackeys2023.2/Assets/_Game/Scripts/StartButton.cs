using JadePhoenix.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public GameObject inputManager;
    public GameObject player;

    private void Start()
    {
        inputManager = GameObject.Find("InputManager");
        player = GameObject.FindGameObjectWithTag("Player");
        inputManager.SetActive(false);
        player.GetComponent<CharacterAiming>().enabled = false;
    }

    public void clickedStart()
    {
        inputManager.SetActive(true);
        //hide the start canvas
        gameObject.transform.parent.gameObject.SetActive(false);
        player.GetComponent<CharacterAiming>().enabled = true;
    }
}
