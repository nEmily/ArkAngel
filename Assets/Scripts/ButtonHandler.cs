using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour
{
    public GameObject[] buttonList;
    public int numButtons;
    private int selectedButton = 0;

    private Color color;
    public float unselectAlpha = 0;
    public float selectAlpha = .3f;

    public bool isVertical = true;
    public bool isDialogue = true;

    public void Awake()
    {
        color = buttonList[selectedButton].GetComponent<Image>().color;
    }

    public void SetNumButtons(int num)
    {
        numButtons = num;
    }

    public void OnEnable()
    {
        selectedButton = 0;
        //buttonList[selectedButton].GetComponent<Image>().color = new Color(color.r, color.g, color.b, selectAlpha);
        if (isDialogue)
        {
            buttonList[selectedButton].GetComponent<Image>().color = new Color(color.r, color.g, color.b, selectAlpha);
        }
    }

    public void OnDisable()
    {
        foreach(var button in buttonList)
        {
            button.GetComponent<Image>().color = new Color(color.r, color.g, color.b, unselectAlpha);
        }
    }

    void Update()
    {
        if (isVertical)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                MoveToNextButton();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                MoveToPreviousButton();
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveToNextButton();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveToPreviousButton();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SpaceBar");
            Action(selectedButton);
        }

    }

    void MoveToNextButton()
    {
        buttonList[selectedButton].GetComponent<Image>().color = new Color(color.r, color.g, color.b, unselectAlpha);
        selectedButton++;
        if (selectedButton >= numButtons)
        {
            selectedButton = 0;
        }
        buttonList[selectedButton].GetComponent<Image>().color = new Color(color.r, color.g, color.b, selectAlpha);
    }

    void MoveToPreviousButton()
    {
        buttonList[selectedButton].GetComponent<Image>().color = new Color(color.r, color.g, color.b, unselectAlpha);
        selectedButton--;
        if (selectedButton <= -1)
        {
            selectedButton = numButtons - 1;
        }
        buttonList[selectedButton].GetComponent<Image>().color = new Color(color.r, color.g, color.b, selectAlpha);
    }

    void Action(int index)
    {
        if (isDialogue)
        {
            gameObject.GetComponent<Yarn.Unity.Example.ExampleDialogueUI>().SetOption(index);
        }
        else
        {
            buttonList[selectedButton].gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void SetSelectedButton(int i)
    {
        selectedButton = i;
        //SetSelectedButtonColor();
        SetUnselectedButtonColor();
    }

    void SetSelectedButtonColor()
    {
        buttonList[selectedButton].GetComponent<Image>().color = new Color(color.r, color.g, color.b, selectAlpha);
    }

    void SetUnselectedButtonColor()
    {
        for (int i = 0; i < numButtons; i++)
        {
            if (i != selectedButton)
            {
                buttonList[i].GetComponent<Image>().color = new Color(color.r, color.g, color.b, unselectAlpha);
            }
        }
    }
}
