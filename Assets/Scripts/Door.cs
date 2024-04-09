using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
//using UnityEditor;


public class Door : MonoBehaviour {

    public GameObject from;
    public GameObject to;
    public GameObject toDoor;

    public GameObject[] randomDoors;
    public bool isRandom = false;
    public string doorCode = "0";

    public GameObject changeRoomsScreen;

    public GameObject newRoom;
    public Text newRoomName;

    public GameObject levelController;
    private GameController gameController;
    private AudioSource doorAudio;
    public AudioClip doorClip;

    private GameObject player;
    private GameObject playerStatus;
    private string playerName;

    private NPCMovement npcMovement;

    private ExampleVariableStorage2 gameVariables;

    private Sequence sequence;

    private static bool firstTimeGoThroughWeirdDoor = true;

    private void Start()
    {
        doorAudio = levelController.GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameController = levelController.GetComponent<GameController>();


        playerStatus = GameObject.FindGameObjectWithTag("PlayerStatus");
        playerName = playerStatus.GetComponent<PlayerStatus>().getName();

        gameVariables = GameObject.FindObjectOfType<ExampleVariableStorage2>();
        npcMovement = GameObject.FindObjectOfType<NPCMovement>();

        sequence = GameObject.FindGameObjectWithTag("SecretDoorSequence").GetComponent<Sequence>();
    }

    void Action()
    {
        if (doorCode != "0")
        {
            sequence.AddToSequence(doorCode);
        }
        doorAudio.clip = doorClip;
        doorAudio.Play();
        gameController.FreezeGame();
        StartCoroutine(GoToRoom());


        if (gameVariables.HasToMoveMom() == true)
        {
            Vector2 pos = npcMovement.GetAthenaPos();
            npcMovement.MoveMom(new Vector2(pos.x, pos.y - 1.0f), "Back");
            npcMovement.MoveAthena(pos, "Front");
        }
        if (gameVariables.HasToNotifyDad() == true)
        {
            Vector2 pos = npcMovement.GetDadPos();
            npcMovement.MoveMom(new Vector2(pos.x + 0.8f, pos.y), "Left");
            npcMovement.MoveDad(npcMovement.GetDadPos(), "Right");
        }
        if (gameVariables.HasToMoveMomSleep1() == true)
        {
            Vector2 pos = npcMovement.GetDadPos();
            npcMovement.MoveMom(new Vector2(pos.x, pos.y - 1), "Back");
        }
        if (gameVariables.HasToMoveDay2() == true)
        {
            npcMovement.MoveJohnson(new Vector2(0.068f, 15.849f), "Back");
            npcMovement.MoveDad(new Vector2(-15.71f, -11.3f), "Back");
            npcMovement.MoveMom(new Vector2(-0.87f, 10.57f), "Front");
        }
        if (gameVariables.HasToMoveDay3() == true)
        {
            npcMovement.MoveJohnson(new Vector2(-24.6f, -11.3f), "Left");
            npcMovement.MoveDad(new Vector2(14.8f, -2.82f), "Back");
            npcMovement.MoveMom(new Vector2(-0.1f, 15.79f), "Back");
        }
        if (gameVariables.HasToMoveDay4() == true)
        {
            npcMovement.MoveJohnson(new Vector2(4.86f, 15.5f), "Back");
            npcMovement.MoveDad(new Vector2(-5.21f, 13.18f), "Front");
            npcMovement.MoveMom(new Vector2(-3f, 9.36f), "Right");
            npcMovement.MoveAthena(new Vector2(4.85f, 10.02f), "Left");
        }
    }

    IEnumerator GoToRoom()
    {
        if (isRandom)
        {
            toDoor = randomDoors[Random.Range(0, randomDoors.Length)];
            player.transform.position = new Vector2(toDoor.transform.position.x, toDoor.transform.position.y);
            gameController.UnfreezeGame();
            if (firstTimeGoThroughWeirdDoor)
            {
                firstTimeGoThroughWeirdDoor = false;
                StartCoroutine(Test());
            }
        }
        else
        {
            changeRoomsScreen.SetActive(true);
            player.transform.position = new Vector2(toDoor.transform.position.x, toDoor.transform.position.y);

            to.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);

            from.SetActive(false);
            changeRoomsScreen.SetActive(false);
            toDoor.GetComponent<Door>().StartNewRoom();
            gameController.UnfreezeGame();
        }
    }

    public void StartNewRoom()
    {
        StartCoroutine(NewRoomState());
    }

    IEnumerator NewRoomState()
    { //newRoomName.text.Contains("Riley") && 
        if (playerName != null && playerName != "") {
            newRoomName.text = SplitString(from.name).Replace("Riley", playerName);
        } else
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "PlayerInteract")
        {
            Action();
        }
    }

    IEnumerator Test()
    {
        FindObjectOfType<PlayerMovement>().FreezePlayer();
        yield return new WaitForSecondsRealtime(0.1f);

        FindObjectOfType<DialogueRunner>().StartDialogue("WeirdDoor");
        while (FindObjectOfType<DialogueRunner>().isDialogueRunning)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }
        FindObjectOfType<PlayerMovement>().UnfreezePlayer();
    }
}
