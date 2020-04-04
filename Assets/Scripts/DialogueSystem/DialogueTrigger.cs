using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

/********************
 * DIALOGUE TRIGGER *
 ********************
 * A Dialogue Trigger can be anything, an NPC, a signpost, or even an in game event.
 * 
 * A Dialogue Trigger is responsible to activate based on some action in the game e.g. talking to an NPC
 * To handle talking to an NPC, we first attach this script to an NPC along with a dialogue file we write (e.g. .txt)
 * 
 * Text file example is found in SampleDialogue.txt
 * Special characters: left bracket [ , right bracket ] , 2 single-quotes ''
 * Tags: NAME, IMG, QUEST, TYPE, TAG, NUM, DESC
 */

public class DialogueTrigger : MonoBehaviour
{
    public TextAsset textFileAsset; // your imported text file for your NPC
    public bool TriggerWithButton = false;
    public GameObject optionalButtonIndicator;
    public Vector3 optionalIndicatorOffset = new Vector3(0, 0, 0);
    public string button = "Jump";

    private bool dialogueTriggered = false;
    private Queue<string> dialogue = new Queue<string>(); // stores the dialogue (Great Performance!)
    private float waitTime = 0.5f; // lag time for advancing dialogue so you can actually read it
    private float nextTime = 0f; // used with waitTime to create a timer system
    private GameObject indicator;
    private DialogueManager dialogueManager;
    private string[] Separators = {"\'\'"}; // splits the dialogue by 2 single quotes

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (optionalButtonIndicator != null)
        {
            indicator = Instantiate(optionalButtonIndicator, transform.position + optionalIndicatorOffset, Quaternion.identity, transform);
            indicator.SetActive(false);
        }
    }

    /* Loads in your text file */
    private void ReadTextFile()
    {
        // Retrieve text, then split dialogue lines by 2 single quotes, removing empty strings
        string txt = textFileAsset.text;
        string[] lines = txt.Split(Separators, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines) // for every line of dialogue
        {
            SearchAndQueueTag(line, "NAME"); // adds [NAME=Michael] to be printed
            SearchAndQueueTag(line, "IMG"); // adds [IMG=Images/chester] to be processed
            if (SearchAndQueueTag(line, "QUEST")) // if QUEST is found, adds [QUEST=Quest Name] and returns true
            {
                SearchAndQueueTag(line, "DESC"); // [DESC=Your job is to collect 3 apples.]
                foreach (string type in Enum.GetNames(typeof(QuestType))) // {Collect, Goal, ...}
                {
                    if (SearchAndQueueTag(line, type.ToUpper()))
                        break; // If tag found, queue it and break
                }
            }
            
            // Queue the actual dialogue
            int start = line.LastIndexOf(']') + 1;
            dialogue.Enqueue(line.Substring(start).Trim());

            // // If there's an [END] tag, queue the dialogue then the [END] tag
            // if (line.Contains("[END]"))
            // {
            //     int start = line.LastIndexOf(']', line.IndexOf("[END]")) + 1;
            //     int end = line.IndexOf("[END]");
            //     dialogue.Enqueue(line.Substring(start, end - start).Trim());
            //     dialogue.Enqueue("[END]");
            // }
            // else // Else, just queue the dialogue
            // {
            //     int start = line.LastIndexOf(']') + 1;
            //     dialogue.Enqueue(line.Substring(start).Trim());
            // }
            dialogue.Enqueue("[BREAK=]"); // breaks up the lines
        }
    }

    /*
     * Searches for a tag in the line.
     * If found, add it to dialogue and return true.
     * Else, return false
     */
    private bool SearchAndQueueTag(string line, string tag)
    {
        string tagPattern = @"\[\s*" + tag + @"\s*=[^\[\]]*\]";
        Match match = Regex.Match(line, tagPattern);
        if (match.Success) // If there's a match for [TAG=something]
        {
            dialogue.Enqueue(match.Value);
            return true;
        }
        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameObject.tag == "NPC")
            {
                // If the other character is an NPC, add it as a friend
                other.gameObject.GetComponent<PlayerInfo>().AddFriend(gameObject.name);
            }

            if (!TriggerWithButton) // If dialogue triggered on collision
            {
                ReadTextFile();
                dialogueManager.StartDialogue(dialogue);
                nextTime = Time.timeSinceLevelLoad + waitTime;
                dialogueTriggered = true;
            }
            else if (indicator != null)
            {
                indicator.SetActive(true);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        // If player pressed button while colliding with other, then show dialogue
        if (other.gameObject.tag == "Player" && Input.GetButton(button) && nextTime < Time.timeSinceLevelLoad)
        {
            nextTime = Time.timeSinceLevelLoad + waitTime; // reset waiting period before advancing dialogue
            if (dialogueTriggered) // if dialogue triggered already, advance dialogue
            {
                dialogueManager.AdvanceDialogue();
            }
            else // Else, start the dialogue
            {
                ReadTextFile();
                dialogueManager.StartDialogue(dialogue);
                dialogueTriggered = true;
                if (indicator != null)
                {
                    indicator.SetActive(false);
                }
            }
        }
    }

    // If player leaves, end dialogue and clear dialogue queue
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
