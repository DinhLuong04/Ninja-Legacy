using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData npcData;
    public GameObject questAvailableIcon; // Dấu "!"
    public GameObject questTurnInIcon;    // Dấu "?"

    protected bool  playerInRange;
    private Coroutine blinkCoroutine;

    protected virtual void Start()
    {
        UpdateQuestIcon();

        // Lắng nghe sự kiện khi Quest thay đổi
        QuestManager.OnQuestUpdated += UpdateQuestIcon;
    }

    void OnDestroy()
    {
        //Hủy đăng ký sự kiện khi NPC bị xóa
        QuestManager.OnQuestUpdated -= UpdateQuestIcon;
    }

     protected virtual void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (npcData.dialogue != null)
                DialogueManager.Instance.StartDialogue(npcData.dialogue, npcData, this);
        }
    }

    public void UpdateQuestIcon()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (questAvailableIcon != null) questAvailableIcon.SetActive(false);
        if (questTurnInIcon != null) questTurnInIcon.SetActive(false);

        var qm = QuestManager.Instance;
        if (npcData.quest == null) return;

        //  Nếu chưa nhận nhiệm vụ
        if (qm.GetState() == QuestState.NotStarted && npcData == npcData.quest.giverNPC)
        {
            questAvailableIcon.SetActive(true);
            blinkCoroutine = StartCoroutine(BlinkIcon(questAvailableIcon));
        }
    
        else if (qm.CanReport(npcData.npcName))
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
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
