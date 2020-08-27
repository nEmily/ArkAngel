using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject loadingScreen;
    public Slider slider;
    public GameObject pauseScreen;
    private bool isPaused;
    private string sceneName;

    [HideInInspector]
    public int currentLevelIndex = 1;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "menu")
        {
            pauseScreen.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !sceneName.Equals("menu"))
        {
            Pause();
        }
    }

    public void SaveGame()
    {
        Game.current = new global::Game();
        SaveData.Save();
    }

    void Pause()
    {
        isPaused = !pauseScreen.activeSelf;
        if (isPaused)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
        } else
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadAsynchronously(0));
    }

    public void LoadLevel()
    {
        StartCoroutine(LoadAsynchronously(currentLevelIndex));
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
}
