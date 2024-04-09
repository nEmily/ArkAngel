using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerChanger : MonoBehaviour {

    private GameObject player;
    public double yOffset = 0;
    private Animator anim;


    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
		if (player.transform.position.y > this.gameObject.transform.position.y + yOffset)
        {
            if (anim != null)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 16;
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 15;
            }
        } else if (player.transform.position.y < this.gameObject.transform.position.y + yOffset)
        {
            if (anim != null)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
            }
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
            }
        }
	}
}
