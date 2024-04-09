using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class TimeTransition : MonoBehaviour {

    public GameObject textBox;
    public Text text;
    public string expo;
    public AudioClip buttonPress;
    public GameObject levelController;
    public Transform wakeUpLocation;
    public GameObject rileyRoom;
    public GameObject housingHall;

    private GameObject player;
    private string playerName;

    private AudioSource soundEffect;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        soundEffect = levelController.GetComponent<AudioSource>();
    }

    public void ShowDay()
    {
        Color color = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1f);
        this.gameObject.SetActive(true);
        textBox.SetActive(true);
        levelController.GetComponent<GameController>().FreezeGame();
        StartCoroutine(TypeSentence("Day 1", false));
    }


    public void StartTimeTransition(int day)
    {
        this.gameObject.SetActive(true);
        Debug.Log("here1");
        Debug.Log(player);
        player.GetComponentInChildren<PlayerDialogueManager>().enabled = false;
        StartCoroutine(FadeIn(2.0f));
        Debug.Log("here2");
        player.GetComponent<PlayerMovement>().FreezePlayer();
        Debug.Log("Time transtion day: " + day);
        StartCoroutine(TypeSentence(expo + " " + day, true));
    }

    IEnumerator FadeIn(float seconds)
    {
        float t = 0.0f;
        Color color = GetComponent<Image>().color;
        while (t < 1.0f)
        {
            color.a = Mathf.Lerp(0.0f, 1.0f, t);
            GetComponent<Image>().color = color;
            t = t + seconds / 100;
            yield return new WaitForSecondsRealtime(seconds / 100);
        }
        textBox.SetActive(true);
        yield return null;
    }

    IEnumerator TypeSentence(string sentence, bool hasFadeIn)
    {
        text.text = "";

        playerName = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<PlayerStatus>().getName();

        if (playerName != null && !playerName.Equals("") && !playerName.Equals(" "))
        {
            sentence = sentence.Replace("Riley", playerName);
            sentence = sentence.Replace("RILEY", playerName.ToUpper());
        }

        if (hasFadeIn)
        {
            yield return new WaitForSecondsRealtime(2.0f);
        }

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
        EndIntro();
        
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

    public void EndIntro()
    {
        player.transform.position = wakeUpLocation.position;
        Debug.Log("Ending intro");
        FindObjectOfType<DialogueRunner>().StartDialogue("WakeUp");
        player.GetComponentInChildren<PlayerDialogueManager>().enabled = true;
        player.GetComponent<PlayerMovement>().UnfreezePlayer();
        textBox.SetActive(false);
        this.gameObject.SetActive(false);
        rileyRoom.SetActive(true);
        housingHall.SetActive(false);
        StopAllCoroutines();
        Debug.Log(player.transform.position);
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