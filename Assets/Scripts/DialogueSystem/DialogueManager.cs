using System.Collections;
using System.Collections.Generic;
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
    public Text NameText; // the text body of the name you want to display
    public bool freezePlayerOnDialogue = true;
    private Queue<string> inputStream = new Queue<string>(); // stores dialogue
    private M_PlayerController pController; // used for freezing player
    private PlayerInfo playerInfo;

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
        inputStream = dialogue; // store the dialogue from dialogue trigger
        AdvanceDialogue(); // Prints out the first line of dialogue
    }

    public void AdvanceDialogue() // call when a player presses a button in Dialogue Trigger
    {
        if (inputStream.Count <= 0)
        {
            return;
        }
        
        PlayerInfo.Quest quest = null; // quest to add to player's quests
        if (inputStream.Peek().Contains("EndQueue")) // special phrase to stop dialogue
        {
            inputStream.Dequeue(); // Clear Queue
            EndDialogue();
        }
        else if (inputStream.Peek().Contains("[NAME"))
        {
            NameText.text = DequeueNextTag();
            AdvanceDialogue(); // print the rest of this line
        }
        else if (inputStream.Peek().Contains("[QUEST")) // this must be the first tag of the "quest" tags
        {
            quest = new PlayerInfo.Quest();
            quest.Name = DequeueNextTag();
            AdvanceDialogue();
        }
        else if (inputStream.Peek().Contains("[TAG"))
        {
            quest.TagOfItem = DequeueNextTag();
            AdvanceDialogue();
        }
        else if (inputStream.Peek().Contains("[NUM"))
        {
            quest.RequiredItems = int.Parse(DequeueNextTag());
            AdvanceDialogue();
        }
        else if (inputStream.Peek().Contains("[DESC"))
        {
            quest.Description = DequeueNextTag();
            AdvanceDialogue();
        }
        else
        {
            TextBox.text = inputStream.Dequeue();
            if (quest != null)
            {
                quest.CurrentItems = 0;
                quest.Assigner = NameText.text; // Gets the name of the assigner from the name text box
                playerInfo.quests.Add(quest);
            }
        }
    }

    private string DequeueNextTag()
    {
        string tag = inputStream.Dequeue();
        return tag.Substring(tag.IndexOf('=') + 1, tag.IndexOf(']') - (tag.IndexOf('=') + 1));
    }

    public void EndDialogue()
    {
        TextBox.text = "";
        NameText.text = "";
        inputStream.Clear();
        CanvasBox.SetActive(false);
        if (freezePlayerOnDialogue)
        {
            pController.zeroMovement = false; // enable player controller
        }
    }
}