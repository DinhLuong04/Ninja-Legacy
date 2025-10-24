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
    
    public NPCData giverNPC;  
    public NPCData receiverNPC;
    
    public QuestData nextQuest;  
    public NPCData nextQuestNPC;
    public DialogueData dialogueNotStarted; // Lời mời nhận quest
    public DialogueData dialogueInProgress; // Lời động viên khi làm
    public DialogueData dialogueCompleted; // Lời chúc mừng/report
    public DialogueData dialogueRewarded;  
}
