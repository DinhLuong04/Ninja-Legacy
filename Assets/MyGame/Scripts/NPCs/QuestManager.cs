using UnityEngine;

public enum QuestState { NotStarted, InProgress, Completed, Rewarded }

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // Event thông báo cho tất cả NPC khi trạng thái nhiệm vụ thay đổi
    public static System.Action OnQuestUpdated;

    private QuestData currentQuest;
    private int currentProgress;
    private QuestState state = QuestState.NotStarted;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartQuest(QuestData quest)
    {
        currentQuest = quest;
        currentProgress = 0;
        state = QuestState.InProgress;

        Debug.Log($"[QuestManager] Nhận quest: {quest.questName}");

        QuestUI.Instance.ShowQuest(
            quest.questName,
            quest.description,
            $"0 / {quest.requiredAmount}"
        );

        OnQuestUpdated?.Invoke(); // 🔥 báo NPC cập nhật icon
    }

    public void EnemyKilled(EnemyType type)
    {
        if (currentQuest == null || state != QuestState.InProgress) return;
        if (type != currentQuest.targetEnemyType) return;

        currentProgress++;
        Debug.Log($"Quest tiến trình: {currentProgress}/{currentQuest.requiredAmount}");

        QuestUI.Instance.ShowQuest(
            currentQuest.questName,
            currentQuest.description,
            $"{currentProgress} / {currentQuest.requiredAmount}"
        );

        if (currentProgress >= currentQuest.requiredAmount)
        {
            state = QuestState.Completed;
            Debug.Log("Quest hoàn thành! Hãy quay lại NPC để báo cáo.");
            OnQuestUpdated?.Invoke(); //  báo NPC update icon (hiện dấu ?)
        }
    }

    public bool CanReport(string npcName)
    {
        bool result = currentQuest != null
                      && state == QuestState.Completed
                      && currentQuest.receiverNPC != null
                      && npcName == currentQuest.receiverNPC.npcName;

        Debug.Log($"[QuestManager] CanReport({npcName}) => {result} | state={state} | currentQuest={(currentQuest != null ? currentQuest.questName : "null")}");
        return result;
    }

    public void ReportQuest()
    {
        if (currentQuest == null || state != QuestState.Completed) return;

        Debug.Log($"[QuestManager] Báo cáo thành công quest: {currentQuest.questName}");
        state = QuestState.Rewarded;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null) ps.GainExp(currentQuest.rewardExp);
        }

        QuestUI.Instance.HideQuest();
        currentQuest = null;

        OnQuestUpdated?.Invoke(); 
    }

    public QuestState GetState() => state;
    public QuestData GetCurrentQuest() => currentQuest;
}
