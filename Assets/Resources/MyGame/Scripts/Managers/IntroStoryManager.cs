using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroStoryManager : MonoBehaviour
{
    public TypewriterEffect typewriter;
    public string[] storyLines;        // các câu story
    public string nextSceneName = "Tutorial"; // scene tutorial

    void Start()
    {
        StartCoroutine(PlayStory());
    }

    IEnumerator PlayStory()
    {
        yield return StartCoroutine(typewriter.ShowStory(storyLines));
        SceneManager.LoadScene(nextSceneName);
    }
}
