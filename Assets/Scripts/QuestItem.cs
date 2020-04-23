using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public string questName;
    private PlayerInfo player;

    private bool hasReceivedCoinQuest = false;

    private void Start()
    {
        player = PlayerInfo.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Quest quest = null;
            if (player.quests.ContainsKey(questName))
            {
                quest = player.quests[questName];
            }

            HandleSpecialQuestItem(quest);

            // Do not check completed quests
            if (quest == null || quest.Completed)
                return;
            
            switch (quest.Type)
            {
                case QuestType.Collect:
                    quest.CurrentItems++;
                    if (quest.CurrentItems >= quest.RequiredItems)
                    {
                        quest.Completed = true;
                        // [TODO] Display some congratulations message for collecting all items
                    }
                    Destroy(gameObject);
                    break;
                case QuestType.Goal:
                    quest.Completed = true;
                    Debug.Log("Goal reached.");
                   // [TODO] Display congratulations message for reaching goal
                    break;
            }
        }
    }

    private void HandleSpecialQuestItem(Quest quest)
    {
        // for the coin quest
        if (questName.Equals(player.nameOfCoinQuest))
        {
            player.coinCount++;
            player.scoreText.text = player.coinCount.ToString();
            if (!hasReceivedCoinQuest && quest != null)
            {
                quest.CurrentItems = player.coinCount - 1;
                hasReceivedCoinQuest = true;
            }
            Destroy(gameObject);
        }
    }
}
