using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {

    public Dialogue dialogue;
    public static bool acting = false;

    void Action()
    {
        acting = true;
        FindObjectOfType<PlayerMovement>().enabled = false; // NEED TO RE-ENABLE PLAYER MOVEMENT SOMEHOW....BUT NOT IN COLLISION EXIT
        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (dialogue.initial)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        } else
        {
            FindObjectOfType<DialogueManager>().RepeatDialogue(dialogue);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!acting)
        {
            Collider2D other = collision.collider;
            if (other.tag == "Player" && Input.GetKeyDown(KeyCode.Space))
            {
                Action();
            }
        }
    }
}
