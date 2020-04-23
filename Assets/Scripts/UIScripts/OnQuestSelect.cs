using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnQuestSelect : MonoBehaviour, ISelectHandler
{
    [HideInInspector] public Quest quest;

    public void OnSelect(BaseEventData eventData)
    {
        PlayerInfo.Player.PrintQuestDesc(quest);
    }
}
