using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public GameObject dialogueBox;
    public Text nameText;
    public Text dialogueText;
    private Queue<string> sentences;

	// Use this for initialization
	void Start () {
        sentences = new Queue<string>();
	}
	
    public void StartDialogue(Dialogue dialogue)
    {
        nameText.text = dialogue.name;
        dialogueBox.SetActive(true);

        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        dialogue.initial = false;
        DisplayNextSentence();
    }

    public void RepeatDialogue(Dialogue dialogue)
    {
        nameText.text = dialogue.name;
        dialogueBox.SetActive(true);

        sentences.Clear();
        foreach (string sentence in dialogue.second)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                dialogueText.text = sentence;
                break;
            }
        }
        yield return StartCoroutine(WaitForKeyDown(KeyCode.Space));
        DisplayNextSentence();
    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
    }

    void EndDialogue()
    {
        StopAllCoroutines();
        dialogueBox.SetActive(false);
        FindObjectOfType<PlayerMovement>().enabled = true;
    }
}
