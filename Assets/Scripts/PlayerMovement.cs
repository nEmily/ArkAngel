using UnityEngine;
using System.Collections;
using System.Text;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 6f;
    public GameObject interact;
    public AudioClip walkSound;

    private bool playerCanMove = true;
    private bool movePlayer = false;

    private bool pushPull = false;
    private bool horizontal;
    private GameObject pushTarget;
    private Rigidbody2D pushTargetRB;


    private Vector2 newLocation;
    private Animator anim;
    private Rigidbody2D playerRB;
    private ArrayList movementStack = new ArrayList();
    private AudioSource soundEffect;

    private float originalSpeed;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        soundEffect = GetComponent<AudioSource>();
        anim.speed = speed / 3;
        originalSpeed = speed;
    }

    void Update()
    {
        if (movePlayer && Input.GetKeyDown(KeyCode.Escape))
        {
            movePlayer = false;
            playerCanMove = true;
        }

        if (playerCanMove)
        {
            if (HorizontalKeyDown())
            {
                movementStack.Reverse();
                movementStack.Add(0);
                movementStack.Reverse();
                PlayWalkSound();
            }
            if (VerticalKeyDown())
            {
                movementStack.Reverse();
                movementStack.Add(1);
                movementStack.Reverse();
                PlayWalkSound();
            }
            if (HorizontalKeyUp())
            {
                movementStack.Reverse();
                movementStack.Remove(0);
                movementStack.Reverse();
            }
            if (VerticalKeyUp())
            {
                movementStack.Reverse();
                movementStack.Remove(1);
                movementStack.Reverse();
            }
            if (!HorizontalKeyPressed() && !VerticalKeyPressed())
            {
                movementStack.Clear();
                StopWalkSound();
            }
            var stringBuilder = new StringBuilder();
            foreach (var item in movementStack)
            {
                stringBuilder.Append(item.ToString());
            }
        }
        else if (!playerCanMove && movePlayer)
        {
            movementStack.Clear();
            PlayWalkSound();

            if (this.transform.position.x != newLocation.x)
            {
                SetX(newLocation.x - this.transform.position.x);
                SetY(0);
                this.transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(newLocation.x, this.transform.position.y), (speed - 1.5f) * Time.deltaTime);
            } else if (this.transform.position.y != newLocation.y)
            {
                SetX(0);
                SetY(newLocation.y - this.transform.position.y);
                this.transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(this.transform.position.x, newLocation.y), (speed - 1.5f) * Time.deltaTime);
            } else if (this.transform.position.x == newLocation.x && this.transform.position.y == newLocation.y)
            {
                if (pushTarget != null) // set the player to be facing its pushPull target
                {
                    if (horizontal)
                    {
                        int x = (pushTarget.transform.position.x - this.gameObject.transform.position.x) > 0 ? 1 : -1;
                        SetX(x);
                        SetY(0);
                        pushTarget.GetComponent<InteractableObject>().FaceMe(-x, 0);
                    }
                    else
                    {
                        int y = (pushTarget.transform.position.y - this.gameObject.transform.position.y) > 0 ? 1 : -1;
                        SetX(0);
                        SetY(y);
                        pushTarget.GetComponent<InteractableObject>().FaceMe(0, -y);
                    }
                }

                StopWalkSound();
                playerCanMove = true;
                movePlayer = false;
            } // else nothing happens
        }
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 movement;

        movement = new Vector2(0, 0);

        if (!pushPull)
        {
            if (movementStack.Count != 0 && movementStack[0].Equals(0))
            {
                SetX(x);
                anim.SetInteger("y", 0);
                movement = new Vector2(x, 0);

                if (x > 0)
                {
                    interact.transform.position = new Vector2(transform.position.x + .3f, transform.position.y - .1f);
                    interact.transform.eulerAngles = new Vector3(0, 0, 90);
                }
                else if (x < 0)
                {
                    interact.transform.position = new Vector2(transform.position.x - .3f, transform.position.y - .1f);
                    interact.transform.eulerAngles = new Vector3(0, 0, -90);
                }
            }
            else if (movementStack.Count != 0 && movementStack[0].Equals(1))
            {
                SetY(y);
                anim.SetInteger("x", 0);
                movement = new Vector2(0, y);

                if (y > 0)
                {
                    interact.transform.position = new Vector2(transform.position.x, transform.position.y);
                }
                else if (y < 0)
                {
                    interact.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
                }

                interact.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (movementStack.Count == 0)
            {
                anim.SetInteger("x", 0);
                anim.SetInteger("y", 0);
            }

            playerRB.velocity = movement * speed;

        } else if (playerCanMove && pushPull) // implementing the pushPull mechanics here
        {
            if (movementStack.Count != 0 && movementStack[0].Equals(0) && horizontal)
            {
                movement = new Vector2(x, 0);
                SetX(x);
                SetY(0);

                if (x > 0)
                {
                    interact.transform.position = new Vector2(transform.position.x + .3f, transform.position.y - .1f);
                    interact.transform.eulerAngles = new Vector3(0, 0, 90);
                    pushTarget.GetComponent<InteractableObject>().FaceMe(1, 0);
                }
                else if (x < 0)
                {
                    interact.transform.position = new Vector2(transform.position.x - .3f, transform.position.y - .1f);
                    interact.transform.eulerAngles = new Vector3(0, 0, -90);
                    pushTarget.GetComponent<InteractableObject>().FaceMe(-1, 0);
                }
            }
            else if (movementStack.Count != 0 && movementStack[0].Equals(1) && !horizontal) // this is vertical
            {
                movement = new Vector2(0, y);
                SetX(0);
                SetY(y);

                if (y > 0)
                {
                    interact.transform.position = new Vector2(transform.position.x + .3f, transform.position.y - .1f);
                    interact.transform.eulerAngles = new Vector3(0, 0, 90);
                    pushTarget.GetComponent<InteractableObject>().FaceMe(0, 1);
                }
                else if (y < 0)
                {
                    interact.transform.position = new Vector2(transform.position.x - .3f, transform.position.y - .1f);
                    interact.transform.eulerAngles = new Vector3(0, 0, -90);
                    pushTarget.GetComponent<InteractableObject>().FaceMe(0, -1);
                }
            }
            else if (movementStack.Count == 0)
            {
                movement = new Vector2(0, 0);
                anim.SetInteger("x", 0);
                anim.SetInteger("y", 0);
            }

            playerRB.velocity = movement * speed;
            pushTargetRB.velocity = movement * speed;
        }
    }

    public void SetX(float x)
    {
        if (x > 0)
        {
            anim.SetInteger("x", 1);
        }
        else if (x < 0)
        {
            anim.SetInteger("x", -1);
        }
        else
        {
            anim.SetInteger("x", 0);
        }
    }

    public void SetY(float y)
    {
        if (y > 0)
        {
            anim.SetInteger("y", 1);
        }
        else if (y < 0)
        {
            anim.SetInteger("y", -1);
        }
        else
        {
            anim.SetInteger("y", 0);
        }
    }

    private bool HorizontalKeyDown()
    {
        return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow);
    }

    private bool VerticalKeyDown()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow);
    }

    private bool HorizontalKeyUp()
    {
        return Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow);
    }

    private bool VerticalKeyUp()
    {
        return Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow);
    }

    private bool HorizontalKeyPressed()
    {
        return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow);
    }

    private bool VerticalKeyPressed()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
    }

    private void PlayWalkSound()
    {
        if (soundEffect.isPlaying)
        {
            return;
        }
        soundEffect.clip = walkSound;
        soundEffect.Play();
    }
    
    private void StopWalkSound()
    {
        if (soundEffect.isPlaying)
        {
            soundEffect.Stop();
        }
    }

    public IEnumerator MovePlayer(Vector2 movement)
    {
        newLocation = movement;
        playerCanMove = false;
        movePlayer = true;
        while (movePlayer)
        {
            yield return null;
        }
    }

    public void FreezePlayer()
    {
        movePlayer = false;
        playerCanMove = false;
        speed = 0;
    }

    public void UnfreezePlayer()
    {
        playerCanMove = true;
        speed = originalSpeed;
    }

    public void StartPushPull(GameObject target, bool hor)
    //{
    //if (pushPull)
    //{
    //    EndPushPull();
    //} else
    {
        horizontal = hor;

        if (horizontal)
        {
            StartCoroutine(MovePlayer(new Vector2(this.gameObject.transform.position.x,
                target.gameObject.transform.position.y - 0.1156f)));
        }
        else
        {
            StartCoroutine(MovePlayer(new Vector2(target.gameObject.transform.position.x,
                this.gameObject.transform.position.y)));
        }

        pushPull = true;
        pushTarget = target;

        pushTargetRB = target.GetComponent<Rigidbody2D>();
        pushTargetRB.bodyType = RigidbodyType2D.Dynamic;
        pushTargetRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        pushTargetRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        //}
    }

    public void EndPushPull()
    {
        pushTargetRB.bodyType = RigidbodyType2D.Static;
        pushPull = false;
        pushTarget = null;
        pushTargetRB = null;
    }

    public void ChangeSpeed(float speed)
    {
        this.originalSpeed = speed;
        this.speed = speed;
        this.anim.speed = speed / 3;
    }

    public void SetCanMove(bool allowed)
    {
        playerCanMove = allowed;
    }
}