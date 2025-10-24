using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stats")]
    public int level = 1;
    public int exp = 0;
    public int expToNextLevel = 100;

    public int maxHP = 100;
    public int currentHP;

    public int maxMP = 50;
    public int currentMP;

    [Header("Base Stats (for Buff)")]
    public int baseMaxHP;
    public int baseMaxMP;
    public int baseDamage = 10;
    public int currentDamage;

    [Header("Economy")]
    public int gold = 500;
    public List<TextMeshProUGUI> goldTexts = new List<TextMeshProUGUI>();

    [Header("UI")]
    public Image hpFill;
    public Image mpFill;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI percenLevelExps;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        baseMaxHP = maxHP;
        baseMaxMP = maxMP;
        currentHP = maxHP;
        currentMP = maxMP;
        currentDamage = baseDamage;

        hpFill.color = Color.red;
        mpFill.color = Color.blue;

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateUI();
    }

    public void RestoreMana(int amount)
    {
        currentMP += amount;
        if (currentMP > maxMP) currentMP = maxMP;
        UpdateUI();
    }

    public void UseMana(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            UpdateUI();
        }
    }

    public void GainExp(int amount)
    {
        exp += amount;
        if (exp >= expToNextLevel)
        {
            exp -= expToNextLevel;
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        expToNextLevel += 50;
        baseMaxHP += 20;
        baseMaxMP += 10;

        maxHP = baseMaxHP;
        maxMP = baseMaxMP;
        currentHP = maxHP;
        currentMP = maxMP;

        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateUI();
            return true;
        }
        Debug.Log("Không đủ tiền!");
        return false;
    }

    public void UpdateUI()
    {
        hpFill.fillAmount = (float)currentHP / maxHP;
        mpFill.fillAmount = (float)currentMP / maxMP;
        hpText.text = $"{currentHP} / {maxHP}";
        mpText.text = $"{currentMP} / {maxMP}";
        levelText.text = "Lv " + level;

        float expPercentage = (float)exp / expToNextLevel * 100;
        percenLevelExps.text = expPercentage.ToString("F1") + "%";

        foreach (var text in goldTexts)
        {
            if (text != null)
                text.text = gold.ToString() + " Yên";
        }
    }

    //  Tính lại chỉ số dựa theo buff hiện có
    public void RecalculateFromBuffs()
    {
        float hpMul = 1f;
        float mpMul = 1f;
        float dmgMul = 1f;

        if (BuffPanelManager.Instance != null)
        {
            var buffs = BuffPanelManager.Instance.GetActiveBuffs();
            foreach (var buff in buffs)
            {
                hpMul *= buff.hpMultiplier;
                mpMul *= buff.mpMultiplier;
                dmgMul *= buff.damageMultiplier;
            }
        }

        maxHP = Mathf.RoundToInt(baseMaxHP * hpMul);
        maxMP = Mathf.RoundToInt(baseMaxMP * mpMul);
        currentDamage = Mathf.RoundToInt(baseDamage * dmgMul);

        if (currentHP > maxHP) currentHP = maxHP;
        if (currentMP > maxMP) currentMP = maxMP;

        UpdateUI();
    }
}
