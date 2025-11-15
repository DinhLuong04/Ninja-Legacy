using UnityEngine;

public enum QuestType { Kill, Collect }

[CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public QuestType questType;
    [Header("Collect Quest")]
    public ItemData targetItem;
    public GameObject CollectItemPrefab;
    [Header("Kill Quest")]
    public EnemyType targetEnemyType;
    public int requiredAmount = 1;
    public int rewardExp = 50;
    [Header("Chuỗi nhiệm vụ")]
    public QuestData previousQuest;  
    public NPCData giverNPC;  
    public NPCData receiverNPC;
    
    public QuestData nextQuest;  
    public NPCData nextQuestNPC;
    public DialogueData dialogueNotStarted; // Lời mời nhận quest
    public DialogueData dialogueInProgress; // Lời động viên khi làm
    public DialogueData dialogueCompleted; // Lời chúc mừng/report

}
