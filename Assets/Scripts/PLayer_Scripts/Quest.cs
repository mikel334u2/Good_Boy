using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum QuestType
{
    Collect,
    Goal
}

public class Quest
{
    private string name;        // Name of the quest
    private QuestType type;     // Type of quest
    private string assigner;    // Who assigned the quest
    private bool completed;     // Has it been completed?
    private string description; // Description of the quest

    // Collect quests
    private string tagOfItem;   // Tag of item to be collected (must tag item in Unity)
    private int requiredItems;  // Required number of items to complete task
    private int currentItems;   // Current number of items held

    // Goal quests
    private string nameOfGoal;  // Name of the goal

    public Quest()
    {
        completed = false;
    }

    public string Name { get => name; set => name = value; }
    public QuestType Type { get => type; set => type = value; }
    public string Assigner { get => assigner; set => assigner = value; }
    public bool Completed { get => completed; set => completed = value; }
    public string Description { get => description; set => description = value; }
    public string TagOfItem { get => tagOfItem; set => tagOfItem = value; }
    public int RequiredItems { get => requiredItems; set => requiredItems = value; }
    public int CurrentItems { get => currentItems; set => currentItems = value; }
    public string NameOfGoal { get => nameOfGoal; set => nameOfGoal = value; }

    // Prints all quest details
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Quest: ").AppendLine(name);
        sb.Append("Type: ").AppendLine(type.ToString());
        sb.Append("Assigner: ").AppendLine(assigner);
        sb.Append("Status: ").AppendLine(Completed ? "Completed" : "Incomplete");
        switch (type)
        {
            case QuestType.Collect:
                sb.Append("Item: ").AppendLine(tagOfItem);
                sb.Append(currentItems).Append(" / ").Append(requiredItems).AppendLine(" Collected");
                break;
            case QuestType.Goal:
                sb.Append("Destination: ").AppendLine(nameOfGoal);
                break;
        }
        sb.Append("Description: ").Append(description);
        return sb.ToString();
    }

    // Shorter version of ToString method
    public string Print()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Quest: ").AppendLine(name);
        sb.Append("Status: ").AppendLine(Completed ? "Completed" : "Incomplete");
        switch (type)
        {
            case QuestType.Collect:
                sb.Append(currentItems).Append(" / ").Append(requiredItems).AppendLine(" Collected");
                break;
        }
        sb.Append("Description: ").Append(description);
        return sb.ToString();
    }
}
