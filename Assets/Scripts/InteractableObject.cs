using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InteractableObject : MonoBehaviour {

    public string talkToNode = "";
    public int priority = 0;
    public bool isWindow = false;
    public bool canTurn = true;
    private Animator anim;
    private bool isNPC;

    private GameObject player;
    private PlayerMovement playerMovement;
    private static bool startPushPull = false;
    private bool pushPull = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        isNPC = anim != null;
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (pushPull && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)))
        {
            EndPushPull();
        }
    }

    public void FaceMe(int x, int y)
    {
        if (isNPC && canTurn)
        {
            anim.SetInteger("x", x);
            anim.SetInteger("y", y);
        }
    }

    private bool IsHorizontal(GameObject other)
    {
        float x = this.gameObject.transform.position.x - other.transform.parent.position.x;
        float y = this.gameObject.transform.position.y - other.transform.parent.position.y;

        if (Mathf.Pow(x, 2) > Mathf.Pow(y, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isNPC && collision.gameObject.tag == "PlayerInteract" && startPushPull && !pushPull
            && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)))
        {
            StartCoroutine(SetPushPull());
            playerMovement.StartPushPull(this.gameObject, IsHorizontal(collision.gameObject));
        }
    }

    IEnumerator SetPushPull()
    {
        yield return null;
        yield return null;
        pushPull = true;
    }

    IEnumerator EndPushPullHelper()
    {
        yield return null;
        yield return null;
        pushPull = false;
    }

    void EndPushPull()
    {
        StartCoroutine(EndPushPullHelper());
        playerMovement.EndPushPull();
    }

    // Starts off the minigame, and enables the alt button as being able to pushpull
    public void BeginPushPullGame()
    {
        startPushPull = true;
    }

    public bool IsNPC()
    {
        return isNPC;
    }

    public void SetCanTurn(bool allowed)
    {
        canTurn = allowed;
    }
}
