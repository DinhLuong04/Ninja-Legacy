using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public float charDelay = 0.05f; 
    public float lineDelay = 2f;    

    
    public IEnumerator ShowText(string line)
    {
        storyText.text = "";
        foreach (char c in line)
        {
            storyText.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        yield return new WaitForSeconds(lineDelay);
    }

 
    public IEnumerator ShowStory(string[] lines)
    {
        foreach (string line in lines)
        {
            yield return StartCoroutine(ShowText(line));
        }
    }
}
