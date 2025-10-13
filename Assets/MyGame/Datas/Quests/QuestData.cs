using UnityEngine;

public enum QuestType { Kill, Collect }

[CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public QuestType questType;

    public EnemyType targetEnemyType;
    public int requiredAmount = 1;
    public int rewardExp = 50;
    
    public NPCData giverNPC;  // NPC cho quest
    public NPCData receiverNPC; // NPC cần báo cáo
}
