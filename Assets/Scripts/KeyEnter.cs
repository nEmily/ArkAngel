using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyEnter : MonoBehaviour {


    Button buttonMe;
    // Use this for initialization
    void Start()
    {
        buttonMe = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            buttonMe.onClick.Invoke();
        }
    }
}
