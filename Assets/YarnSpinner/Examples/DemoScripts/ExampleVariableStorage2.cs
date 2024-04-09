/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections.Generic;
using Yarn.Unity;

/// An extremely simple implementation of DialogueUnityVariableStorage, which
/// just stores everything in a Dictionary.
public class ExampleVariableStorage2 : VariableStorageBehaviour
{
    public GameObject backgroundMusic;
    public GameObject inventory;
    private InventoryScript inv;
    public TimeTransition timeTransition;
    public PlayerMovement playerMovement;
    public GameObject dialogueRunner;

    public GameObject doorStop;
    public Animator blackholeAnimator;
    public EndSequence endSequence;

    private bool playMusic = false;
    private bool momHelpAthena = false;
    private bool day1 = true;
    private bool day2, day3, day4 = false;
    private bool endSpaceScene = false;
    private int day = 1;
    private bool goToBed = false;
    private bool notifyDad = false;
    private bool canSleep1 = false;
    private bool hasMovedDay2, hasMovedDay3, hasMovedDay4 = false;
    private bool hasHelpedAthena, hasNotifiedDad, hasMovedMomSleep1 = false;
    private AudioSource soundEffect;
    private NPCMovement npcMovement;
    private Dictionary<string, Yarn.Value> bufferVariables;
    private bool mom, dad, athena, johnson = false;
    private bool hasStartedFinale = false;

    // Minigame variables
    public GameObject minigame;
    public GameObject cockpit;
    private bool tutorial = true;
    public bool left = false;
    public bool center = false;
    public bool right = false;

    /// Where we actually keeping our variables
    Dictionary<string, Yarn.Value> variables = new Dictionary<string, Yarn.Value> ();

    /// A default value to apply when the object wakes up, or
    /// when ResetToDefaults is called
    [System.Serializable]
    public class DefaultVariable
    {
        /// Name of the variable
        public string name;
        /// Value of the variable
        public string value;
        /// Type of the variable
        public Yarn.Value.Type type;
    }

    /// Our list of default variables, for debugging.
    public DefaultVariable[] defaultVariables;

    [Header("Optional debugging tools")]
    /// A UI.Text that can show the current list of all variables. Optional.
    public UnityEngine.UI.Text debugTextView;

    /// Reset to our default values when the game starts
    void Awake ()
    {
        ResetToDefaults ();
        soundEffect = gameObject.GetComponent<AudioSource>();
        npcMovement = FindObjectOfType<NPCMovement>();
        inv = inventory.GetComponent<InventoryScript>();
    }

    /// Erase all variables and reset to default values
    public override void ResetToDefaults ()
    {
        Clear ();

        // For each default variable that's been defined, parse the string
        // that the user typed in in Unity and store the variable
        foreach (var variable in defaultVariables) {
            
            object value;

            switch (variable.type) {
            case Yarn.Value.Type.Number:
                float f = 0.0f;
                float.TryParse(variable.value, out f);
                value = f;
                break;

            case Yarn.Value.Type.String:
                value = variable.value;
                break;

            case Yarn.Value.Type.Bool:
                bool b = false;
                bool.TryParse(variable.value, out b);
                value = b;
                break;

            case Yarn.Value.Type.Variable:
                // We don't support assigning default variables from other variables
                // yet
                Debug.LogErrorFormat("Can't set variable {0} to {1}: You can't " +
                    "set a default variable to be another variable, because it " +
                    "may not have been initialised yet.", variable.name, variable.value);
                continue;

            case Yarn.Value.Type.Null:
                value = null;
                break;

            default:
                throw new System.ArgumentOutOfRangeException ();

            }

            var v = new Yarn.Value(value);

            SetValue ("$" + variable.name, v);
        }
    }

    /// Set a variable's value
    public override void SetValue (string variableName, Yarn.Value value)
    {
        // Copy this value into our list
        variables[variableName] = new Yarn.Value(value);
    }

    /// Get a variable's value
    public override Yarn.Value GetValue (string variableName)
    {
        // If we don't have a variable with this name, return the null value
        if (variables.ContainsKey(variableName) == false)
            return Yarn.Value.NULL;
        
        return variables [variableName];
    }

    /// Erase all variables
    public override void Clear ()
    {
        variables.Clear ();
    }

