using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
[CreateAssetMenu(fileName = "NPCData", menuName = "Game/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public Sprite portrait;
    public DialogueData dialogue;
    public QuestData quest;
}

