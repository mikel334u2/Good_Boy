using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public string questName;
    private PlayerInfo player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.TryGetComponent<PlayerInfo>(out player);
            Quest quest = player.quests[questName];

            // Do not check completed quests
            if (quest.Completed)
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
                    Destroy(other.gameObject);
                    break;
                case QuestType.Goal:
                    quest.Completed = true;
                    Debug.Log("Goal reached.");
                   // [TODO] Display congratulations message for reaching goal
                    break;
            }
        }
    }
}
