using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    private int id;
    private string NameQuest;
    private string content;
    private QuestType type;
    private int required;
    public void LoadQuest()
    {
        
    }
}

public enum QuestType
{
    COLLECT,
    BATTLE
}