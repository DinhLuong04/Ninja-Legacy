using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ExitGameUI : MonoBehaviour
{
    public GameObject panelExitGame;
    public Button buttonExitGame;
    public Button buttonYes;
    public Button buttonNo;

    void Start()
    {
        panelExitGame.SetActive(false);
        buttonExitGame.onClick.AddListener(OpenExitPanel);
        buttonYes.onClick.AddListener(ConfirmExit);
        buttonNo.onClick.AddListener(CloseExitPanel);
    }

    void OpenExitPanel()
    {
        panelExitGame.SetActive(true);
    }

    void CloseExitPanel()
    {
        panelExitGame.SetActive(false);
    }

    void ConfirmExit()
    {
        SceneManager.LoadScene("LobbyScene");  
        panelExitGame.SetActive(false);
    }
}
