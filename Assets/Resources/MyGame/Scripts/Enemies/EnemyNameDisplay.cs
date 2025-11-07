using UnityEngine;
using TMPro;

public class EnemyNameDisplay : MonoBehaviour
{
    [Header("References")]
    public TMP_Text nameText;      // Gán từ Inspector
    public string enemyName = "Enemy";

    private void Start()
    {
        if (nameText != null)
            nameText.text = enemyName;
    }
}
