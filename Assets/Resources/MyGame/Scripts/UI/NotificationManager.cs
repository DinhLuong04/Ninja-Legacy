using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text messageText;
    public float displayDuration = 2f; // thời gian hiển thị

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        panel.SetActive(false);
    }

    public void Show(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        panel.SetActive(true);
        messageText.text = message;

        yield return new WaitForSeconds(displayDuration);

        panel.SetActive(false);
        currentRoutine = null;
    }
}
