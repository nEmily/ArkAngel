using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

    public string name;
    public bool initial = true;

    [TextArea(3, 10)]
    public string[] sentences;

    [TextArea(3, 10)]
    public string[] second;

    void updateSentences(string[] _sentences)
    {
        sentences = _sentences;
    }

    void updateSecond(string[] _sentences)
    {
        second = _sentences;
    }
}
