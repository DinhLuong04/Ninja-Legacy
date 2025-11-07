using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerStealth : MonoBehaviour
{
    [Header("Stealth Settings")]
    public bool isStealthed = false;
    public float manaDrainPerSecond = 5f;
    public KeyCode stealthKey = KeyCode.T;

    private PlayerStats stats;
    private PlayerCombat combat;
    private Animator anim;
    private SpriteRenderer sr;
    private Coroutine stealthRoutine;

    private Color originalColor;
    [Range(0f, 1f)] public float stealthTransparency = 0.5f;

    void Start()
    {
        stats = PlayerStats.Instance;
        combat = GetComponent<PlayerCombat>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        originalColor = sr.color;
    }
     void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Update()
    {
        // Bật/Tắt tàng hình
        if (Input.GetKeyDown(stealthKey))
        {
            if (!isStealthed)
                ActivateStealth();
            else
                DeactivateStealth();
        }

        // Nếu đang tàng hình mà tấn công -> lộ
        if (isStealthed && Input.GetKeyDown(KeyCode.Space))
        {
            DeactivateStealth();
        }
    }

    public void ActivateStealth()
    {
        if (isStealthed || stats.currentMP <= 0) return;

        isStealthed = true;
        anim.SetBool("isStealth", true);

        // Làm mờ nhân vật để dễ nhìn
        Color c = sr.color;
        c.a = stealthTransparency;
        sr.color = c;

        // Vô hiệu hóa combat (tạm thời)
        combat.enabled = false;

        stealthRoutine = StartCoroutine(StealthManaDrain());
    }

    public void DeactivateStealth()
    {
        if (!isStealthed) return;

        isStealthed = false;
        anim.SetBool("isStealth", false);

        // Trả lại màu bình thường
        sr.color = originalColor;

        // Cho phép tấn công lại
        combat.enabled = true;

        if (stealthRoutine != null)
            StopCoroutine(stealthRoutine);
    }

    private IEnumerator StealthManaDrain()
    {
        while (isStealthed)
        {
            if (stats.currentMP <= 0)
            {
                DeactivateStealth();
                yield break;
            }

            stats.UseMana(Mathf.RoundToInt(manaDrainPerSecond));
            yield return new WaitForSeconds(1f);
        }
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cập nhật lại các reference trong Player mới
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            anim = player.GetComponent<Animator>();
            sr = player.GetComponent<SpriteRenderer>();
            combat = player.GetComponent<PlayerCombat>();
        }
    }
}
