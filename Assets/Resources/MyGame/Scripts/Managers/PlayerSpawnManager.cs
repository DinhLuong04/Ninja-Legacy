// Assets/Scripts/PlayerSpawnManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;

    private string nextSceneName;
    private string nextSpawnKey;
    private Vector2 nextSpawnPosition;
    private bool hasSpawnData = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SetNextSpawn(string sceneName, string spawnKey, Vector2 position)
    {
        if (Instance == null) return;

        Instance.nextSceneName = sceneName;
        Instance.nextSpawnKey = spawnKey;
        Instance.nextSpawnPosition = position;
        Instance.hasSpawnData = true;

        Debug.Log($"[Spawn] Sẽ spawn tại {position} khi vào {sceneName} (key: {spawnKey})");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!hasSpawnData || scene.name != nextSceneName) return;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = nextSpawnPosition;
            Debug.Log($"[Spawn] Player xuất hiện tại {nextSpawnPosition} ({nextSpawnKey})");
        }

        hasSpawnData = false; // Reset
    }
}