using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class QuestSO : ScriptableObject {
    public string questID;               // unique identifier
    public string questTitle;
    [TextArea] public string description;
    public List<Objective> objectives;
}

[System.Serializable]
public class Objective {
    public string description;
    public ObjectiveType type;
    public string targetID;              // e.g. NPC name
    public int requiredAmount;
}

public enum ObjectiveType { Talk, Collect, Kill, Find }