    /// If we have a debug view, show the list of all variables in it
    void Update ()
    {
        if (debugTextView != null) {
            var stringBuilder = new System.Text.StringBuilder ();
            foreach (KeyValuePair<string,Yarn.Value> item in variables) {
                stringBuilder.AppendLine (string.Format ("{0} = {1}",
                                                         item.Key,
                                                         item.Value));
            }
            debugTextView.text = stringBuilder.ToString ();
            
        }
        bufferVariables = new Dictionary<string, Yarn.Value>(variables);
        foreach (KeyValuePair<string, Yarn.Value> item in bufferVariables)
        {
            if (item.Key == "$has_watch" && item.Value.AsBool == true && !inv.IsInInventory("Watch"))
            {
                if (item.Key == "$has_watch_but_lost" && item.Value.AsBool == false)
                {
                    inv.AddToInventory("Watch");
                }
            }
            else if (item.Key == "$has_watch" && item.Value.AsBool == false && inv.IsInInventory("Watch"))
            {
                inv.RemoveFromInventory("Watch");
            }
            else if (item.Key == "$has_watch_but_lost" && item.Value.AsBool == true && inv.IsInInventory("Watch"))
            {
                inv.RemoveFromInventory("Watch");
            }

            if (playMusic == false && item.Key == "$play_music" && item.Value.AsBool == true)
            {
                backgroundMusic.GetComponent<AudioSource>().Stop();
                soundEffect.Play();
                playMusic = true;
            }
            if (playMusic == true && item.Key == "$play_music" && item.Value.AsBool == false)
            {
                soundEffect.Stop();
                backgroundMusic.GetComponent<AudioSource>().Play();
                playMusic = false;
            }
            if (momHelpAthena == false && item.Key == "$help_athena" && item.Value.AsBool == true)
            {
                momHelpAthena = true;
            }

            if (endSpaceScene == false && item.Key == "$end_space_scene" && item.Value.AsBool == true)
            {
                FindObjectOfType<Space>().EndSpaceScene();
                doorStop.SetActive(true);
                endSpaceScene = true;
            }
            if (day2 == false && item.Key == "$day_2" && item.Value.AsBool == true)
            {
                playerMovement.ChangeSpeed(3.8f);
                blackholeAnimator.speed = 1.5f;
                day2 = true;
                day = 2;
                //timeTransition.StartTimeTransition();   // Remember to remove this line before game demo!
                //Debug.Log("Day: " + day);               // Remember to remove this line before game demo!
            }
            if (day3 == false && item.Key == "$day_3" && item.Value.AsBool == true)
            {
                playerMovement.ChangeSpeed(3.5f);
                blackholeAnimator.speed = 5.0f;
                day3 = true;
                day = 3;
                //timeTransition.StartTimeTransition();   // Remember to remove this line before game demo!
                //Debug.Log("Day: " + day);               // Remember to remove this line before game demo!
            }
            if (day4 == false && item.Key == "$day_4" && item.Value.AsBool == true)
            {
                playerMovement.ChangeSpeed(8.0f);
                day4 = true;
                day = 4;
                //timeTransition.StartTimeTransition();   // Remember to remove this line before game demo!
                //Debug.Log("Day: " + day);               // Remember to remove this line before game demo!
            }
            //if (goToBed == false && item.Key == "$go_to_bed" && item.Value.AsBool == true)
            //{
            //    timeTransition.StartTimeTransition(day);
            //    goToBed = true;
            //}
            //if (goToBed == true && item.Key == "$go_to_bed" && item.Value.AsBool == false)
            //{
            //    goToBed = false;
            //}
            if (item.Key == "$go_to_bed_2" && item.Value.AsBool == true)
            {
                timeTransition.StartTimeTransition(2);
                SetValue("$go_to_bed_2", new Yarn.Value(false));
            }
            if (item.Key == "$go_to_bed_3" && item.Value.AsBool == true)
            {
                timeTransition.StartTimeTransition(3);
                SetValue("$go_to_bed_3", new Yarn.Value(false));
            }
            if (item.Key == "$go_to_bed_4" && item.Value.AsBool == true)
            {
                timeTransition.StartTimeTransition(4);
                SetValue("$go_to_bed_4", new Yarn.Value(false));
            }
            if (item.Key == "$go_to_bed_5" && item.Value.AsBool == true)
            {
                timeTransition.StartTimeTransition(5);
                SetValue("$go_to_bed_5", new Yarn.Value(false));
            }
            if (item.Key == "$unlock_doors" && item.Value.AsBool == true && doorStop.activeSelf) {
                doorStop.SetActive(false);
                SetValue("$unlock_doors", new Yarn.Value(false));
            }
            if (notifyDad == false && item.Key == "$dad_notified" && item.Value.AsBool == true)
            {
                notifyDad = true;
            }
            if (canSleep1 == false && item.Key == "$can_sleep_1" && item.Value.AsBool == true)
            {
                canSleep1 = true;
            }

            // MINIGAME VARIABLES
            if (item.Key == "$start_ch2_minigame" && item.Value.AsBool == true && cockpit.activeSelf)
            {
                if (tutorial && !minigame.activeSelf)
                {
                    minigame.SetActive(true);
                    tutorial = false;
                }
                
            }
            if (!left && item.Key == "$start_left_minigame" && item.Value.AsBool == true)
            {
                minigame.GetComponent<Minigame2>().InitiateGame(0);
                left = true;
            }
            if (!center && item.Key == "$start_center_minigame" && item.Value.AsBool == true)
            {
                minigame.GetComponent<Minigame2>().InitiateGame(1);
                center = true;
            }
            if (!right && item.Key == "$start_right_minigame" && item.Value.AsBool == true)
            {
                minigame.GetComponent<Minigame2>().InitiateGame(2);
                right = true;
            }
            // END

            if (item.Key == "$mom_face_back" && item.Value.AsBool == true)
            {
                npcMovement.TurnMom("Back");
                SetValue("$mom_face_back", new Yarn.Value(false));
            }
            if (item.Key == "$mom_face_front" && item.Value.AsBool == true)
            {
                npcMovement.TurnMom("Front");
                SetValue("$mom_face_front", new Yarn.Value(false));
            }
            if (item.Key == "$mom_face_right" && item.Value.AsBool == true)
            {
                npcMovement.TurnMom("Right");
                SetValue("$mom_face_right", new Yarn.Value(false));
            }
            if (item.Key == "$mom_face_left" && item.Value.AsBool == true)
            {
                npcMovement.TurnMom("Left");
                SetValue("$mom_face_left", new Yarn.Value(false));
            }
            if (item.Key == "$athena_face_front" && item.Value.AsBool == true)
            {
                npcMovement.TurnAthena("Front");
                SetValue("$athena_face_front", new Yarn.Value(false));
            }
            if (item.Key == "$athena_face_back" && item.Value.AsBool == true)
            {
                npcMovement.TurnAthena("Back");
                SetValue("$athena_face_back", new Yarn.Value(false));
            }
            if (item.Key == "$athena_face_right" && item.Value.AsBool == true)
            {
                npcMovement.TurnAthena("Right");
                SetValue("$athena_face_right", new Yarn.Value(false));
            }
            if (item.Key == "$dad_face_back" && item.Value.AsBool == true)
            {
                npcMovement.TurnDad("Back");
                SetValue("$dad_face_back", new Yarn.Value(false));
            }
            if (item.Key == "$dad_face_front" && item.Value.AsBool == true)
            {
                npcMovement.TurnDad("Front");
                SetValue("$dad_face_front", new Yarn.Value(false));
            }
            if (item.Key == "$dad_face_right" && item.Value.AsBool == true)
            {
                npcMovement.TurnDad("Right");
                SetValue("$dad_face_right", new Yarn.Value(false));
            }
            if (item.Key == "$dad_face_left" && item.Value.AsBool == true)
            {
                npcMovement.TurnDad("Left");
                SetValue("$dad_face_left", new Yarn.Value(false));
            }
            if (item.Key == "$player_face_front" && item.Value.AsBool == true)
            {
                SetValue("$player_face_front", new Yarn.Value(false));
                playerMovement.SetY(-1);
            }
            if (item.Key == "$flickering" && item.Value.AsBool == true)
            {
                SetValue("$flickering", new Yarn.Value(false));
                endSequence.Flickering();
            }
            if (item.Key == "$has_talked_to_johnson_final" && item.Value.AsBool == true)
            {
                SetValue("$has_talked_to_johnson_final", new Yarn.Value(false));
                johnson = true;
            }
            if (item.Key == "$has_talked_to_athena_final" && item.Value.AsBool == true)
            {
                SetValue("$has_talked_to_athena_final", new Yarn.Value(false));
                athena = true;
            }
            if (item.Key == "$has_talked_to_mom_final" && item.Value.AsBool == true)
            {
                SetValue("$has_talked_to_mom_final", new Yarn.Value(false));
                mom = true;
            }
            if (item.Key == "$has_talked_to_dad_final" && item.Value.AsBool == true)
            {
                SetValue("$has_talked_to_dad_final", new Yarn.Value(false));
                dad = true;
            }
            if (item.Key == "$sad_credits" && item.Value.AsBool == true)
            {
                SetValue("$sad_credits", new Yarn.Value(false));
                StartCoroutine(endSequence.SadCredits());
            }
            if (item.Key == "$happy_credits" && item.Value.AsBool == true)
            {
                SetValue("$happy_credits", new Yarn.Value(false));
                StartCoroutine(endSequence.HappyCredits());
            }
        }

        if (playMusic && !dialogueRunner.GetComponent<DialogueRunner>().isDialogueRunning)
        {
            playMusic = false;
            soundEffect.Stop();
            backgroundMusic.GetComponent<AudioSource>().Play();
        }

        bufferVariables = null;
        //if (Input.GetKey(KeyCode.U))
        //{
        //    SetValue("$unlock_doors", new Yarn.Value(true));
        //    endSequence.EndStuff();
        //}
        //if (Input.GetKey(KeyCode.Alpha1))
        //{
        //    day = 1;
        //    SetValue("$day_2", new Yarn.Value(false));
        //    Debug.Log("hi");
        //}
        //if (Input.GetKey(KeyCode.Alpha2))
        //{
        //    day = 2;
        //    SetValue("$day_2", new Yarn.Value(true));
        //    SetValue("$day_3", new Yarn.Value(false));
        //    SetValue("$day_4", new Yarn.Value(false));
        //    SetValue("$day_5", new Yarn.Value(false));
        //}
        //if (Input.GetKey(KeyCode.Alpha3))
        //{
        //    day = 3;
        //    day2 = true;
        //    SetValue("$day_2", new Yarn.Value(true));
        //    SetValue("$day_3", new Yarn.Value(true));
        //    SetValue("$day_4", new Yarn.Value(false));
        //    SetValue("$day_5", new Yarn.Value(false));
        //}
        //if (Input.GetKey(KeyCode.Alpha4))
        //{
        //    day = 4;
        //    day2 = true;
        //    day3 = true;
        //    SetValue("$day_2", new Yarn.Value(true));
        //    SetValue("$day_3", new Yarn.Value(true));
        //    SetValue("$day_4", new Yarn.Value(true));
        //    SetValue("$day_5", new Yarn.Value(false));
        //}
        //if (Input.GetKey(KeyCode.Alpha5))
        //{
        //    day = 5;
        //    day2 = true;
        //    day3 = true;
        //    day4 = true;
        //    SetValue("$day_2", new Yarn.Value(true));
        //    SetValue("$day_3", new Yarn.Value(true));
        //    SetValue("$day_4", new Yarn.Value(true));
        //    SetValue("$day_5", new Yarn.Value(true));
        //}
        if (johnson && mom && dad && athena && !hasStartedFinale)
        {
            endSequence.StartGrandFinale();
            hasStartedFinale = true;
        }
    }

