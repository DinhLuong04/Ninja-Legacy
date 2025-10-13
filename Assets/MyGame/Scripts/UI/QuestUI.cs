using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;
    public TMP_Text questProgressText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowQuest(string name, string description, string progress)
    {
        questNameText.text = name;
        questDescriptionText.text = description;
        questProgressText.text = progress;
        gameObject.SetActive(true);
    }

    public void HideQuest()
    {
        gameObject.SetActive(false);
    }
}
