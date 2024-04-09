using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour {

    private static string secret = "143124";
    private static string sequence = "";
    public Space space;

    public void AddToSequence(string doorCode)
    {
        sequence = sequence + doorCode;
        Debug.Log(sequence);
        if (sequence == secret)
        {
            sequence = "";
            space.StartSpaceScene();
        }
        else if (sequence.Length >= secret.Length || sequence != secret.Substring(0, sequence.Length))
        {
            sequence = "";
        }
    }
}
