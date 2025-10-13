using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

        //  Nếu có thể nộp quest
        if (qm.CanReport(npcData.npcName))
        {
            ShowSimpleDialogue("Tốt lắm! Cảm ơn vì đã hoàn thành nhiệm vụ.");
            completeButton.gameObject.SetActive(true);
            return;
        }

        //  Nếu đang làm nhiệm vụ
        if (qm.GetState() == QuestState.InProgress && qm.GetCurrentQuest() == npcData.quest)
        {
            ShowSimpleDialogue("Hãy hoàn thành nhiệm vụ trước đã!");
            return;
        }

        //  Nếu đã hoàn thành
        if (qm.GetState() == QuestState.Rewarded)
        {
            ShowSimpleDialogue("Cảm ơn, nhiệm vụ đã xong!");
            return;
        }

        // Nếu là hội thoại bình thường
        sentences = data.sentences;
        index = 0;

        dialoguePanel.SetActive(true);
        dialogueText.text = sentences[index];
        nextButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        completeButton.gameObject.SetActive(false);
    }

    void ShowSimpleDialogue(string text)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = text;
        nextButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(false);
        completeButton.gameObject.SetActive(false);
        StartCoroutine(CloseDialogueAfterDelay(3f));
    }

    public void NextSentence()
    {
        index++;
        if (index < sentences.Length)
        {
            dialogueText.text = sentences[index];
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            UpdateButtonVisibility();
        }
    }

    void UpdateButtonVisibility()
    {
        acceptButton.gameObject.SetActive(false);
        completeButton.gameObject.SetActive(false);

        var qm = QuestManager.Instance;
        if (currentNpcData != null && currentNpcData.quest != null)
        {
            var quest = currentNpcData.quest;

            if (qm.GetState() == QuestState.NotStarted && quest.giverNPC == currentNpcData)
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
        if (currentNpcData != null && currentNpcData.quest != null)
        {
            QuestManager.Instance.StartQuest(currentNpcData.quest);
            dialoguePanel.SetActive(false);
            currentNpc?.UpdateQuestIcon();
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
}
