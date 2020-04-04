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
    private GameObject respawn;
    [HideInInspector] public List<Quest> quests = new List<Quest>();
    public Text questText;
    // public Text friendsList;
    // public Text questMessage;

    private void Start()
    {
        friends = new List<string>();
        // currentHealth = capacityHealth;
        // respawnPos = transform.position;
        TryGetComponent<M_PlayerController>(out controller);
        respawn = GameObject.FindGameObjectWithTag("Respawn");
        questText.gameObject.SetActive(false);
        transform.position = respawn.transform.position;
    }

    private void Update()
    {
        if (Input.GetButtonDown("e"))
        {
            controller.zeroMovement = !controller.zeroMovement;
            questText.text = "(" + (quests[0].Completed ? '/' : ' ') + ")\t" + quests[0].Name;
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

    // public void PrintQuest(int i)
    // {
    //     StringBuilder sb = new StringBuilder();
    //     sb.Append(quests[i].ToString());
    //     questText.text = sb.ToString().TrimEnd();
    // }

    // public void PrintFriends()
    // {
    //     StringBuilder sb = new StringBuilder();
    //     foreach (string friend in friends)
    //     {
    //         sb.Append(friend).Append('\n');
    //     }
    //     friendsList.text = sb.ToString().TrimEnd();
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Death")
        {
            Respawn();
        }
        foreach (Quest quest in quests)
        {
            if (quest.Completed) continue; // Do not check completed quests
            
            // If it's a collecting quest item, collect the item
            if (quest.Type == QuestType.Collect && other.gameObject.tag == quest.TagOfItem)
            {
                quest.CurrentItems++;
                if (quest.CurrentItems >= quest.RequiredItems)
                {
                    quest.Completed = true;
                    // [TODO] Display some congratulations message for collecting all items
                }
                Destroy(other.gameObject);
            }

            // If it's a goal area, complete that quest
            else if (quest.Type == QuestType.Goal && other.name == quest.NameOfGoal)
            {
                quest.Completed = true;
                Debug.Log("Goal reached.");
                // [TODO] Display congratulations message for reaching goal
            }
        }
    }

    private void Respawn()
    {
        controller.grounded = true;
        transform.position = respawn.transform.position;
        Debug.Log("Died");
    }

    // public void ModifyHealth(float amount)
    // {
    //     // TODO: spot for animation
    //     currentHealth += amount;
    //     if (currentHealth > capacityHealth)
    //     {
    //         currentHealth = capacityHealth;
    //     }
    //     else if (currentHealth <= 0)
    //     {
    //         if (respawn)
    //         {
    //             // UponRespawn();
    //             controller.grounded = true;
    //             transform.position = respawnPos;
    //             currentHealth = capacityHealth;
    //         }
    //         else
    //         {
    //             // Kill the object
    //             // TODO: spot for animation
    //             gameObject.SetActive(false);
    //             Destroy(gameObject);
    //         }
    //     }
    // }

    // private void UponRespawn()
    // {
    //     // TODO: Spot for animation
    //     if (controller != null)
    //     {
    //         controller.grounded = true;
    //         controller.enabled = false;
    //         Invoke("WaitThenRespawn", 1.5f);
    //     }

    // }

    // void WaitThenRespawn()
    // {
    //     transform.position = respawnPos;
    //     currentHealth = capacityHealth;
    //     controller.enabled = true;
    // }
}