    public int GetDay()
    {
        return day;
    }

    public bool HasToMoveMom()
    {
        if (!hasHelpedAthena && momHelpAthena)
        {
            hasHelpedAthena = true;
            SetValue("$mom_has_moved_to_talk_to_athena", new Yarn.Value(true));
            return true;
        }
        return false;
    }

    public bool HasToNotifyDad()
    {
        if (!hasNotifiedDad && notifyDad)
        {
            hasNotifiedDad = true;
            SetValue("$mom_has_moved_to_talk_to_dad", new Yarn.Value(true));
            return true;
        }
        return false;
    }

    public bool HasToMoveMomSleep1()
    {
        if (!hasMovedMomSleep1 && canSleep1)
        {
            hasMovedMomSleep1 = true;
            return true;
        }
        return false;
    }

    public bool HasToMoveDay2()
    {
        if (!hasMovedDay2 && day == 2)
        {
            hasMovedDay2 = true;
            return true;
        }
        return false;
    }

    public bool HasToMoveDay3()
    {
        if (!hasMovedDay3 && day == 3)
        {
            hasMovedDay3 = true;
            return true;
        }
        return false;
    }

    public bool HasToMoveDay4()
    {
        if (!hasMovedDay4 && day == 4)
        {
            hasMovedDay4 = true;
            return true;
        }
        return false;
    }

    // MINIGAME FUNCTIONS
    public void SetNPCAtComp(int index)
    {
        if (index == 0)
        {
            SetValue("$npc_at_left", new Yarn.Value(true));
        } else if (index == 1)
        {
            SetValue("$npc_at_center", new Yarn.Value(true));
        }
        else if (index == 2)
        {
            SetValue("$npc_at_right", new Yarn.Value(true));
        }
    }

    public void SetNPCAtCompFalse(int index)
    {
        if (index == 0)
        {
            SetValue("$npc_at_left", new Yarn.Value(false));
        }
        else if (index == 1)
        {
            SetValue("$npc_at_center", new Yarn.Value(false));
        }
        else if (index == 2)
        {
            SetValue("$npc_at_right", new Yarn.Value(false));
        }
    }

    public void EndTutorial()
    {
        tutorial = false;
    }

    public void SetDay5True()
    {
        SetValue("$day_5", new Yarn.Value(true));
    }
}
