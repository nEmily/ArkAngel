using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerDialogueManager : MonoBehaviour {

    public AudioClip startDialogueSound;
    public GameObject space;
    private Animator animator;
    float x;
    float y;
    private HashSet<GameObject> colliders = new HashSet<GameObject>();
    private AudioSource soundEffect;
    private GameObject player;

    private bool DialogueEnabled = true;

    // Use this for initialization
    void Start () {
        animator = gameObject.GetComponentInParent<Animator>();
        soundEffect = GetComponent<AudioSource>();
        player = this.gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (DialogueEnabled)
        {
            if (FindObjectOfType<DialogueRunner>().isDialogueRunning)
            {
                FindObjectOfType<PlayerMovement>().enabled = false;
                animator.SetInteger("x", 0);
                animator.SetInteger("y", 0);
            }
            else
            {
                FindObjectOfType<PlayerMovement>().enabled = true;
                if (space.activeSelf)
                {
                    space.SetActive(false);
                }
            }
            if (colliders.Count > 0 && Input.GetKeyDown(KeyCode.Space) && !FindObjectOfType<DialogueRunner>().isDialogueRunning)
            {
                GameObject other = FindPriorityCollider();
                if (other.GetComponent<InteractableObject>().IsNPC())
                {
                    StartCoroutine(TalkToNPC(other));
                }
                else
                {
                    Talk();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "InteractableObject")
        {
            colliders.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        colliders.Remove(other.gameObject);
    }

    private GameObject FindPriorityCollider()
    {
        int maxPriority = 0;
        GameObject priorityCollider = new GameObject() ;
        if (colliders.Count == 0)
        {
            throw new System.InvalidOperationException("Calling FindPriorityCollider() when there are no colliders.");
        }
        foreach (GameObject collider in colliders)
        {
            int colliderPriority = collider.GetComponent<InteractableObject>().priority;
            if (colliderPriority >= maxPriority)
            {
                priorityCollider = collider;
                maxPriority = colliderPriority;
            }
        }
        return priorityCollider;
    }

    void Talk()
    {
        DisableDialogue();
        GameObject other = FindPriorityCollider();
        InteractableObject target = other.GetComponent<InteractableObject>();

        soundEffect.clip = startDialogueSound;
        soundEffect.Play();
        GameObject.Find("Player").GetComponent<AudioSource>().Stop();
        if (target.isWindow)
        {
            space.SetActive(true);
        }
        FindObjectOfType<DialogueRunner>().StartDialogue(target.talkToNode);
        StartCoroutine(ResetAnimatorAfterDialogue(target));
        EnableDialogue();
    }

    IEnumerator ResetAnimatorAfterDialogue(InteractableObject target)
    {
        DialogueRunner dr = FindObjectOfType<DialogueRunner>();
        while (dr.isDialogueRunning)
        {
            yield return null;
        }
        target.FaceMe(0, 0);
        yield return null;
    }

    IEnumerator TalkToNPC(GameObject other)
    {
        DisableDialogue();

        Transform playerT = player.transform;
   
        x = playerT.position.x - other.transform.position.x;
        y = playerT.position.y - other.transform.position.y;

        Vector2 location;

        if (Mathf.Pow(x, 2) > Mathf.Pow(y, 2))
        {
            location = new Vector2(playerT.position.x,
                other.gameObject.transform.position.y - 0.1156f);
        }
        else
        {
            location = new Vector2(other.gameObject.transform.position.x,
                playerT.position.y);
        }

        InteractableObject target = other.GetComponent<InteractableObject>();
        PlayerMovement pMove = player.GetComponent<PlayerMovement>();

        yield return StartCoroutine(pMove.MovePlayer(location));

        if (Mathf.Pow(x, 2) > Mathf.Pow(y, 2))
        {
            if (x > 0)
            {
                target.FaceMe(1, 0);
                pMove.SetX(-1);
                pMove.SetY(0);
            }
            else
            {
                target.FaceMe(-1, 0);
                pMove.SetX(1);
                pMove.SetY(0);
            }
        }
        else
        {
            if (y > 0)
            {
                target.FaceMe(0, 1);
                pMove.SetX(0);
                pMove.SetY(-1);
            }
            else
            {
                target.FaceMe(0, -1);
                pMove.SetX(0);
                pMove.SetY(1);
            }
        }

        Talk();
    }

    public void DisableDialogue()
    {
        DialogueEnabled = false;
    }

    public void EnableDialogue()
    {
        DialogueEnabled = true;
    }
}
