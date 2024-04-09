using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour {

    public GameObject mom, athena, dad, johnson;
    public ColliderParent[] colliders;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveMom(Vector2 position, string direction)
    {
        mom.transform.position = position;
        string imgLocation = "Mom " + direction;
        Sprite sprite = Resources.Load<Sprite>(imgLocation);
        if (sprite != null)
        {
            Debug.Log("here!!!");
            mom.GetComponent<SpriteRenderer>().sprite = sprite;
        } else {
            Debug.Log("Couldn't find: " + imgLocation);
        }
        SetParent(mom);
    }

    public void TurnMom(string direction)
    {
        SetAnimatorParameters(mom.GetComponent<InteractableObject>(), direction);
    }

    public void MoveAthena(Vector2 position, string direction)
    {
        athena.transform.position = position;
        string imgLocation = "Athena " + direction;
        Sprite sprite = Resources.Load<Sprite>(imgLocation);
        if (sprite != null)
        {
            athena.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else
        {
            Debug.Log("Couldn't find: " + imgLocation);
        }
        SetParent(athena);
    }

    public void TurnAthena(string direction)
    {
        SetAnimatorParameters(athena.GetComponent<InteractableObject>(), direction);
    }

    public void MoveDad(Vector2 position, string direction)
    {
        dad.transform.position = position;
        string imgLocation = "Dad " + direction;
        Sprite sprite = Resources.Load<Sprite>(imgLocation);
        if (sprite != null)
        {
            dad.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else
        {
            Debug.Log("Couldn't find: " + imgLocation);
        }
        SetParent(dad);
    }

    public void TurnDad(string direction)
    {
        SetAnimatorParameters(dad.GetComponent<InteractableObject>(), direction);
    }

    public void MoveJohnson(Vector2 position, string direction)
    {
        johnson.transform.position = position;
        string imgLocation = "Old Man " + direction;
        Sprite sprite = Resources.Load<Sprite>(imgLocation);
        if (sprite != null)
        {
            johnson.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else
        {
            Debug.Log("Couldn't find: " + imgLocation);
        }
        SetParent(johnson);
    }

    public void TurnJohnson(string direction)
    {
        SetAnimatorParameters(johnson.GetComponent<InteractableObject>(), direction);
    }

    public Vector2 GetMomPos()
    {
        return mom.transform.position;
    }

    public Vector2 GetAthenaPos()
    {
        return athena.transform.position;
    }

    public Vector2 GetDadPos()
    {
        return dad.transform.position;
    }

    public Vector2 GetJohnsonPos()
    {
        return johnson.transform.position;
    }

    private void SetParent(GameObject npc)
    {
        Vector2 position = npc.transform.position;
        foreach (ColliderParent collider in colliders)
        {
            Bounds bounds = collider.GetComponent<BoxCollider2D>().bounds;
            if (bounds.center.x + bounds.extents.x >= position.x && bounds.center.x - bounds.extents.x <= position.x 
                && bounds.center.y + bounds.extents.y >= position.y && bounds.center.y - bounds.extents.y <= position.y)
            {
                npc.transform.parent = collider.parent.transform;
            }
        }
    }

    private void SetAnimatorParameters(InteractableObject target, string direction)
    {
        switch (direction) {
            case "Back": target.FaceMe(0, 1); break;
            case "Front": target.FaceMe(0, -1); break;
            case "Left": target.FaceMe(-1, 0); break;
            case "Right": target.FaceMe(1, 0); break;
            default: Debug.Log("unknown direction when calling setAnimatorParameters"); break;
        }
    }
}
