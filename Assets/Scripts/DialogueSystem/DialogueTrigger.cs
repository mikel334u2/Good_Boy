using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/********************
 * DIALOGUE TRIGGER *
 ********************
 * A Dialogue Trigger can be anything, an NPC, a signpost, or even an in game event.
 * 
 * A Dialogue Trigger is responsible to activate based on some action in the game e.g. talking to an NPC
 * To handle talking to an NPC, we first attach this script to an NPC along with a dialogue file we write (e.g. .txt)
 */

public class DialogueTrigger : MonoBehaviour
{
    public TextAsset textFileAsset; // your imported text file for your NPC
    public bool TriggerWithButton = false;
    public GameObject optionalButtonIndicator;
    public Vector3 optionalIndicatorOffset = new Vector3(0, 0, 0);
    private Queue<string> dialogue = new Queue<string>(); // stores the dialogue (Great Performance!)
    private float waitTime = 0.5f; // lag time for advancing dialogue so you can actually read it
    private float nextTime = 0f; // used with waitTime to create a timer system
    private bool dialogueTriggered;
    private GameObject indicator;
    private DialogueManager dialogueManager;
    private string button = "Jump";

    // public bool useCollision; // unused for now

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (optionalButtonIndicator != null)
        {
            indicator = Instantiate(optionalButtonIndicator, transform.position + optionalIndicatorOffset, Quaternion.identity, transform);
            indicator.SetActive(false);
        }
    }
    /* Called when you want to start dialogue */
    void TriggerDialogue()
    {
        ReadTextFile(); // loads in the text file
        dialogueManager.StartDialogue(dialogue); // Accesses Dialogue Manager and Starts Dialogue
    }

    /* loads in your text file */
    private void ReadTextFile()
    {
        string txt = textFileAsset.text;

        // Split dialogue lines by 2 newlines
        string[] lines = txt.Split((System.Environment.NewLine + System.Environment.NewLine).ToCharArray());

        foreach (string line in lines) // for every line of dialogue
        {
            if (!string.IsNullOrEmpty(line))// ignore empty lines of dialogue
            {
                int start = 0; // Starting location to look for strings
                int end = 0;   // End of a string bit
                if (line.Substring(start).StartsWith("[NAME")) // e.g [NAME=Michael] Hello, my name is Michael
                {
                    // substring(start, length), length = ending index (new "start") - starting index
                    end = line.IndexOf(']', start) + 1;
                    dialogue.Enqueue(line.Substring(start, end - start)); // adds [NAME=Michael] to be printed
                    start = end;
                }
                // e.g. [QUEST=Quest Name][TAG=Apple][NUM=3][DESC=Your job is to collect...] So your job is to...
                if (line.Substring(start).StartsWith("[QUEST"))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        end = line.IndexOf(']', start) + 1;
                        dialogue.Enqueue(line.Substring(start, end - start)); // adds [QUEST=Quest Name] to be printed
                        start = end;
                    }
                }
                dialogue.Enqueue(line.Substring(start).Trim()); // adds to the dialogue to be printed
            }
        }
        dialogue.Enqueue("EndQueue");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameObject.tag == "NPC")
            {
                other.gameObject.GetComponent<PlayerInfo>().AddFriend(gameObject.name);
            }

            if (!TriggerWithButton)
            {
                TriggerDialogue();
                dialogueTriggered = true;
                nextTime = Time.timeSinceLevelLoad + waitTime;
            }
            else if (indicator != null)
            {
                indicator.SetActive(true);
            }
            // Debug.Log("Collision");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        // Debug.Log(other.name);
        if (other.gameObject.tag == "Player" && Input.GetButton(button) && nextTime < Time.timeSinceLevelLoad)
        {
            if (!dialogueTriggered)
            {
                TriggerDialogue();
                dialogueTriggered = true;
                if (indicator != null)
                {
                    indicator.SetActive(false);
                }
            }
            else
            {
                dialogueManager.AdvanceDialogue();
            }
            nextTime = Time.timeSinceLevelLoad + waitTime;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            dialogueManager.EndDialogue();
            dialogueTriggered = false;
            if (indicator != null)
            {
                indicator.SetActive(false);
            }
        }
    }
}
