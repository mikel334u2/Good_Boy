using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public string questName;
    private PlayerInfo player;

    private bool hasReceivedCoinQuest = false;
    private bool hasReceivedKeyQuest = false;

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

            // Special cases because of UI/receiving items before start of quest
            HandleCoins(quest);
            HandleKeys(quest);
            HandleStickers();

            // Do not check completed quests
            if (quest == null || quest.Completed)
                return;
            
            switch (quest.Type)
            {
                case QuestType.Collect:
                    quest.CurrentItems++;
                    Destroy(gameObject);
                    break;
                case QuestType.Goal:
                    break;
            }

            player.CheckQuestCompleted(quest);
        }
    }

    private void HandleCoins(Quest quest)
    {
        if (questName.Equals(player.nameOfCoinQuest))
        {
            player.coinCount++;
            player.scoreText.text = player.coinCount.ToString();
            Destroy(gameObject);
        }
    }

    private void HandleKeys(Quest quest)
    {
        if (questName.Equals(player.nameOfKeyQuest))
        {
            player.keyCount++;
            if (player.keyMap.ContainsKey(gameObject))
            {
                player.keyMap[gameObject].SetActive(true);
            }
            Destroy(gameObject);
        }
    }

    private void HandleStickers()
    {
        // for stickers (not quests but it still works)
        if (player.stickerMap.ContainsKey(gameObject))
        {
            player.stickerCount++;
            player.stickerMap[gameObject].SetActive(true);
            player.stickerText.text = player.stickerCount.ToString();
            Destroy(gameObject);
        }
    }
}
