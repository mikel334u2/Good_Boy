using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/********************
 * DIALOGUE MANAGER *
 ********************
 * This Dialogue Manager is what links your dialogue which is sent by the Dialogue Trigger to Unity
 *
 * The Dialogue Manager navigates the sent text and prints it to text objects in the canvas and will toggle
 * the Dialogue Box when appropriate
 */

public class DialogueManager : MonoBehaviour
{
    public GameObject CanvasBox; // your fancy canvas box that holds your text objects
    public Text TextBox; // the text body
    public Text NameTextBox; // the text body of the name you want to display
    public Image ImageBox; // the position of the character image for your dialogue
    public bool freezePlayerOnDialogue = true;

    private Queue<string> inputStream = new Queue<string>(); // stores dialogue
    private M_PlayerController pController; // used for freezing player
    private DialogueTrigger triggerObject;
    private bool destroyTrigger = false;

    private void Start()
    {
        CanvasBox.SetActive(false); // close the dialogue box on play
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<M_PlayerController>(out pController);
    }

    public void StartDialogue(Queue<string> dialogue, DialogueTrigger triggerObject)
    {
        this.triggerObject = triggerObject;
        if (freezePlayerOnDialogue)
        {
            pController.zeroMovement = true; // disable player controller
        }
        CanvasBox.SetActive(true); // open the dialogue box
        ImageBox.sprite = null;
        inputStream = dialogue; // store the dialogue from dialogue trigger
        AdvanceDialogue(); // Prints out the first line of dialogue
    }

    public void EndDialogue()
    {
        TextBox.text = "";
        NameTextBox.text = "";
        inputStream.Clear();
        CanvasBox.SetActive(false);
        if (freezePlayerOnDialogue)
        {
            pController.zeroMovement = false; // enable player controller
        }
        triggerObject.isDisabled = destroyTrigger; // if it's a one-time dialogue, disable the DialogueTrigger
    }

    // call when a player presses a button in Dialogue Trigger
    public void AdvanceDialogue()
    {
        // Debug.Log("Advancing dialogue");
        if (inputStream.Count <= 0)
        {
            EndDialogue(); // End the dialogue if it's over
            return;
        }

        // Debug.Log(inputStream.Peek() + " -- " + CheckNextTag());
        // Processes the tags and dialogue between breaks
        bool endDialogue = false;
        bool hasNormalDialogue = false;
        string tagTitle = CheckNextTag();
        while (tagTitle != "BREAK")
        {
            // Debug.Log(inputStream.Peek());
            switch (tagTitle)
            {
                case "NAME":    // Set the name of character speaking
                    NameTextBox.text = DequeueNextTag();
                    break;
                case "IMG":     // Set the image of character speaking
                    // Real path: Assets/Resources/...
                    ImageBox.sprite = Resources.Load<Sprite>(DequeueNextTag());
                    // Debug.Log(ImageBox.sprite != null);
                    ImageBox.gameObject.SetActive(ImageBox.sprite != null);
                    break;
                case "QUEST":   // this must be the first tag of the "quest" tags
                    // If all necessary tags aren't there, this won't work
                    Quest quest = CreateQuest();
                    // Debug.Log(quest.ToString());
                    DialogueEvents.Manager.OnQuestAdded(quest);
                    PlayerInfo.Player.quests.Add(quest.Name, quest);
                    break;
                case "CALL":
                    DialogueEvents.Manager.HandleEvent(DequeueNextTag());
                    break;
                case null:
                    TextBox.text = inputStream.Dequeue(); // Set dialogue text
                    hasNormalDialogue = true;
                    break;
                case "CLEAR":   // Clears dialogue textbox without advancing dialogue
                    TextBox.text = "";
                    hasNormalDialogue = true;
                    inputStream.Dequeue();
                    break;
                case "DESTROY": // Deactivates the DialogueTrigger so subsequent interactions cannot occur
                    destroyTrigger = true;
                    endDialogue = true;
                    inputStream.Dequeue();
                    break;
                case "END":     // Force ends the dialogue
                    endDialogue = true;
                    inputStream.Dequeue();
                    break;
            }
            tagTitle = CheckNextTag();
        }
        inputStream.Dequeue();

        if (endDialogue) // if tag ends dialogue, clear the queue
        {
            inputStream.Clear();
        }
        if (!hasNormalDialogue) // in case there's no actual dialogue in a line
        {
            AdvanceDialogue();
        }
    }

    // Returns the name of the next tag
    // Returns null if there is no tag
    private string CheckNextTag()
    {
        string tag = inputStream.Peek();
        if (Regex.IsMatch(tag, @"^\[\s*[A-Z]+\s*=[^\[\]]*\]$")) // tag pattern
            return tag.Substring(1, tag.IndexOf('=') - 1).Trim();
        return null;
    }

    // Returns the value of the next tag
    private string DequeueNextTag()
    {
        string tag = inputStream.Dequeue();
        return tag.Substring(tag.IndexOf('=') + 1, tag.IndexOf(']') - (tag.IndexOf('=') + 1)).Trim();
    }

    private Quest CreateQuest()
    {
        // Create a new quest and add the rest of the quest descriptors
        Quest quest = new Quest();
        quest.Name = DequeueNextTag();                                              // QUEST tag
        quest.Description = DequeueNextTag();                                       // DESC tag
        string questType = CheckNextTag().ToLower();
        questType = char.ToUpper(questType[0]) + questType.Substring(1); // in case capitalization is wrong
        quest.Type = (QuestType) Enum.Parse(typeof(QuestType), questType);   // TYPE tag

        // Dequeues next tags based on quest type
        switch (quest.Type)
        {
            case QuestType.Collect:
                string[] parameters = DequeueNextTag().Split(new char[]{','});
                quest.TagOfItem = parameters[0].Trim();;                                 // TAG tag
                quest.RequiredItems = int.Parse(parameters[1].Trim());                  // NUM tag
                break;
            case QuestType.Goal:
                quest.NameOfGoal = DequeueNextTag();
                break;
        }
        quest.Assigner = NameTextBox.text; // Gets the name of the assigner from the name text box
        return quest;
    }
}