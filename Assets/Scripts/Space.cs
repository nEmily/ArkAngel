using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Space : MonoBehaviour {

    public GameObject levelController;
    public TimeTransition timeTransition;
    public GameObject hallway;

    public GameObject player;
    private string playerName;

    public void StartSpaceScene()
    {
        this.gameObject.transform.parent.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
        player.GetComponent<PlayerMovement>().FreezePlayer();
        player.GetComponent<AudioSource>().Stop();
        FindObjectOfType<DialogueRunner>().StartDialogue("SpaceScene");
    }

    public void EndSpaceScene()
    {
        player.GetComponent<PlayerMovement>().UnfreezePlayer();
        timeTransition.StartTimeTransition(4);
        hallway.SetActive(false);
        this.gameObject.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
