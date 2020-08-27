using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 6f;
    private Animator anim;
    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 movement;

        if (h * h > v * v)
        {
            
            anim.SetInteger("Movement", (int) Mathf.Round(h));
            anim.SetBool("Horizontal", true);
            anim.SetBool("Vertical", false);

            movement = new Vector2(h, 0);
        }
        else
        {
            anim.SetInteger("Movement", (int)Mathf.Round(v));
            anim.SetBool("Horizontal", false);
            anim.SetBool("Vertical", true);

            movement = new Vector2(0, v);
        }

        rb.velocity = movement * speed;
    }
}