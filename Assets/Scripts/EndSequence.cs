using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class EndSequence : MonoBehaviour {
    public GameObject blackScreen;
    public GameObject[] lightings;
    public GameObject spotLight;
    public PlayerMovement playerMovement;
    public Collider2D centerLocation;
    public GameObject oldMan;
    public InteractableObject mom, dad, athena, johnson;
    public Text text;
    public GameObject textObject;
    public GameController levelController;
    public GameObject doorStop;
    public string sadEpilogue, happyEpilogue;

    public GameObject backgroundMusic;
    public AudioClip lastSong;

    private bool canEnter = false;
    private bool oldManCanMove = false;
    private bool done = false;
    private Vector2 playerFinalPosition, oldManFinalPosition;
	// Use this for initialization
	void Start () {
        playerFinalPosition = new Vector2(centerLocation.bounds.center.x, centerLocation.bounds.center.y);
        oldManFinalPosition = new Vector2(centerLocation.bounds.center.x, centerLocation.bounds.center.y - 0.8f);
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 playerPos = playerMovement.transform.position;
        if (!oldManCanMove && playerPos == playerFinalPosition)
        {
            FindObjectOfType<PlayerMovement>().FreezePlayer();
            oldManCanMove = true;
            playerMovement.SetX(0);
            playerMovement.SetY(1);
        }
		if (!done && oldManCanMove)
        {
            Vector2 oldManPosition = oldMan.transform.position;
            oldMan.SetActive(true);
            Debug.Log("old man now pos: " + oldManPosition);
            Debug.Log("old man final pos: " + oldManFinalPosition);
            if (oldManPosition != oldManFinalPosition)
            {
                oldMan.transform.position = Vector2.MoveTowards(oldManPosition, oldManFinalPosition, 0.5f * Time.deltaTime);
            }
            else
            {
                oldMan.GetComponent<Animator>().SetBool("Stop", true);
                done = true;
                FindObjectOfType<DialogueRunner>().StartDialogue("Old Man");
            }
        }
	}

    public void EndStuff()
    {
        backgroundMusic.GetComponent<AudioSource>().Stop();
        backgroundMusic.GetComponent<AudioSource>().clip = lastSong;
        backgroundMusic.GetComponent<AudioSource>().Play();

        StartCoroutine(Dialogue1());
    }

    public void Flickering()
    {
        StartCoroutine(FlickeringCoroutine());
    }

    IEnumerator FlickeringCoroutine()
    {
        float[] pauseTimes = { 0.2f, 2.0f, 0.25f, 0.5f, 0.25f, 0.7f, 0.5f, 0.1f, 0.3f, 0.2f, 0.1f, 0.2f, 0.1f, 0.05f, 0.02f, 0.05f, 0.02f, 0.05f, 0.1f, 0.05f, 0.02f };
        for (int i = 0; i < pauseTimes.Length; i++)
        {
            blackScreen.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            blackScreen.SetActive(false);
            yield return new WaitForSecondsRealtime(pauseTimes[i]);
            Debug.Log(i);
        }
        blackScreen.SetActive(true);
        mom.SetCanTurn(false);
        dad.SetCanTurn(false);
        athena.SetCanTurn(false);
        johnson.SetCanTurn(false);
        doorStop.SetActive(true);
        yield return new WaitForSecondsRealtime(5.0f);
        yield return FadeIn(4.0f);
        FindObjectOfType<ExampleVariableStorage2>().SetDay5True();
    }

    IEnumerator FadeIn(float seconds)
    {
        float t = 0.0f;
        Color color = blackScreen.GetComponent<Image>().color;
        while (t < 1.0f)
        {
            color.a = Mathf.Lerp(1.0f, 0.4f, t);
            blackScreen.GetComponent<Image>().color = color;
            t = t + 0.01f;
            yield return new WaitForSecondsRealtime(seconds / 100);
        }
        yield return null;
    }

    IEnumerator FadeOut(float seconds)
    {
        float t = 0.0f;
        Color color = blackScreen.GetComponent<Image>().color;
        while (t < 1.0f)
        {
            color.a = Mathf.Lerp(0.4f, 1.0f, t);
            blackScreen.GetComponent<Image>().color = color;
            t = t + 0.01f;
            yield return new WaitForSecondsRealtime(seconds / 100);
        }
        yield return null;
    }

    IEnumerator Dialogue1()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        FindObjectOfType<DialogueRunner>().StartDialogue("FinalDialogue1");
        while (FindObjectOfType<DialogueRunner>().isDialogueRunning)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public void StartGrandFinale()
    {
        StartCoroutine(DimLights());
    }

    IEnumerator DimLights()
    {
        for (int i = 0; i < lightings.Length; i++)
        {
            lightings[i].SetActive(true);
            yield return new WaitForSecondsRealtime(1.0f);
        }
        yield return new WaitForSecondsRealtime(1.0f);
        spotLight.SetActive(true);
        lightings[2].SetActive(false);
        canEnter = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canEnter && other.tag == "Player")
        {
            Debug.Log("player final pos: " + centerLocation.bounds.center);
            Debug.Log("player now pos: " + playerMovement.transform.position);
            playerMovement.ChangeSpeed(3.0f);
            StartCoroutine(playerMovement.MovePlayer(centerLocation.bounds.center));
            playerMovement.SetCanMove(false);
        }
    }

    public IEnumerator SadCredits()
    {
        StartCoroutine(FadeOut(6.0f));
        yield return new WaitForSecondsRealtime(6.0f);
        StartCoroutine(TypeSentence(sadEpilogue));
    }

    public IEnumerator HappyCredits()
    {
        StartCoroutine(FadeOut(6.0f));
        yield return new WaitForSecondsRealtime(6.0f);
        StartCoroutine(TypeSentence(happyEpilogue));
    }

    IEnumerator TypeSentence(string sentence)
    {
        text.text = "";
        textObject.SetActive(true);

        string playerName = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<PlayerStatus>().getName();

        if (playerName != null && !playerName.Equals("") && !playerName.Equals(" "))
        {
            sentence = sentence.Replace("Riley", playerName);
            sentence = sentence.Replace("RILEY", playerName.ToUpper());
        }

        foreach (char letter in sentence.ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSecondsRealtime(.025f);
        }
        yield return StartCoroutine(WaitForKeyDown());
    }

    IEnumerator WaitForKeyDown()
    {
        while (!Input.anyKeyDown)
            yield return null;
        levelController.LoadCredits();
    }
}
