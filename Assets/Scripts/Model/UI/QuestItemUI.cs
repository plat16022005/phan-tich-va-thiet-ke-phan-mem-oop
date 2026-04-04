using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItemUI : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    private Quests questData;

    public void Init(Quests quest)
    {
        questData = quest;
        txtName.text = quest.NameQuest;

        GetComponent<Button>().onClick.AddListener(() => questData.ChoiceQuest(quest.id));
    }

    // void OnClick()
    // {
    //     UI.instance.DisplayQuestInfo(questData);
    // }
}