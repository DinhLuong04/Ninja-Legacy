using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData npcData;
    public GameObject questAvailableIcon;
    public GameObject questTurnInIcon;
    protected bool playerInRange;
    private Coroutine blinkCoroutine;

    protected virtual void Start()
    {
        UpdateQuestIcon();
        QuestManager.OnQuestUpdated += UpdateQuestIcon;
    }

    void OnDestroy()
    {
        QuestManager.OnQuestUpdated -= UpdateQuestIcon;
    }

    protected virtual void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DialogueData selectedDialogue = GetDialogueBasedOnState();

            //  Nếu không có thoại nhiệm vụ, fallback về thoại mặc định
            if (selectedDialogue == null && npcData.defaultDialogue != null)
            {
                selectedDialogue = npcData.defaultDialogue;
            }

            if (selectedDialogue != null)
                DialogueManager.Instance.StartDialogue(selectedDialogue, npcData, this);
        }

    }

    public void UpdateQuestIcon()
    {
        
        if (QuestManager.Instance != null && QuestManager.Instance.AllQuestsCompleted)
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            if (questAvailableIcon != null) questAvailableIcon.SetActive(false);
            if (questTurnInIcon != null) questTurnInIcon.SetActive(false);
            return; 
        }

        //  Reset icon trước khi kiểm tra mới
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (questAvailableIcon != null) questAvailableIcon.SetActive(false);
        if (questTurnInIcon != null) questTurnInIcon.SetActive(false);

        //  Lấy thông tin quest hiện tại
        var qm = QuestManager.Instance;
        if (qm == null) return;

        var currentQuest = qm.GetCurrentQuest();
        var state = qm.GetState();

        if (npcData.availableQuests == null || npcData.availableQuests.Count == 0)
            return;

        //  Hiện icon "!" nếu là người cho nhiệm vụ mới (và nhiệm vụ chưa bắt đầu)
        if (currentQuest != null && state == QuestState.NotStarted && npcData == currentQuest.giverNPC)
        {
            questAvailableIcon.SetActive(true);
            blinkCoroutine = StartCoroutine(BlinkIcon(questAvailableIcon));
        }

        // Hiện icon "?" nếu là người nhận báo cáo (hoàn thành nhiệm vụ)
        if (qm.CanReport(npcData.npcName))
        {
            questTurnInIcon.SetActive(true);
            blinkCoroutine = StartCoroutine(BlinkIcon(questTurnInIcon));
        }
    }

    private System.Collections.IEnumerator BlinkIcon(GameObject icon)
    {
        while (true)
        {
            icon.SetActive(!icon.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"Player entered range of {npcData.npcName}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"Player exited range of {npcData.npcName}");
        }
    }

    private DialogueData GetDialogueBasedOnState()
    {
        var qm = QuestManager.Instance;
        var currentQuest = qm.GetCurrentQuest();

        if (currentQuest == null) return null;

        QuestState state = qm.GetState();
        foreach (var quest in npcData.availableQuests)
        {
            if (quest == currentQuest)
            {
                if (state == QuestState.NotStarted && npcData == quest.giverNPC)
                    return quest.dialogueNotStarted;
                if (state == QuestState.InProgress && npcData == quest.giverNPC)
                    return quest.dialogueInProgress;
                if (state == QuestState.Completed && qm.CanReport(npcData.npcName))
                    return quest.dialogueCompleted;
                if (state == QuestState.Rewarded && npcData == quest.giverNPC)
                    return quest.dialogueRewarded;
            }
        }
        return null;
    }
}
