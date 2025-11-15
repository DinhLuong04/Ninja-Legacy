using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;
    public Button acceptButton;
    public Button completeButton;

    private NPCData currentNpcData;
    private NPC currentNpc;
    private string[] sentences;
    private int index;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);

        nextButton.onClick.AddListener(NextSentence);
        acceptButton.onClick.AddListener(OnAcceptQuest);
        completeButton.onClick.AddListener(OnCompleteQuest);
    }

    public void StartDialogue(DialogueData data, NPCData npcData, NPC npc)
    {
        var qm = QuestManager.Instance;
        currentNpcData = npcData;
        currentNpc = npc;

        npcNameText.text = npcData.npcName;
        npcPortrait.sprite = npcData.portrait;

        QuestData currentQuest = qm.GetCurrentQuest();
        QuestState state = qm.GetState();

        acceptButton.gameObject.SetActive(false);
        completeButton.gameObject.SetActive(false);

        if (currentQuest != null && qm.CanReport(npcData.npcName) && currentQuest.receiverNPC == npcData)
        {
            ShowDialogue(currentQuest.dialogueCompleted);
            return;
        }

        if (currentQuest != null && state == QuestState.InProgress && currentQuest.giverNPC == npcData)
        {
            ShowDialogue(currentQuest.dialogueInProgress);
            return;
        }


        if (currentQuest != null && state == QuestState.NotStarted && currentQuest.giverNPC == npcData)
        {
            ShowDialogue(currentQuest.dialogueNotStarted);
            return;
        }

        if (npcData.defaultDialogue != null)
        {
            ShowDialogue(npcData.defaultDialogue);
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }

   void ShowDialogue(DialogueData data)
{
    if (data == null)
    {
        dialoguePanel.SetActive(false);
        return;
    }

    sentences = data.sentences;
    index = 0;

    dialoguePanel.SetActive(true);
    dialogueText.text = sentences[index];

    // Reset tất cả nút
    nextButton.gameObject.SetActive(false);
    acceptButton.gameObject.SetActive(false);
    completeButton.gameObject.SetActive(false);

    // Nếu có nhiều câu → dùng NEXT
    if (sentences.Length > 1)
    {
        nextButton.gameObject.SetActive(true);
        return;
    }

    // Nếu chỉ có 1 câu → kiểm tra nút nhiệm vụ ngay lập tức
    UpdateButtonVisibility();

    // Nếu không phải nhiệm vụ → auto-close
    if (!acceptButton.gameObject.activeSelf && !completeButton.gameObject.activeSelf)
    {
        StartCoroutine(CloseDialogueAfterDelay(2f));
    }
}


    public void NextSentence()
{
    index++;

    if (index < sentences.Length)
    {
        dialogueText.text = sentences[index];

        if (index == sentences.Length - 1)
            nextButton.gameObject.SetActive(false);
    }
    else
    {
        nextButton.gameObject.SetActive(false);
        UpdateButtonVisibility();

        if (!acceptButton.gameObject.activeSelf && !completeButton.gameObject.activeSelf)
        {
            StartCoroutine(CloseDialogueAfterDelay(2f));
        }
    }
}


    void UpdateButtonVisibility()
    {
        acceptButton.gameObject.SetActive(false);
        completeButton.gameObject.SetActive(false);

        var qm = QuestManager.Instance;
        QuestData currentQuest = qm.GetCurrentQuest();

        if (currentQuest != null)
        {
            if (qm.GetState() == QuestState.NotStarted && currentQuest.giverNPC == currentNpcData)
            {
                acceptButton.gameObject.SetActive(true);
            }
            else if (qm.CanReport(currentNpcData.npcName))
            {
                completeButton.gameObject.SetActive(true);
            }
        }
    }

    public void OnAcceptQuest()
    {
        if (currentNpcData != null)
        {
            var qm = QuestManager.Instance;
            QuestData currentQuest = qm.GetCurrentQuest();
            if (currentQuest != null && currentQuest.giverNPC == currentNpcData)
            {
                qm.StartQuest(currentQuest);
                dialoguePanel.SetActive(false);
                currentNpc?.UpdateQuestIcon();
            }
        }
    }

    public void OnCompleteQuest()
    {
        QuestManager.Instance.ReportQuest();
        dialoguePanel.SetActive(false);
        currentNpc?.UpdateQuestIcon();
    }

    private System.Collections.IEnumerator CloseDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialoguePanel.SetActive(false);
    }
    public void ShowTutorialDialogue(string message, float duration = 3f)
{
    dialoguePanel.SetActive(true);
    dialogueText.text = message;
    nextButton.gameObject.SetActive(false);
    completeButton.gameObject.SetActive(false);
    acceptButton.gameObject.SetActive(false);
    StopAllCoroutines();
    StartCoroutine(AutoHideDialogue(duration));
}

private IEnumerator AutoHideDialogue(float delay)
{
    yield return new WaitForSeconds(delay);
    dialoguePanel.SetActive(false);
}

}