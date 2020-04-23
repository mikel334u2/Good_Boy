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

        // [TODO] INITIALIZE UI TO FALSE
    }

    // private void Update()
    // {
    //     if (Input.GetButtonDown("e"))
    //     {
    //         controller.zeroMovement = !controller.zeroMovement;
    //         m_camera.isRotatable = !m_camera.isRotatable;
    //         questText.text = PrintQuests();
    //         questText.gameObject.SetActive(!questText.gameObject.activeSelf);
    //     }
    // }

    public void PrintQuests()
    {
        if (questPrinted)
            return;
        foreach (Quest q in quests.Values)
        {
            GameObject questButton = (GameObject) Instantiate(questButtonPrefab);
            questButton.transform.parent = questPanel;
            questButton.GetComponent<OnQuestSelect>().quest = q;
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
