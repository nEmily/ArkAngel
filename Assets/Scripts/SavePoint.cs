using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {

    public GameObject gameController;

	// Use this for initialization
	void Action()
    {
        gameController.GetComponent <GameController> ().SaveGame();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            this.GetComponent<Light>().enabled = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Action();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "player")
        {
            this.GetComponent<Light>().enabled = false;
        }
    }
}
