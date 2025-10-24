using UnityEngine;

public enum QuestState { NotStarted, InProgress, Completed, Rewarded }

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public QuestData startingQuest;
    public static System.Action OnQuestUpdated;

    private QuestData currentQuest;
    private int currentProgress;
    private QuestState state = QuestState.NotStarted;
    private bool allQuestsCompleted = false;
    public bool AllQuestsCompleted => allQuestsCompleted;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        
        if (allQuestsCompleted)
        {
            OnQuestUpdated?.Invoke();
            return;
        }

        // Khởi tạo quest đầu nếu có
        if (currentQuest == null && startingQuest != null)
        {
            currentQuest = startingQuest;
            state = QuestState.NotStarted;
            OnQuestUpdated?.Invoke();
            if (QuestUI.Instance != null)
                QuestUI.Instance.ShowHint("Hãy đến gặp Trưởng Làng để nhận nhiệm vụ đầu tiên!");
        }
    }

    public void StartQuest(QuestData quest)
    {
        currentQuest = quest;
        currentProgress = 0;
        state = QuestState.InProgress;

        Debug.Log($"[QuestManager] Nhận quest: {quest.questName}");

        if (QuestUI.Instance != null)
            QuestUI.Instance.ShowQuest(quest.questName, quest.description, $"0 / {quest.requiredAmount}");

        OnQuestUpdated?.Invoke();
    }

    public void EnemyKilled(EnemyType type)
    {
        if (currentQuest == null || state != QuestState.InProgress) return;
        if (type != currentQuest.targetEnemyType) return;

        currentProgress++;
        Debug.Log($"Quest tiến trình: {currentProgress}/{currentQuest.requiredAmount}");

        if (QuestUI.Instance != null)
            QuestUI.Instance.ShowQuest(currentQuest.questName, currentQuest.description, $"{currentProgress} / {currentQuest.requiredAmount}");

        if (currentProgress >= currentQuest.requiredAmount)
        {
            state = QuestState.Completed;
            Debug.Log("Quest hoàn thành! Hãy quay lại NPC để báo cáo.");
            OnQuestUpdated?.Invoke();

            if (QuestUI.Instance != null && currentQuest.receiverNPC != null)
            {
                string npcName = currentQuest.receiverNPC.npcName;
                QuestUI.Instance.ShowHint($"Hãy quay lại gặp {npcName} để báo cáo nhiệm vụ!");
            }

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

        // Trao thưởng
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null) ps.GainExp(currentQuest.rewardExp);
        }

        
        if (QuestUI.Instance != null)
            QuestUI.Instance.HideQuest();

        if (currentQuest.nextQuest != null)
        {
            currentQuest.nextQuest.giverNPC = currentQuest.nextQuestNPC;
            currentQuest = currentQuest.nextQuest;
            state = QuestState.NotStarted;

            string nextNpcName = currentQuest.giverNPC != null ? currentQuest.giverNPC.npcName : "???";
            if (QuestUI.Instance != null)
                QuestUI.Instance.ShowHint($"Hãy đến gặp {nextNpcName} để nhận nhiệm vụ mới.");
        }
        else
        {
            // Hết chuỗi nhiệm vụ
            if (QuestUI.Instance != null)
                QuestUI.Instance.ShowHint("Bạn đã hoàn thành tất cả chuỗi nhiệm vụ! Hãy nghỉ ngơi hoặc quay lại làng.");
            currentQuest = null;
            state = QuestState.NotStarted;
            allQuestsCompleted = true;
        }

        OnQuestUpdated?.Invoke();
    }

    public void ResetAllQuestsCompleted()
    {
        allQuestsCompleted = false;
        OnQuestUpdated?.Invoke();
    }

    public QuestState GetState() => state;
    public QuestData GetCurrentQuest() => currentQuest;
}
