using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class GameController : MonoBehaviour {

    public GameObject loadingScreen;
    public Slider slider;

    public GameObject pauseButton;
    public GameObject pauseScreen;
    public GameObject pauseMenu;
    public GameObject inventoryScreen;
    public GameObject helpScreen;

    public GameObject playerInteract;
    private PlayerDialogueManager dialogueManager;
    private DialogueRunner dialogueRunner;
    public AudioClip buttonPress;

    private int items = 0;

    private AudioSource soundEffect;
    private bool isPaused;
    private string sceneName;

    public GameObject playerStatus;
    private GameObject player;

    private float speed = 1;

    [HideInInspector]
    public int currentLevelIndex = 1;

    private void Awake()
    {
        if (GameObject.Find("PlayerStatus") == null)
        {
            Instantiate(playerStatus);
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        Time.timeScale = speed;
        soundEffect = GetComponent<AudioSource>();
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "menu")
        {
            dialogueRunner = GameObject.Find("DialogueRunner").GetComponent<DialogueRunner>();
            dialogueManager = playerInteract.GetComponent<PlayerDialogueManager>();

            inventoryScreen.GetComponent<InventoryScript>().InitiateInv();
        }

    }

    private void Update()
    {
        if ((!sceneName.Equals("menu") && !dialogueRunner.isDialogueRunning && Input.GetKeyDown(KeyCode.P)) ||
            (!sceneName.Equals("menu") && !dialogueRunner.isDialogueRunning && isPaused && Input.GetKeyDown(KeyCode.Escape)))
        {
            Pause();
        } 
    }

    public void SaveGame()
    {
        Game.current = new global::Game();
        SaveData.Save();
    }

    public void ButtonSoundEffect()
    {
        soundEffect.clip = buttonPress;
        soundEffect.Play();
    }

    public void FreezeGame()
    {
        Time.timeScale = 0;
    }

    public void UnfreezeGame()
    {
        Time.timeScale = speed;

    }

    public void LoadMenu()
    {
        pauseScreen.SetActive(false);
        StartCoroutine(LoadAsynchronously(0));
        RemovePlayerDialogueManager();
    }

    public void LoadCredits()
    {
        pauseScreen.SetActive(false);
        StartCoroutine(LoadAsynchronously(3));
        RemovePlayerDialogueManager();
    }

    public void LoadLevel()
    {
        StartCoroutine(LoadAsynchronously(currentLevelIndex));
    }

    public void LoadLevel(int index)
    {
        StartCoroutine(LoadAsynchronously(index));
    }

    public void LevelUp()
    {
        currentLevelIndex++;
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slider.value = progress;

            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Pause()
    {
        soundEffect.clip = buttonPress;
        soundEffect.Play();
        isPaused = !pauseScreen.activeSelf;
        if (isPaused)
        {
            Time.timeScale = 0;
            player.GetComponent<PlayerMovement>().FreezePlayer();
            dialogueManager.DisableDialogue();
            pauseScreen.SetActive(true);
            pauseMenu.SetActive(true);
            pauseButton.SetActive(false);
        }
        else
        {
            Time.timeScale = speed;
            player.GetComponent<PlayerMovement>().UnfreezePlayer();
            dialogueManager.EnableDialogue();
            inventoryScreen.SetActive(false);
            helpScreen.SetActive(false);
            pauseScreen.SetActive(false);
            pauseButton.SetActive(true);
        }
    }

    private void RemovePlayerDialogueManager()
    {
        Destroy(FindObjectOfType<PlayerDialogueManager>());
    }

    public void ChangeSpeed(int speed)
    {
        this.speed = speed;
        Time.timeScale = speed;
    }
}
