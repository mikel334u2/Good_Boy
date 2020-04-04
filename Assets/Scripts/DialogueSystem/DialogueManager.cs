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
    private PlayerInfo playerInfo; // does stuff like add quests

    private void Start()
    {
        CanvasBox.SetActive(false); // close the dialogue box on play
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<M_PlayerController>(out pController);
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<PlayerInfo>(out playerInfo);
    }

    public void StartDialogue(Queue<string> dialogue)
    {
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
    }

    public void AdvanceDialogue() // call when a player presses a button in Dialogue Trigger
    {
        // Debug.Log("Advancing dialogue");
        if (inputStream.Count <= 0)
        {
            EndDialogue(); // End the dialogue if it's over
            return;
        }
        // Debug.Log(inputStream.Peek() + " -- " + CheckNextTag());
        string tagTitle = CheckNextTag();
        while (tagTitle != "BREAK")
        {
            switch (tagTitle)
            {
                case "NAME":    // Set the name of character speaking
                    NameTextBox.text = DequeueNextTag();
                    break;
                case "IMG":     // Set the image of character speaking
                    // Real path: Assets/Resources/...
                    ImageBox.sprite = Resources.Load<Sprite>(DequeueNextTag());
                    break;
                case "QUEST":   // this must be the first tag of the "quest" tags
                    // If all necessary tags aren't there, this won't work
                    Quest quest = CreateQuest();
                    Debug.Log(quest.ToString());
                    playerInfo.quests.Add(quest);
                    break;
                case null:
                    TextBox.text = inputStream.Dequeue(); // Set dialogue text
                    break;
            }
            tagTitle = CheckNextTag();
        }
        inputStream.Dequeue();
        if (inputStream.Count <= 0)
        {
            EndDialogue();
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