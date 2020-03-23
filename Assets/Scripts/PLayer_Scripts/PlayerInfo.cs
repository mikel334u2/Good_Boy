using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public class Quest
    {
        // "Collect" type quests
        private string name;
        private string assigner;
        private string description;
        private string tagOfItem;
        private int requiredItems;
        private int currentItems;

        public string Name { get => name; set => name = value; }
        public string Assigner { get => assigner; set => assigner = value; }
        public string Description { get => description; set => description = value; }
        public string TagOfItem { get => tagOfItem; set => tagOfItem = value; }
        public int RequiredItems { get => requiredItems; set => requiredItems = value; }
        public int CurrentItems { get => currentItems; set => currentItems = value; }
    }
    [HideInInspector] public List<string> friends;
    public float currentHealth = 3;
    public float capacityHealth = 3;
    public bool respawn = true;
    public Vector3 respawnPos = new Vector3(0, 0, 0);
    private M_PlayerController controller;
    [HideInInspector] public List<Quest> quests;

    private void Start()
    {
        friends = new List<string>();
        currentHealth = capacityHealth;
        respawnPos = transform.position;
        TryGetComponent<M_PlayerController>(out controller);
    }
    public void modifyHealth(float amount)
    {
        // TODO: spot for animation
        currentHealth += amount;
        if (currentHealth > capacityHealth)
        {
            currentHealth = capacityHealth;
        }
        else if (currentHealth <= 0)
        {
            if (respawn)
            {
                UponRespawn();
            }
            else
            {
                // Kill the object
                // TODO: spot for animation
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }

    private void UponRespawn()
    {
        // TODO: Spot for animation
        if (controller != null)
        {
            controller.grounded = true;
            controller.enabled = false;
            Invoke("WaitThenRespawn", 1.5f);
        }

    }

    void WaitThenRespawn()
    {
        transform.position = respawnPos;
        currentHealth = capacityHealth;
        controller.enabled = true;
    }

    public void AddFriend(string friendName)
    {
        if (!friends.Contains(friendName))
        {
            friends.Add(friendName);
        }
    }
}
