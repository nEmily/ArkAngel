using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;


public class Teleporter : MonoBehaviour
{

    public GameObject from;
    public GameObject to;
    public GameObject toTeleporter;

    public GameObject changeRoomsScreen;

    public GameObject newRoom;
    public Text newRoomName;

    public GameObject levelController;
    public AudioClip teleportClip;

    public int x = 0;
    public int y = 0;

    private GameObject player;
    private GameObject playerStatus;
    private string playerName;

    private Animator anim;
    private PlayerMovement movement;
    private AudioSource teleportAudio;

    private Vector2 location;
    private bool entering = true;

    private void Awake()
    {
        teleportAudio = levelController.GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
        anim = player.GetComponent<Animator>();
        location = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

 
        playerStatus = GameObject.FindGameObjectWithTag("PlayerStatus");
        playerName = playerStatus.GetComponent<PlayerStatus>().getName();
    }

    void Action()
    {
        StartCoroutine(GoToRoom());
    }

    /* This function is to be used by the starting teleporter. */
    IEnumerator GoToRoom()
    {
        yield return StartCoroutine(movement.MovePlayer(location));
        movement.FreezePlayer();
        
        teleportAudio.clip = teleportClip;
        teleportAudio.volume += 0.2f;
        teleportAudio.Play();

        anim.SetBool("Teleporting", true);
        yield return new WaitForSecondsRealtime(.7f);

        changeRoomsScreen.SetActive(true);
        to.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);

        toTeleporter.GetComponent<Teleporter>().End();
        from.SetActive(false);
    }

    /* This function is to be used by the target teleporter. */
    public void End()
    {
        StartCoroutine(EndInRoom());
    }

    /* This function is to be used by the target teleporter. */
    IEnumerator EndInRoom()
    {
        changeRoomsScreen.SetActive(false);
        StartNewRoom();

        player.transform.position = location;
        teleportAudio.Play();
        teleportAudio.volume -= 0.2f;
        anim.SetBool("Teleporting", false);
        yield return new WaitForSecondsRealtime(0.7f);
        movement.UnfreezePlayer();
    }

    /* This function is to be used by the target teleporter. */
    public void StartNewRoom()
    {
        StartCoroutine(NewRoomState());
    }

    IEnumerator NewRoomState()
    {
        if (newRoomName.text.Contains("Riley") && playerName != null)
        {
            newRoomName.text = SplitString(from.name).Replace("Riley", playerName);
        }
        else
        {
            newRoomName.text = SplitString(from.name);
        }

        newRoom.SetActive(true);
        yield return new WaitForSeconds(2);
        newRoom.SetActive(false);
    }

    private string SplitString(string s)
    {
        var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        return r.Replace(s, " ");
    }

    /* This function is to be used by the starting teleporter. */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && entering)
        {
            toTeleporter.GetComponent<Teleporter>().entering = false;
            entering = false;
            Action();
        } 
    }

    /* This function is to be used by the target teleporter. */
    private void OnTriggerExit2D(Collider2D other)
    {
        entering = true;
    }
}
