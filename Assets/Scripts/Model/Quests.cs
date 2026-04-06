using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Quests : MonoBehaviour
{
    private GameData GameData = new GameData();
    public Quests()
    {
        
    }
    public int id;
    public string NameQuest;
    public QuestType TypeQuest;
    private QuestRepository QuestRepository = new QuestRepositoryImpl();
    private QuestContentRepository QuestContentRepository = new QuestContentRepositoryImpl();
    public static Quests instance;
    private void Awake()
    {
        instance = this;
    }
    public void LoadQuest()
    {
        List<Quests> ListQuest = GameData.FindAllQuest();
        GameUI.instance.DisplayListQuest(ListQuest);
    }
    public void ChoiceQuest(int QuestId)
    {
        QuestContent quest = GameData.ViewDetail(QuestId);
        GameUI.instance.DisplayDetailQuest(quest);
    }
    public void Track(int QuestId)
    {
        QuestManager.instance.CurrentQuest = QuestId;
    }
    public void UpdateState(string state)
    {
        QuestManager.instance.State = state;
    }
    public string GetLocation(int QuestId)
    {
        return QuestManager.instance.location[QuestId];
    }
    public int GetReward(int QuestId)
    {
        return QuestManager.instance.rewards[QuestId];
    }
    public bool CheckConditions(int QuestId)
    {
        QuestContent quest = GameData.ViewDetail(QuestId);
        if (QuestManager.instance.playerProcess[NetworkManager.Singleton.LocalClientId] >= quest.required)
        {
            return true;
        }
        return false;
    }
    public void CancelQuest()
    {
        UpdateState("Thất bại");
        GameUI.instance.DisplayMessage("Bạn đã thất bại");        
    }
}

public enum QuestType
{
    COLLECT = 0,
    BATTLE = 1
}