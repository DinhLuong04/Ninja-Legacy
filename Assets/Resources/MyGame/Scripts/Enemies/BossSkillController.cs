using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossSkillController : MonoBehaviour
{
    [Header("Boss Skill Settings")]
    public GameObject[] skillPrefabs;
    public float[] skillCooldowns; 

    private float[] lastSkillTimes; // lưu lần dùng gần nhất
    private Enemy enemyScript;
    private Animator anim;

    public void Init(Enemy enemy)
    {
        enemyScript = enemy;
        anim = enemy.GetComponent<Animator>();
        lastSkillTimes = new float[skillPrefabs.Length];
        for (int i = 0; i < lastSkillTimes.Length; i++)
            lastSkillTimes[i] = -999f; // đảm bảo có thể dùng ngay từ đầu
    }

    public bool HasAvailableSkill()
    {
        for (int i = 0; i < skillPrefabs.Length; i++)
        {
            if (Time.time >= lastSkillTimes[i] + skillCooldowns[i])
                return true;
        }
        return false;
    }

    public int GetAvailableSkillId()
    {
        // Lấy danh sách skill sẵn sàng
        var available = new List<int>();
        for (int i = 0; i < skillPrefabs.Length; i++)
        {
            if (Time.time >= lastSkillTimes[i] + skillCooldowns[i])
                available.Add(i);
        }

        if (available.Count == 0) return -1;
        return available[Random.Range(0, available.Count)];
    }

    public void MarkSkillUsed(int skillId)
    {
        if (skillId >= 0 && skillId < lastSkillTimes.Length)
            lastSkillTimes[skillId] = Time.time;
    }
  
}