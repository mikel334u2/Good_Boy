using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnQuestSelect : MonoBehaviour, ISelectHandler
{
    [HideInInspector] public Quest quest;
    [HideInInspector] public Button questButton;

    void Start()
    {
        Selectable s;
        if (!TryGetComponent<Selectable>(out s))
        {
            Debug.LogWarning("Selectable not on quest object");
        }
        if (s != null && questButton != null)
        {
            Navigation n = s.navigation;
            n.selectOnRight = questButton;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayerInfo.Player.PrintQuestDesc(quest);
    }
}
