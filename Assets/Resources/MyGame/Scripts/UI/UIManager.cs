using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        // Nếu đã có Instance thì hủy bản này để tránh duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Giữ Canvas này khi load scene khác
        DontDestroyOnLoad(gameObject);
    }
}
