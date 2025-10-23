using UnityEngine;
using UnityEngine.UI;
using TMPro; // Thêm namespace cho TextMeshPro
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

    void Start()
    {
        currentHP = maxHP;
        currentMP = maxMP;

        
        hpFill.color = Color.red; 
        mpFill.color = Color.blue; 

        UpdateUI();
    }
     private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
        maxHP += 20;
        maxMP += 10;
        currentHP = maxHP;
        currentMP = maxMP;
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
    private void UpdateUI()
    {
        
        hpFill.fillAmount = (float)currentHP / maxHP;
       
        mpFill.fillAmount = (float)currentMP / maxMP;
       
        hpText.text = currentHP + " / " + maxHP;
       
        mpText.text = currentMP + " / " + maxMP;
        
        levelText.text = "Lv " + level;
       
        float expPercentage = (float)exp / expToNextLevel * 100;
        percenLevelExps.text = expPercentage.ToString("F1") + "%";
        foreach (var text in goldTexts)
        {
            if (text != null)
                text.text = gold.ToString()+" Yên";
        }
    }
}