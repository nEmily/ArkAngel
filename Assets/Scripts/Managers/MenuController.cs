using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MenuController : MonoBehaviour {

    public GameObject loadGameButton;
    public GameObject startNewGameScreen;
    public GameObject startButton;
    private AudioSource audioSource;

    private void Start()
    {
        startNewGameScreen.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    public void pressButton()
    {
        audioSource.Play();
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void LoadGame()
    {
        SaveData.Load();
        if (SaveData.savedGame != null)
        {
            // DO SOMETHING HERE TO LOAD DATA FROM THE SAVED FILE
        }
    }

    public void NewGame()
    {
        //SaveData.DeleteFile();
        startNewGameScreen.SetActive(true);
    }

    //public void OpenStartOptions()
    //{
    //    startButton.SetActive(false);
    //    startOptions.SetActive(true);
    //    if (!File.Exists(Application.persistentDataPath + "/arkangelsavefile.gd"))
    //    {
    //        loadGameButton.SetActive(false);
    //    }
    //}
}
