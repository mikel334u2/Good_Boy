using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Text;

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
 * Tags: NAME, IMG, QUEST, DESC, COLLECT, GOAL, DESTROY, BREAK, END, CLEAR, CALL
 */

public class DialogueTrigger : MonoBehaviour
{
    // public TextAsset textFileAsset;
    public List<TextAsset> textFileAssets; // your imported text file for your NPC
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
    [HideInInspector] public bool isDisabled = false;
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
        TextAsset textFileAsset = DialogueEvents.Manager.CheckAndLoadText(textFileAssets);
        // Debug.Log("Text Asset Name: " + textFileAsset.name);
        string txt = textFileAsset.text;
        string[] lines = txt.Split(Separators, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines) // for every line of dialogue
        {
            StringBuilder sb = new StringBuilder(line); // Helps improve efficiency
            SearchAndQueueTag(sb, "NAME"); // adds [NAME=Michael] to be printed
            SearchAndQueueTag(sb, "IMG"); // adds [IMG=Images/chester] to be processed
            if (SearchAndQueueTag(sb, "QUEST")) // if QUEST is found, adds [QUEST=Quest Name] and returns true
            {
                SearchAndQueueTag(sb, "DESC"); // [DESC=Your job is to collect 3 apples.]
                foreach (string type in Enum.GetNames(typeof(QuestType))) // {Collect, Goal, ...}
                {
                    if (SearchAndQueueTag(sb, type.ToUpper()))
                        break; // If tag found, queue it and break
                }
            }
            SearchAndQueueTag(sb, "CALL"); // adds [CALL=Cart Animation]

            QueueActualDialogue(sb.ToString());  // Queue the actual dialogue
            SearchAndQueueTag(sb, "CLEAR");
            SearchAndQueueTag(sb, "DESTROY");
            SearchAndQueueTag(sb, "END");

            dialogue.Enqueue("[BREAK=]"); // breaks up the lines
        }
    }

    /*
     * Searches for a tag in the line.
     * If found, add it to dialogue, remove it, and return true.
     * Else, return false
     */
    private bool SearchAndQueueTag(StringBuilder sb, string tag)
    {
        string line = sb.ToString();
        string tagPattern = @"\[\s*" + tag + @"\s*=[^\[\]]*\]";
        string noEqualsPattern = @"\[\s*" + tag + @"\s*\]";
        Match matchTag = Regex.Match(line, tagPattern);
        Match noEqualsTag = Regex.Match(line, noEqualsPattern);
        if (matchTag.Success) // If there's a match for [TAG=something]
        {
            // Debug.Log(matchTag.Value);
            sb.Remove(matchTag.Index, matchTag.Length);
            dialogue.Enqueue(matchTag.Value);
            return true;
        }
        else if (noEqualsTag.Success) // If the tag is [DESTROY] or something
        {
            // Debug.Log(noEqualsTag.Value);
            sb.Remove(noEqualsTag.Index, noEqualsTag.Length);
            string newTag = noEqualsTag.Value.TrimEnd(']') + "=]";
            dialogue.Enqueue(newTag);
            return true;
        }
        return false;
    }

    // Queues the actual dialogue
    private void QueueActualDialogue(string line)
    {
        // Matches any text outside of tags
        string actualDialoguePattern = @"[^\]]+(?![^\[]*\])";
        Match match = Regex.Match(line, actualDialoguePattern);
        do  // Matches until the match value is non-whitespace (first match)
        {
            if (!String.IsNullOrWhiteSpace(match.Value))
            {
                dialogue.Enqueue(match.Value.Trim());
                return;
            }
            match = match.NextMatch();
        } while (match.Success);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isDisabled && other.gameObject.tag == "Player")
        {
            if (gameObject.tag == "NPC")
            {
                // If the other character is an NPC, add it as a friend
                PlayerInfo.Player.AddFriend(gameObject.name);
            }

            if (!TriggerWithButton) // If dialogue triggered on collision
            {
                ReadTextFile();
                dialogueManager.StartDialogue(dialogue, this);
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
        if (!isDisabled && other.gameObject.tag == "Player" && Input.GetButton(button) && nextTime < Time.timeSinceLevelLoad)
        {
            nextTime = Time.timeSinceLevelLoad + waitTime; // reset waiting period before advancing dialogue
            if (dialogueTriggered) // if dialogue triggered already, advance dialogue
            {
                dialogueManager.AdvanceDialogue();
            }
            else // Else, start the dialogue
            {
                ReadTextFile();
                dialogueManager.StartDialogue(dialogue, this);
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
