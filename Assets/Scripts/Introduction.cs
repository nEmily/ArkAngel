using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Introduction : MonoBehaviour {

    public GameObject textBox;
    public Text text;
    public string expo;
    public GameObject tutorialItems;
    public AudioClip buttonPress;
    public GameObject levelController;
    public AudioSource backgroundMusic;
    public bool hasTutorial = true;
    public bool fadeMusic = false;
    public bool hasTimeTransition = false;
    public TimeTransition timeTransition;
    public GameObject pauseButton;

    private GameObject player;

    private AudioSource soundEffect;

    //public GameObject newRoom;
    //public Text newRoomName;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().FreezePlayer();
        levelController.GetComponent<GameController>().FreezeGame();
        pauseButton.SetActive(false);

        backgroundMusic.GetComponent<MusicLoop>().FadeIn();
        if (hasTutorial)
        {
            tutorialItems.SetActive(false);
        }

        soundEffect = levelController.GetComponent<AudioSource>();
        textBox.SetActive(true);
        StartExpo();
    }

    public void StartExpo()
    {
        StartCoroutine(TypeSentence(expo));
    }

    IEnumerator TypeSentence(string sentence)
    {
        PlayerStatus playerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<PlayerStatus>();
        sentence = playerStatus.ReplaceName(sentence);
        sentence = playerStatus.ReplaceGender(sentence);

        text.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if (Input.anyKey == true)
            {
                text.text = sentence;
                yield return new WaitForSecondsRealtime(.01f);
                break;
            }
            text.text += letter;
            yield return new WaitForSecondsRealtime(.025f);
        }
        yield return StartCoroutine(WaitForKeyDown());
        if (hasTutorial)
        {
            StartCoroutine(StartTutorial());
        } else
        {
            EndIntro();
        }
        
    }

    IEnumerator WaitForKeyDown()
    {
        while (!Input.anyKeyDown)
            yield return null;
        PressButton();
    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
        PressButton();
    }

    IEnumerator StartTutorial()
    {
        textBox.SetActive(false);
        tutorialItems.SetActive(true);
        yield return new WaitForSecondsRealtime(0.01f);

        yield return StartCoroutine(WaitForKeyDown(KeyCode.Space));
        EndIntro();
    }

    public void EndIntro()
    {
        StopAllCoroutines();
        if (fadeMusic)
        {
            backgroundMusic.GetComponent<MusicLoop>().FadeOut();
        }
        if (hasTimeTransition == true)
        {
            timeTransition.ShowDay();
        }

        player.GetComponent<PlayerMovement>().UnfreezePlayer();
        levelController.GetComponent<GameController>().UnfreezeGame();
        this.gameObject.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void PressButton()
    {
        soundEffect.clip = buttonPress;
        soundEffect.Play();
    }

    //public void StartNewRoom()
    //{
    //    StartCoroutine(NewRoomState());
    //}

    //IEnumerator NewRoomState()
    //{
    //    newRoomName.text = "Riley's Home".Replace("Riley", playerName);
    //    newRoom.SetActive(true);
    //    yield return new WaitForSeconds(2);
    //    newRoom.SetActive(false);
    //}
}