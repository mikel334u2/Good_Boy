using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [Tooltip("Quest items can be selected with description")]
    [SerializeField] private GameObject questButtonPrefab;
    [Tooltip("Layout panel where quest items will go")]
    [SerializeField] private Transform questPanel;
    [Tooltip("Description textbox for each quest")]
    [SerializeField] private Text questDescription;

    // COIN QUEST
    public string nameOfCoinQuest = "Shiny Things";
    public Text scoreText;
    [HideInInspector] public int coinCount = 0;

    // KEY QUEST
    public string nameOfKeyQuest = "The Keys to Success";
    public GameObject[] keys;
    public GameObject[] keyImages;
    public Dictionary<GameObject, GameObject> keyMap;
    [HideInInspector] public int keyCount = 0;

    // STICKERS/MOTHS
    public GameObject[] stickers;
    public GameObject[] stickerImages;
    public Text stickerText;
    public Dictionary<GameObject, GameObject> stickerMap;
    [HideInInspector] public int stickerCount = 0;

    [HideInInspector] public bool questPrinted = false;
    [HideInInspector] public List<string> friends;
    //public float currentHealth = 3;
    //public float capacityHealth = 3;
    // public bool respawn = true;
    // public Vector3 respawnPos = new Vector3(0, 0, 0);
    private M_PlayerController controller;
    private M_Camera m_camera;
    private GameObject respawn;
    [HideInInspector] public Dictionary<string, Quest> quests = new Dictionary<string, Quest>();
    // public Text questText;
    // public Text friendsList;
    // public Text questMessage;

    // Singleton pattern
    private static PlayerInfo _instance;
    public static PlayerInfo Player { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        friends = new List<string>();
        // currentHealth = capacityHealth;
        // respawnPos = transform.position;
        if (!TryGetComponent<M_PlayerController>(out controller))
        {
            Debug.LogError("Attach a player controller to PlayerInfo");
        }
        if (!GameObject.FindGameObjectWithTag("MainCamera").TryGetComponent<M_Camera>(out m_camera))
        {
            Debug.LogError("Attach a player camera to PlayerInfo");
        }
        respawn = GameObject.FindGameObjectWithTag("Respawn");
        // questText.gameObject.SetActive(false);
        transform.position = respawn.transform.position;

        keyMap = new Dictionary<GameObject, GameObject>();
        for (int i = 0; i < keyImages.Length; i++)
        {
            keyMap.Add(keys[i], keyImages[i]);
        }
        stickerMap = new Dictionary<GameObject, GameObject>();
        for (int i = 0; i < stickerImages.Length; i++)
        {
            stickerMap.Add(stickers[i], stickerImages[i]);
        }

        // TAKE THESE OUT
        // coinCount = 49;
    }

    public void PrintQuests(Button questTab)
    {
        if (questPrinted)
            return;
        foreach (Quest q in quests.Values)
        {
            GameObject questButton = (GameObject) Instantiate(questButtonPrefab);
            questButton.transform.SetParent(questPanel, false);
            OnQuestSelect questInfo = questButton.GetComponent<OnQuestSelect>();
            questInfo.quest = q;
            questInfo.questButton = questTab;
            string questText = "(" + (q.Completed ? "X" : " ") + ")\t" + q.Name;
            questButton.GetComponent<Text>().text = questText;
        }
        questDescription.text = "";
        questPrinted = true;
    }

    public void RemoveQuests()
    {
        foreach (Transform child in questPanel)
        {
            Destroy(child.gameObject);
        }
        questPrinted = false;
    }

    public void PrintQuestDesc(Quest quest)
    {
        questDescription.text = quest.Print();
    }

    public void CheckQuestCompleted(Quest quest)
    {
        switch (quest.Type)
        {
            case QuestType.Collect:
                if (quest.CurrentItems >= quest.RequiredItems)
                {
                    quest.CurrentItems = quest.RequiredItems;
                    quest.Completed = true;
                    // [TODO] Display some congratulations message for collecting all items
                }
                break;
            case QuestType.Goal:
                quest.Completed = true;
                // Debug.Log("Goal reached.");
                // [TODO] Display congratulations message for reaching goal
                break;
        }
    }

    public void AddFriend(string friendName)
    {
        if (!friends.Contains(friendName))
        {
            friends.Add(friendName);
        }
    }

    public IEnumerator Respawn()
    {
        controller.grounded = true;
        controller.enabled = false;
        yield return new WaitForSeconds(0.1f);
        controller.enabled = true;
        transform.position = respawn.transform.position;
    }
}
