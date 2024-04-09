using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class Minigame2 : MonoBehaviour {

    public GameObject levelController;
    public GameObject doorStop;
    public GameObject player;
    public GameObject variableStorage;
    public GameObject npcMovement;
    private PlayerDialogueManager playerDialogue;

    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip winSound;
    private AudioSource soundEffects;

    public AudioSource backgroundMusic;
    public AudioClip musicClip;

    public bool playGame = false;
    public GameObject completeScreen;
    public GameObject loseScreen;
    public GameObject skipScreen;
    public GameObject wrongMoveScreen;

    public GameObject buttonTutorial;
    public GameObject pushTutorial;

    public List<GameObject> activateItems;
    public List<GameObject> deactivateItemsForTutorial;

    public GameObject allCommands;
    public Image[] commands;
    public Sprite[] images;
    public Color pressedSeqColor;
    public Color unpressedSeqColor;

    public GameObject timerSliderObject;
    private Slider timerSlider;
    static float timeLimit = 2.7f;
    float timer = timeLimit;
    float timeBetweenRounds = 0.3f;

    int[] sequence;

    int stepInSeq = 0;
    int stepInRounds = 0;
    int lives = 5;
    private bool firstGame;

    public int numSets = 3;
    public int numRounds = 15;

    public GameObject[] npcs;
    public GameObject[] npcImages;
    public ComputerBounds[] computerVectors;
    public GameObject[] computers; // left, center, right in this order

    private bool[] sets;
    int setIndex;
    private int failedGames;

    // Use this for initialization
    void Start () {
        StartCoroutine(PlayMusic());
        doorStop.SetActive(true);

        //player.GetComponent<PlayerMovement>().FreezePlayer();
        //player.SetActive(false);

        playerDialogue = GameObject.FindGameObjectWithTag("PlayerInteract").GetComponent<PlayerDialogueManager>();
        playerDialogue.DisableDialogue();

        timerSlider = timerSliderObject.GetComponent<Slider>();
        timerSlider.maxValue = timeLimit;
        timerSlider.value = timeLimit;

        sets = new bool[] { false, false, false };
        GameObject.Find("Athena").GetComponent<InteractableObject>().BeginPushPullGame();

        firstGame = true;
        failedGames = 0;

        // be sure to have ONLY the initial tutorial active in start
    }

    IEnumerator PlayMusic()
    {
        soundEffects = levelController.GetComponent<AudioSource>();
        yield return StartCoroutine(backgroundMusic.GetComponent<MusicLoop>().FadeOutIEnumerator());

        backgroundMusic.clip = musicClip;
    }

    // Start game after player closes pushTutorial
    public void SetUpGame()
    {
        backgroundMusic.GetComponent<MusicLoop>().FadeIn();
        backgroundMusic.Play();
        pushTutorial.SetActive(false);
        playerDialogue.EnableDialogue();
        foreach (GameObject item in activateItems)
        {
            item.SetActive(false);
        }
    }

    // Start the button pressing game after NPCs are in place
    public void InitiateGame(int index)
    {
        playerDialogue.DisableDialogue();
        setIndex = index;
        if (firstGame)
        {
            firstGame = false;
            buttonTutorial.SetActive(true);
        } else
        {
            OpenGame();
        }
    }

    public void OpenGame()
    {
        pushTutorial.SetActive(false);
        buttonTutorial.SetActive(false);

        foreach (GameObject item in activateItems)
        {
            item.SetActive(true);
        }
        npcImages[setIndex].SetActive(true);

        RandomizeCommands(Mathf.Clamp(stepInRounds + 2, 3, 5));
        PlayGame();
    }

    public void PlayGame()
    {
        playGame = true;
    }

    public void PauseGame()
    {
        playGame = false;
    }

    public void ResetGame()
    {
        numRounds = 13;
        stepInSeq = 0;
        stepInRounds = 0;
        lives = 5;
        sets = new bool[] { false, false, false };
        playGame = false;
        failedGames++;

        NPCMovement npc = npcMovement.GetComponent<NPCMovement>();

        npc.MoveJohnson(new Vector2(0.4f, 14.6f), "Left");
        npc.MoveDad(new Vector2(-5.21f, 13.18f), "Front");
        npc.MoveMom(new Vector2(-3f, 9.36f), "Right");
        npc.MoveAthena(new Vector2(4.85f, 10.02f), "Left");

        ExampleVariableStorage2 vs = variableStorage.GetComponent<ExampleVariableStorage2>();
        vs.SetValue("$start_left_minigame", new Yarn.Value(false));
        vs.SetValue("$start_center_minigame", new Yarn.Value(false));
        vs.SetValue("$start_right_minigame", new Yarn.Value(false));

        vs.SetValue("$npc_at_left", new Yarn.Value(false));
        vs.SetValue("$npc_at_center", new Yarn.Value(false));
        vs.SetValue("$npc_at_right", new Yarn.Value(false));

        vs.right = false;
        vs.center = false;
        vs.left = false;

        loseScreen.SetActive(false);
        skipScreen.SetActive(false);
        SetUpGame();
    }

    // Update is called once per frame. Left = 0, Up = 1, Right = 2, Down = 3, Space = 4
    void Update () {
        if (playGame)
        {
            int move = -1;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                move = 0;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                move = 1;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                move = 2;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                move = 3;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                move = 4;
            }

            timer -= Time.fixedDeltaTime;
            timerSlider.value = timer;
            if (timer <= 0)
            {
                move = 5;
            }

            CheckMove(move);

            if (lives <= 0)
            {
                LoseGame();
            }

        } else
        {
            for (int i = 0; i < 3; i++)
            {
                Bounds personBound = npcs[i].GetComponent<BoxCollider2D>().bounds;
                Vector2 position = npcs[i].GetComponent<Transform>().position;
                //if (computerVectors[i].right.x >= personBound.center.x && computerVectors[i].left.x <= personBound.center.x && computerVectors[i].right.y >= personBound.center.y && computerVectors[i].left.y <= personBound.center.y)
                //{
                //    Debug.Log("Person " + i);
                //    variableStorage.GetComponent<ExampleVariableStorage2>().SetNPCAtComp(i);
                //    break;
                //}
                //Debug.Log("set " + i + ":" + sets[i]);
                if (!sets[i] && computerVectors[i].right.x >= position.x && computerVectors[i].left.x <= position.x 
                    && computerVectors[i].right.y >= position.y && computerVectors[i].left.y <= position.y)
                {
                    //Debug.Log("Person " + i);
                    variableStorage.GetComponent<ExampleVariableStorage2>().SetNPCAtComp(i);
                    break;
                }
                //if (i == 2)
                //{
                //    Debug.Log("Bottom Left: " + computerVectors[i].left.ToString());
                //    Debug.Log("Top Right: " + computerVectors[i].right.ToString());
                //    Debug.Log("Person location: " + personBound.center.x.ToString() + ", " + personBound.center.y.ToString());
                //    Debug.Log("Person location2: " + position.x.ToString() + ", " + position.y.ToString());
                //}
            }
        }
    }

    public void WinGame()
    {
        skipScreen.SetActive(false);
        playGame = false;
        foreach (GameObject item in deactivateItemsForTutorial)
        {
            item.SetActive(false);
        }
        FindObjectOfType<EndSequence>().EndStuff();
        this.gameObject.SetActive(false);
        playerDialogue.EnableDialogue();
        //completeScreen.SetActive(true);
    }

    void LoseGame()
    {
        playGame = false;
        foreach (GameObject item in deactivateItemsForTutorial)
        {
            item.SetActive(false);
        }
        if (failedGames >= 3)
        {
            skipScreen.SetActive(true);
        } else
        {
            loseScreen.SetActive(true);
        }
    }

    // Check that a move is correct. Remember that a -1 move doesn't do anything. 5 move = timer's up
    void CheckMove(int move)
    {
        if (move == sequence[stepInSeq])
        {
            PlayCorrectMoveSound();
            commands[stepInSeq].color = pressedSeqColor;
            stepInSeq++;
            if (stepInSeq >= sequence.Length)
            {
                StartCoroutine(CompleteRound());
            }
        } else if (move != -1)
        {
            stepInRounds--;
            StartCoroutine(CompleteRound());
            StartCoroutine(WrongMove());
        }
    }

    // After finishing a round, start a new round, start a new set, or end game.
    IEnumerator CompleteRound()
    {
        PauseGame();
        allCommands.SetActive(false);
        stepInRounds++;

        if (stepInRounds < numRounds) // // // TODO : INCREASE # OF ROUNDS WITH EACH SET // // //
        {
            stepInSeq = 0;
            yield return new WaitForSecondsRealtime(timeBetweenRounds);

            RandomizeCommands(Mathf.Clamp(stepInRounds - 1, 3, 5));
            allCommands.SetActive(true);
            PlayGame();
        }
        else
        {
            CompleteSet();
        }
    }

    // After finishing a set, start a new set or end game.
    void CompleteSet()
    {
        sets[setIndex] = true;

        if (sets.All(x => x))
        {
            WinGame();
        } else
        {
            stepInSeq = 0;
            stepInRounds = 0;
            lives = 5;

            npcImages[setIndex].SetActive(false);
            foreach (GameObject item in activateItems)
            {
                item.SetActive(false);
            }

            npcs[setIndex].GetComponent<Animator>().SetInteger("x", 0);
            npcs[setIndex].GetComponent<Animator>().SetInteger("y", -1);

            playerDialogue.EnableDialogue();
            PauseGame();
        }
    }

    // Select different commands and change images
    void RandomizeCommands(int num)
    {
        int[] newSeq = new int[num];
        foreach (Image command in commands)
        {
            command.gameObject.SetActive(false);
        }
        for (int i = 0; i < num; i ++)
        {
            int command = Random.Range(0, num);
            newSeq[i] = command;
            commands[i].sprite = images[command];
            commands[i].color = unpressedSeqColor;
            commands[i].gameObject.SetActive(true);
        }

        if (sequence != null && sequence.SequenceEqual(newSeq))
        {
            RandomizeCommands(num);
        } else
        {
            sequence = newSeq;
            allCommands.SetActive(true);
        }

        SetTimer();
    }

    void SetTimer()
    {
        timer = timeLimit;
    }

    IEnumerator WrongMove()
    {
        PlayIncorrectMoveSound();
        numRounds++;
        lives--;

        wrongMoveScreen.SetActive(true);
        yield return null;
        yield return null;
        wrongMoveScreen.SetActive(false);
    }

    void PlayCorrectMoveSound()
    {
        soundEffects.clip = correctSound;
        soundEffects.Play();
    }

    void PlayIncorrectMoveSound()
    {
        soundEffects.clip = incorrectSound;
        soundEffects.Play();
    }

    void PlayCompleteRoundSound()
    {
        soundEffects.clip = correctSound;
        soundEffects.Play();
    }

    void PlayWinGameSound()
    {
        soundEffects.clip = winSound;
        soundEffects.Play();
    }

    [System.Serializable]
    public class ComputerBounds
    {
        public string name;
        public Vector2 left;
        public Vector2 right;
    }
}
