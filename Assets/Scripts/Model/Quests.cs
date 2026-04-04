using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Quests : MonoBehaviour
{
    public int id;
    public string NameQuest;
    public QuestType TypeQuest;
    private QuestRepository QuestRepository = new QuestRepositoryImpl();
    private QuestContentRepository QuestContentRepository = new QuestContentRepositoryImpl();
    private int CurrentQuest;
    public static Quests instance;
    private void Awake()
    {
        instance = this;
    }
    public void LoadQuest()
    {
        List<Quests> ListQuest = QuestRepository.FindAllQuest();
        UI.instance.DisplayListQuest(ListQuest);
    }
    public void ChoiceQuest(int QuestId)
    {
        QuestContent quest = QuestContentRepository.ViewDetail(QuestId);
        UI.instance.DisplayQuestInfo(quest);
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
        return QuestManager.instance.GetLocation(QuestId);
    }
}

public enum QuestType
{
    COLLECT = 0,
    BATTLE = 1
}