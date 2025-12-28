using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    public TextMeshProUGUI tutorialText;

    [Header("Settings")]
    public int enemiesToKill = 3;
    private int enemiesKilled = 0;
    public int currentStep = 0;
    private bool isTutorialActive = false;

    [Header("References")]
    public Transform tutorialNPC; // Hanzo
    public Transform shopNPC; // NPC bán đồ
    public GameObject inventoryPanel; 

    private bool hasBoughtHP = false;
    private bool hasBoughtMP = false;
    private bool hasUsedHP = false;
    private bool hasUsedMP = false;
    private bool hasSwitchedWeapon = false;
    private bool shopOpened = false;
    public bool OpenInventory = false;
    private bool helpOpened = false; 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(IntroDialogueSequence());
    }

    IEnumerator IntroDialogueSequence()
    {
        isTutorialActive = false;
        
        yield return ShowDialogue("Chào con, ta sẽ nhắc lại lý thuyết trước khi luyện tập.", 3f);
        yield return ShowDialogue("HP là sinh lực, MP là năng lượng cho kỹ năng tàng hình và tấn công .", 3f);
        yield return ShowDialogue("Phi tiêu sẽ bay theo hướng chuột.", 3f);
        yield return ShowDialogue("Khi chiến đấu, tiêu diệt quái sẽ nhận EXP để tăng level và chỉ số.", 3f);
        yield return ShowDialogue("Khi tiêu diệt quái con có thể nhận được vật phẩm rơi ra như bình HP, MP,Yên để con mua đồ", 3f);
        yield return ShowDialogue("Hành trang chứa vật phẩm như bình HP, MP và các vật phẩm hỗ trợ.", 3f);
        yield return ShowDialogue("Bây giờ ta sẽ kiểm tra kỹ năng thực hành của con!", 3f);

        StartTutorial();
    }

    IEnumerator ShowDialogue(string message, float duration)
    {
        DialogueManager.Instance.ShowTutorialDialogue(message, duration);
        yield return new WaitForSeconds(duration + 0.2f);
    }

    public void StartTutorial()
    {
        isTutorialActive = true;
        currentStep = 0;
        enemiesKilled = 0;
        hasBoughtHP = hasBoughtMP = hasUsedHP = hasUsedMP = hasSwitchedWeapon = false;
        ShowStep();
    }

    private void Update()
    {
        if (!isTutorialActive) return;

        switch (currentStep)
        {
            case 0: // Di chuyển
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    NextStep();
                break;

            case 1: // Nhảy
                if (Input.GetKeyDown(KeyCode.W))
                    NextStep();
                break;

            case 2: // Nói chuyện với Hanzo
                if (IsPlayerNearNPC(tutorialNPC) && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(ShowDialogueSequence(new string[]
                    {
                        "Tốt lắm, khởi động cơ bản hoàn tất.",
                        "Giờ ta sẽ kiểm tra khả năng chiến đấu của con."
                    }));
                    NextStep();
                }
                break;

            case 3: // Tấn công
                if (Input.GetKeyDown(KeyCode.Space))
                    NextStep();
                break;

            case 4: // Đổi vũ khí
                if (hasSwitchedWeapon)
                    NextStep();
                break;

            case 5: // Tàng hình
                if (Input.GetKeyDown(KeyCode.T))
                    NextStep();
                break;

            case 6: // Giết quái
                break;

            case 7: // Mở shop
                if (IsPlayerNearNPC(tutorialNPC) && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(ShowDialogueSequence(new string[]
                    {
                        "Chiến đấu tốt lắm!",
                        "Giờ hãy học cách hồi phục và chuẩn bị cho chiến trường thật.",
                        "Hãy đến gặp Chủ tạp hóa để mua vật phẩm."
                    }));
                    NextStep();
                }
                break;

            case 8: // Mở shop
                if (shopOpened)
                    NextStep();
                break;

            case 9: // Mua bình HP + MP
                if (hasBoughtHP && hasBoughtMP)
                    NextStep();
                break;



            case 10: // Mở hành trang
                if (OpenInventory)
                    NextStep();
                break;
            case 11: // Dùng vật phẩm HP + MP
                if (hasUsedHP && hasUsedMP)
                    NextStep();
                break;

            case 12: // Báo cáo Hanzo
                if (IsPlayerNearNPC(tutorialNPC) && Input.GetKeyDown(KeyCode.E))
                {
                    ShowHanzoDialogue("Tốt lắm! Con đã sẵn sàng. Trước khi rời đi, hãy mở bảng hướng dẫn để xem lại điều khiển nhé!");
                    NextStep();
                }
                break;

            case 13: // Mở bảng hướng dẫn (step cuối)
                if (helpOpened)
                    NextStep();
                break;
        }
    }

    void ShowStep()
    {
        switch (currentStep)
        {
            case 0: tutorialText.text = "Dùng A/D để di chuyển"; break;
            case 1: tutorialText.text = "Nhấn W để nhảy"; break;
            case 2: tutorialText.text = "Đến gặp Hanzo (E) để nói chuyện"; break;
            case 3: tutorialText.text = "Nhấn SPACE để tấn công"; break;
            case 4: tutorialText.text = "Nhấn Q để đổi vũ khí"; break;
            case 5: tutorialText.text = "Nhấn T để tàng hình"; break;
            case 6: tutorialText.text = $"Tiêu diệt {enemiesKilled}/{enemiesToKill} quái vật"; break;
            case 7: tutorialText.text = "Quay lại gặp Hanzo (E) "; break;
            case 8: tutorialText.text = "Đến gặp chủ tạp hóa (E) để mở cửa hàng"; break;
            case 9: tutorialText.text = "Mua 1 bình HP và 1 bình MP"; break;
            case 10: tutorialText.text = "Nhấn I hoặc nút túi để mở hành trang"; break;
            case 11: tutorialText.text = "Dùng bình HP và bình MP để hồi phục"; break;
            case 12: tutorialText.text = "Quay lại gặp Hanzo (E) để hoàn thành huấn luyện"; break;
            case 13: tutorialText.text = "Nhấn H hoặc nút 'Hướng Dẫn' để mở bảng hướng dẫn"; break;
        }
    }

    void NextStep()
    {
        currentStep++;
        if (currentStep > 13)
        {
            tutorialText.text = " Hoàn thành huấn luyện! Chuẩn bị ra về làng nhé!";
            isTutorialActive = false;
            StartCoroutine(EndTutorial());
        }
        else
        {
             StartCoroutine(ShowStepWithDelay(1.5f));
        }
    }
    IEnumerator ShowStepWithDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    ShowStep();
}
    public void OnEnemyKilled()
    {
        if (!isTutorialActive || currentStep != 6) return;

        enemiesKilled++;
        tutorialText.text = $"Tiêu diệt {enemiesKilled}/{enemiesToKill} quái vật";
        if (enemiesKilled >= enemiesToKill)
            NextStep();
    }

    public void NotifyItemBought(string itemName)
    {
        if (!isTutorialActive) return;

        if (itemName.Contains("Bình HP")) hasBoughtHP = true;
        else if (itemName.Contains("Bình MP")) hasBoughtMP = true;

        if (currentStep == 9 && hasBoughtHP && hasBoughtMP)
            NextStep();
    }

    public void NotifyItemUsed(ItemData item)
    {
        if (!isTutorialActive) return;

        if (item.itemName.Contains("Bình HP")) hasUsedHP = true;
        else if (item.itemName.Contains("Bình MP")) hasUsedMP = true;

        if (currentStep == 11 && hasUsedHP && hasUsedMP)
            NextStep();
    }

    public void NotifyWeaponSwitched()
    {
        if (currentStep == 4)
            NextStep();
    }

    public void OnShopOpened()
    {
        if (!isTutorialActive) return;

        shopOpened = true;
        if (currentStep == 8)
            NextStep();
    }
    public void OnHelpOpened()
{
    if (!isTutorialActive) return;

    helpOpened = true;

    if (currentStep == 13)
        NextStep();
}
    private bool IsPlayerNearNPC(Transform npc)
    {
        if (npc == null) return false;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;
        return Vector2.Distance(player.transform.position, npc.position) <= 2f;
    }

    void ShowHanzoDialogue(string message)
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowTutorialDialogue(message, 3f);
        else
            Debug.Log(message);
    }

    IEnumerator ShowDialogueSequence(string[] messages)
    {
        foreach (string msg in messages)
        {
            DialogueManager.Instance.ShowTutorialDialogue(msg, 2.5f);
            yield return new WaitForSeconds(2.7f);
        }
    }

    private IEnumerator EndTutorial()
    {
        yield return new WaitForSeconds(3f);
        if (PlayerStats.Instance != null) PlayerStats.Instance.ResetForRealGame();
        if (BuffPanelManager.Instance != null) BuffPanelManager.Instance.ClearAllBuffs();
        if (InventoryManager.Instance != null) InventoryManager.Instance.ResetForRealGame();
        if (tutorialNPC != null) Destroy(tutorialNPC.gameObject);
        if (shopNPC != null) Destroy(shopNPC.gameObject); 
        SceneManager.LoadScene("Level1");
    }
}
