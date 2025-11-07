using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    public TMP_Text npcNameText;
    public TMP_Text dialogueText;
    public Image npcPortrait;
    public Button nextButton;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowDialogue(string npcName, string dialogueLine, Sprite portrait = null)
    {
        npcNameText.text = npcName;
        dialogueText.text = dialogueLine;

        if (portrait != null) npcPortrait.sprite = portrait;

        gameObject.SetActive(true);
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
    }
}
