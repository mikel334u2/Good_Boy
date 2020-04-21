using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{

    [HideInInspector] public List<string> friends;
    //public float currentHealth = 3;
    //public float capacityHealth = 3;
    // public bool respawn = true;
    // public Vector3 respawnPos = new Vector3(0, 0, 0);
    private M_PlayerController controller;
    public M_Camera m_camera;
    private GameObject respawn;
    [HideInInspector] public Dictionary<string, Quest> quests = new Dictionary<string, Quest>();
    public Text questText;
    // public Text friendsList;
    // public Text questMessage;

    private void Start()
    {
        friends = new List<string>();
        // currentHealth = capacityHealth;
        // respawnPos = transform.position;
        if (!TryGetComponent<M_PlayerController>(out controller))
        {
            Debug.LogError("Attach a player controller to PlayerInfo");
        }
        respawn = GameObject.FindGameObjectWithTag("Respawn");
        questText.gameObject.SetActive(false);
        transform.position = respawn.transform.position;
    }

    private void Update()
    {
        if (Input.GetButtonDown("e"))
        {
            controller.zeroMovement = !controller.zeroMovement;
            m_camera.isRotatable = !m_camera.isRotatable;
            questText.text = quests.ContainsKey("Breaking Out") ? ("(" + (quests["Breaking Out"].Completed ? 'X' : ' ') + ")\tBreaking Out") : null;
            questText.gameObject.SetActive(!questText.gameObject.activeSelf);
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
