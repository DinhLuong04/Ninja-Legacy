using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuidePanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelGuide;          // Panel_Guide
    public Button closeButton;             // CloseInventory
    private bool isOpen = false;

    private void Start()
    {
        if (panelGuide != null)
            panelGuide.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseGuide);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (isOpen) CloseGuide();
            else OpenGuide();
        }
    }

    public void OpenGuide()
    {
        if (panelGuide == null) return;

        panelGuide.SetActive(true);
        isOpen = true;

        // Báo về TutorialManager nếu cần
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnHelpOpened();
    }

    public void CloseGuide()
    {
        if (panelGuide == null) return;

        panelGuide.SetActive(false);
        isOpen = false;
    }

}
