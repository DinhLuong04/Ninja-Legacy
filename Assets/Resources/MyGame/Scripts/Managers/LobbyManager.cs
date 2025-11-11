using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Button playButton;
    public Button exitButton;
    public AudioSource audioSource;
    public AudioClip clickSound;
    void Start()
    {
        // Gán sự kiện cho nút
        playButton.onClick.AddListener(OnPlayClicked);
        exitButton.onClick.AddListener(OnExitClicked);


    }

    void OnPlayClicked()
    {
       
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);

   
         StartCoroutine(LoadSceneAfterDelay("IntroStory", 0.2f));
    }

    void OnExitClicked()
    {
        
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);

        // Thoát game
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
{
    yield return new WaitForSeconds(delay);
    SceneManager.LoadScene(sceneName);
}
}
