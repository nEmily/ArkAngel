using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public static bool acting = false;
    public GameObject from;
    public GameObject to;
    public GameObject toDoor;
    public GameObject changeRoomsScreen;
    public int x = 0;
    public int y = 0;
    private float space = 1.7f;

    void Action()
    {
        acting = true;
        FindObjectOfType<PlayerMovement>().enabled = false; // NEED TO RE-ENABLE PLAYER MOVEMENT SOMEHOW....BUT NOT IN COLLISION EXIT
        goToRoom();
    }

    public void goToRoom()
    {
        changeRoomsScreen.SetActive(true);
        from.SetActive(false);
        if (x != 0)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector2(toDoor.transform.position.x + space*x, toDoor.transform.position.y);
        } else
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector2(toDoor.transform.position.x + space * x, toDoor.transform.position.y + space * y);
        }
        // set player to this location, and re-enable movement
        to.SetActive(true);
        changeRoomsScreen.SetActive(false);
        FindObjectOfType<PlayerMovement>().enabled = true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D other = collision.collider;
        if (other.tag == "Player" && Input.GetKeyDown(KeyCode.Space))
        {
            Action();
        }

    }
}
