using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;
    [Header("Quest")]
    public GameObject PanelListQuest;
    public Transform ContentQuest;
    public GameObject QuestsItemPrefab;
    [Header("Quest Infomation")]
    public GameObject PanelQuestInfo;
    public TextMeshProUGUI NameQuestInfo;
    public TextMeshProUGUI ContentQuestInfo;
    public Button ButtonFollow;
    private CharactersRepository CharactersRepository;
    public Button CancelQuest;
    public Button ViewQuest;
    public Button CompleteQuest;
    private Characters characters;
    [Header("Location")]
    public GameObject PanelLocation;
    public TextMeshProUGUI Location;
    private void Awake()
    {
        instance = this;

        CharactersRepository = new CharactersRepositoryImpl();
        characters = CharactersRepository.GetCharacterByAccountId(SessionManager.Instance.account.id);
    }
    public void ClosePanel(string typepanel)
    {
        if (typepanel == "Panel List quest")
            PanelListQuest.gameObject.SetActive(false);
        else if (typepanel == "Panel Quest info")
            PanelQuestInfo.gameObject.SetActive(false);
        else if (typepanel == "Panel Location")
            PanelLocation.gameObject.SetActive(false);
    }
    public void DisplayListQuest(List<Quests> quests)
    {
        PanelListQuest.gameObject.SetActive(true);
        foreach (Transform child in ContentQuest)
        {
            Destroy(child.gameObject);
        }
        foreach (Quests q in quests)
        {
            GameObject item = Instantiate(QuestsItemPrefab, ContentQuest);
            QuestItemUI ui = item.GetComponent<QuestItemUI>();
            ui.Init(q);
        }
    }
    public void DisplayQuestInfo(QuestContent q)
    {
        ResetPanelQuestInfo();
        PanelQuestInfo.gameObject.SetActive(true);
        NameQuestInfo.text = q.NameQuest;
        ContentQuestInfo.text = q.content;
        ButtonFollow.onClick.AddListener(() => characters.StartQuest(q.id_quest));
        if (q.id_quest == QuestManager.instance.CurrentQuest)
        {
            ButtonFollow.gameObject.SetActive(false);
            ViewQuest.gameObject.SetActive(true);
            if (QuestManager.instance.State == "Đang thực hiện")
            {
                CancelQuest.gameObject.SetActive(true);
            }
            else
            {
                CancelQuest.gameObject.SetActive(false);
                CompleteQuest.gameObject.SetActive(true);
            }
        }
    }
    public void ResetPanelQuestInfo()
    {
        NameQuestInfo.text = "";
        ContentQuestInfo.text = "";
        ButtonFollow.gameObject.SetActive(true);
        CancelQuest.gameObject.SetActive(false);
        ViewQuest.gameObject.SetActive(false);
        CompleteQuest.gameObject.SetActive(false);
    }
    public void DisplayLocation(string local)
    {
        PanelQuestInfo.gameObject.SetActive(false);
        PanelListQuest.gameObject.SetActive(false);
        PanelLocation.gameObject.SetActive(true);
        Location.text = local;
    }
}
