using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickOnSelect : MonoBehaviour, ISelectHandler
{
    private Button button;
    private bool selectWithoutClicking;

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent<Button>(out button))
        {
            Debug.LogWarning("No button attached to ClickOnSelect object");
        }
        selectWithoutClicking = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (button != null && button.gameObject.activeInHierarchy && !selectWithoutClicking)
            button.onClick.Invoke();
        selectWithoutClicking = false;
    }

    public void SelectWithoutClicking()
    {
        if (button != null && gameObject.activeInHierarchy)
        {
            selectWithoutClicking = true;
            IEnumerator function = UIActions.UI.SelectLater(button);
            StartCoroutine(function);
        }
    }
}
