using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopDialogueManager : MonoBehaviour
{
    public static ShopDialogueManager Instance;

    [SerializeField] private GameObject shopDialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button closeButton;

    private NPC_Shopkeeper currentNpc;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopDialoguePanel.SetActive(false);

        shopButton.onClick.AddListener(OnShop);
        closeButton.onClick.AddListener(CloseDialogue);
    }

    public void StartShopDialogue(NPCData npcData, NPC_Shopkeeper npc)
    {
        currentNpc = npc;

        shopDialoguePanel.SetActive(true);
        npcNameText.text = npcData.npcName;
        npcPortrait.sprite = npcData.portrait;
        dialogueText.text = "Xin chào! Bạn muốn xem hàng không?";
    }

    private void OnShop()
    {
        shopDialoguePanel.SetActive(false);
        currentNpc?.OpenShop();
    }

    public void CloseDialogue()
    {
        shopDialoguePanel.SetActive(false);
    }
}
