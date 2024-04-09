using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameTutorial : MonoBehaviour {

    public GameObject levelController;
    public GameObject minigame;
    private GameController gC;

    public void Start()
    {
        gC = levelController.GetComponent<GameController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gC.UnfreezeGame();
            StartCoroutine(Delay());
        }
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(2f);
        ActivateMinigame();
    }

    public void ActivateMinigame()
    {
        gC.UnfreezeGame();
        gC.ButtonSoundEffect();
        minigame.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
