using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }
}
