using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

    public string playerName = "Riley";

    /* Use the following to code pronouns:
     * $they : they/she/he, $them : them/her/him, $their : their/her/his,
     * $theirs: theirs/hers/his, $themself : themself/herself/himself */
    public int playerGender = 0; // 0 = nonbinary, 1 = girl, 2 = boy

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public string ReplaceName(string sentence)
    {
        //string upperCasedName = new string(playerName.Select(c => char.IsLetter(c) ? (char.IsUpper(c) ?
        //              char.ToLower(c) : char.ToUpper(c)) : c).ToArray());
        return sentence.Replace("Riley", playerName).Replace("RILEY", playerName.ToUpper());
    }

    /* Works for all lower-case, all upper-case, or title-case pronouns. */
    public string ReplaceGender(string sentence)
    {
        if (getGender() == 0)
        {
            return sentence.Replace("$", "");
        } else if (getGender() == 1)
        {
            sentence = sentence.Replace("$they", "she");
            sentence = sentence.Replace("$them", "her");
            sentence = sentence.Replace("$their", "her");
            sentence = sentence.Replace("$theirs", "hers");
            sentence = sentence.Replace("$themself", "herself");

            sentence = sentence.Replace("$they".ToUpper(), "she".ToUpper());
            sentence = sentence.Replace("$them".ToUpper(), "her".ToUpper());
            sentence = sentence.Replace("$their".ToUpper(), "her".ToUpper());
            sentence = sentence.Replace("$theirs".ToUpper(), "hers".ToUpper());
            sentence = sentence.Replace("$themself".ToUpper(), "herself".ToUpper());

            sentence = sentence.Replace("$They", "She");
            sentence = sentence.Replace("$Them", "Her");
            sentence = sentence.Replace("$Their", "Her");
            sentence = sentence.Replace("$Theirs", "Hers");
            sentence = sentence.Replace("$Themself", "Herself");

            return sentence;
        }
        else
        {
            sentence = sentence.Replace("$they", "he");
            sentence = sentence.Replace("$them", "him");
            sentence = sentence.Replace("$their", "his");
            sentence = sentence.Replace("$theirs", "his");
            sentence = sentence.Replace("$themself", "himself");

            sentence = sentence.Replace("$they".ToUpper(), "he".ToUpper());
            sentence = sentence.Replace("$them".ToUpper(), "him".ToUpper());
            sentence = sentence.Replace("$their".ToUpper(), "his".ToUpper());
            sentence = sentence.Replace("$theirs".ToUpper(), "his".ToUpper());
            sentence = sentence.Replace("$themself".ToUpper(), "himself".ToUpper());

            sentence = sentence.Replace("$They", "He");
            sentence = sentence.Replace("$Them", "Him");
            sentence = sentence.Replace("$Their", "His");
            sentence = sentence.Replace("$Theirs", "His");
            sentence = sentence.Replace("$Themself", "Himself");

            return sentence;
        }
        
        
    }

    public void setName(string n)
    {
        playerName = n;
        if (n.Equals("") || n.Equals(" "))
        {
            playerName = "Riley";
        }
    }

    public void setGender(int num)
    {
        playerGender = num;
    }

    public string getName()
    {
        return playerName;
    }

    public int getGender()
    {
        return playerGender;
    }
}
