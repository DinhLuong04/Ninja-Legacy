using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public GameObject shopPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (shopPanel == null)
                shopPanel = GameObject.Find("Panel_NPCshop");

            if (shopPanel == null)
                Debug.LogError("Không tìm thấy Panel_NPCshop!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenShop() => shopPanel?.SetActive(true);
    public void CloseShop() => shopPanel?.SetActive(false);
}
