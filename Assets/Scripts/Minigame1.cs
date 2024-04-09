using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class Minigame1 : MonoBehaviour {

    public GameObject player;

    public List<GameObject> parts;
    public GameObject[] layout;
    public GameObject targetHolder;

    //public Image incorrectScreen;
    public GameObject completeScreen;
    public GameObject endScreen;
    public GameObject optionsScreen;
    public List<GameObject> removeAfterWinning;

    public GameObject levelController;
    public AudioClip correctSound;
    public AudioClip winSound;
    private AudioSource soundEffects;
    public GameObject backgroundMusic;

    private bool playGame = true;
    private int step = 0;
    private GameObject target;
    private int index;
    private float speed = .2f;
    private float errorMargin = .1f;

    //private Color incorrectColor = new Color(1f, 0f, 0f, 1f);
    //private Color clear = new Color(0f, 0f, 0f, 0f);
    //private bool incorrect;

    void Start() {
        index = 0;
        target = parts[index];
        target.SetActive(true);
        //incorrect = false;
        soundEffects = levelController.GetComponent<AudioSource>();
        player.SetActive(false);
    }

    void Update() {
        if (playGame)
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                RotateRight();
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                RotateLeft();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                UpButton();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                DownButton();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpaceButton();
            }
        }
    }

    public void RotateRight()
    {
        target.transform.Rotate(new Vector3(0, 0, -20) * speed);
        // targetHolder.transform.Rotate(new Vector3(0, 0, -20) * speed);
    }

    public void RotateLeft()
    {
        target.transform.Rotate(new Vector3(0, 0, 20) * speed);
        // targetHolder.transform.Rotate(new Vector3(0, 0, 20) * speed);
    }

    public void UpButton()
    {
        target.SetActive(false);
        index = (index - 1) % parts.Count;
        if (index < 0)
        {
            index = parts.Count - 1;
        }
        target = parts[index];
        target.SetActive(true);

    }

    public void DownButton()
    {
        target.SetActive(false);
        index = (index + 1) % parts.Count;
        target = parts[index];
        target.SetActive(true);
    }

    public void SpaceButton()
    {
        if (target.transform.rotation.z % 1 > -errorMargin && target.transform.rotation.z % 1 < errorMargin)
        {
            if (
            (step == 0 && target.name == "GadgetPart1") ||
            (step == 1 && target.name == "GadgetPart2") ||
            (step == 2 && target.name == "GadgetPart3") ||
            (step == 3 && target.name == "GadgetPart4") ||
            (step == 4 && target.name == "GadgetPart5"))
            {
                Correct();
            }
        } else if (step == 2 && target.name == "GadgetPart3" &&
            ((target.transform.rotation.z % 1 > -errorMargin && target.transform.rotation.z % 1 < errorMargin) ||
            (target.transform.rotation.z - 180) % 1 > -errorMargin && (target.transform.rotation.z - 180) % 1 < errorMargin))
        {
            Correct();
        } else
        {
            Incorrect();
        }
    }

    public void ButtonPress()
    {
        soundEffects.clip = correctSound;
        soundEffects.Play();
    }

    private void Correct()
    {
        ButtonPress();
        layout[step].SetActive(false);
        step = step + 1;
        layout[step].SetActive(true);

        parts.RemoveAt(index);
        if (parts.Count != 0)
        {
            UpButton();
        } else
        {
            StartCoroutine(Complete());
        }       
    }

    private void Incorrect()
    {
        //incorrectScreen.color = incorrectColor;
        //incorrect = true;
    }

    IEnumerator Complete()
    {
        soundEffects.clip = winSound;
        soundEffects.Play();
        target.SetActive(false);
        playGame = false;
        levelController.GetComponent<GameController>().LevelUp();

        yield return StartCoroutine(WaitTime(1f));

        yield return StartCoroutine(WaitForKeyDown(KeyCode.Space));

        foreach (GameObject item in removeAfterWinning)
        {
            item.SetActive(false);
        }

        completeScreen.SetActive(true);
        endScreen.SetActive(true);
        optionsScreen.SetActive(false);

        yield return StartCoroutine(WaitTime(1.5f));
        yield return StartCoroutine(WaitForKeyDown(KeyCode.Space));

        endScreen.SetActive(false);
        optionsScreen.SetActive(true);
        backgroundMusic.GetComponent<MusicLoop>().FadeOut();

    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
    }

    IEnumerator WaitTime(float num)
    {
        yield return new WaitForSecondsRealtime(num);
    }
}
