using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    [Header("Quest")]
    public GameObject PanelListQuest;
    public Transform ContentQuest;
    public GameObject QuestsItemPrefab;
    [Header("Quest Infomation")]
    public GameObject PanelQuestInfo;
    public TextMeshProUGUI NameQuestInfo;
    public TextMeshProUGUI ContentQuestInfo;
    public Button ButtonFollow;
    public Button CancelQuest;
    public Button ViewQuest;
    public Button CompleteQuest;
    public TextMeshProUGUI Process;
    private Characters characters;
    [Header("Location")]
    public GameObject PanelLocation;
    public TextMeshProUGUI Location;
    [Header("Message")]
    public GameObject PanelMessage;
    public TextMeshProUGUI Message;
    [Header("List Dungeon")]
    public GameObject PanelListDungeon;
    public Transform ContentDungeon;
    public GameObject DungeonPrefabs;
    [Header("Panel Display Rewards")]
    public GameObject PanelDisplayRewards;
    public TextMeshProUGUI Rewards;
    [Header("Icon")]
    public Button IconQuest;
    public Button IconDungeon;
    public Button LeaveDungeon;
    public Button IconEnchance;
    [Header("Panel Enchance")]
    public GameObject PanelEnchance;
    public Transform ContentEnchance;
    public GameObject InventoryPrefabs;
    public TextMeshProUGUI NameItems;
    public TextMeshProUGUI CurrentLv;
    public TextMeshProUGUI CurrentStatsItem;
    public TextMeshProUGUI NextLv;
    public TextMeshProUGUI NextStatsItem;
    public Image[] imgWeapon;
    public Button EnhanceButton;
    [Header("Panel Stats")]
    public GameObject PanelStats;
    public TextMeshProUGUI HpPoint;
    public TextMeshProUGUI ManaPoint;
    public TextMeshProUGUI AtkPoint;
    public TextMeshProUGUI DefPoint;
    public TextMeshProUGUI Point;
    private void Awake()
    {
        instance = this;

        characters = PlayerManager.instance.characters;
    }
    public void ClosePanel(string typepanel)
    {
        if (typepanel == "Panel List quest")
            PanelListQuest.gameObject.SetActive(false);
        else if (typepanel == "Panel Quest info")
            PanelQuestInfo.gameObject.SetActive(false);
        else if (typepanel == "Panel Location")
            PanelLocation.gameObject.SetActive(false);
        else if (typepanel == "Panel Message")
            PanelMessage.gameObject.SetActive(false);
        else if (typepanel == "Panel List dungeon")
        {
            PanelListDungeon.gameObject.SetActive(false);
        }  
        else if (typepanel == "Panel Display Rewards")
            PanelDisplayRewards.gameObject.SetActive(false);
        else if (typepanel == "Panel Enchance")
            PanelEnchance.gameObject.SetActive(false);
        else if (typepanel == "Panel Stats")
            PanelStats.gameObject.SetActive(false);
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
    public void DisplayDetailQuest(QuestContent q)
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
                CompleteQuest.gameObject.SetActive(true);
                CancelQuest.gameObject.SetActive(true);
                CompleteQuest.onClick.AddListener(() => characters.ResultQuest(q.id_quest));
                CancelQuest.onClick.AddListener(Quests.instance.CancelQuest);
                Process.gameObject.SetActive(true);
                Process.text = $"{QuestManager.instance.playerProcess[NetworkManager.Singleton.LocalClientId]}/{q.required}";
            }
            else
            {
                QuestManager.instance.CurrentQuest = 0;
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
        Process.gameObject.SetActive(false);
    }
    public void DisplayLocation(string local)
    {
        PanelQuestInfo.gameObject.SetActive(false);
        PanelListQuest.gameObject.SetActive(false);
        PanelLocation.gameObject.SetActive(true);
        Location.text = local;
    }
    public void DisplayMessage(string message)
    {
        if (PanelQuestInfo != null && PanelListQuest != null && PanelLocation != null && PanelEnchance != null)
        {
            PanelQuestInfo.gameObject.SetActive(false);
            PanelListQuest.gameObject.SetActive(false);
            PanelLocation.gameObject.SetActive(false);
            PanelEnchance.gameObject.SetActive(false);
        }
        PanelMessage.gameObject.SetActive(true);
        Message.text = message;    
    }
    public void DisplayListDungeonn(List<Dungeon> ListDungeon)
    {
        PanelListDungeon.SetActive(true);
        foreach (Transform child in ContentDungeon)
        {
            Destroy(child.gameObject);
        }
        foreach (Dungeon dungeon in ListDungeon)
        {
            GameObject obj = Instantiate(DungeonPrefabs, ContentDungeon);
            DungeonItemUI ui = obj.GetComponent<DungeonItemUI>();
            ui.Init(dungeon);
        }
    }
    public void EnterDungeon()
    {
        IconQuest.gameObject.SetActive(false);
        IconDungeon.gameObject.SetActive(false);
        IconEnchance.gameObject.SetActive(false);
        LeaveDungeon.gameObject.SetActive(true);
    }
    public void ExitDungeon()
    {
        IconQuest.gameObject.SetActive(true);
        IconDungeon.gameObject.SetActive(true);
        IconEnchance.gameObject.SetActive(true);
        LeaveDungeon.gameObject.SetActive(false);
        GameObject player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
        Vector3 pos = new Vector3(-1.5f, 0.8f, 0.7f);
        player.transform.position = pos;
        ClosePanel("Panel Display Rewards");        
    }
    public void DisplayRewards(Rewards rewards)
    {
        PanelDisplayRewards.gameObject.SetActive(true);
        Rewards.text = $"{rewards.gold} vàng";
    }
    public void OpenEnhancement()
    {
        List<Inventory> inventory = Inventory.instance.GetEquipmentList();
        PanelEnchance.gameObject.SetActive(true);
        foreach (Transform child in ContentEnchance)
        {
            Destroy(child.gameObject);
        }
        foreach (Inventory i in inventory)
        {
            GameObject obj = Instantiate(InventoryPrefabs, ContentEnchance);
            InventoryItemUI ui = obj.GetComponent<InventoryItemUI>();
            ui.Init(i);
        }
    }
    private Items selectedItems;
    public void SeeItemInfo(int EquipmentId)
    {
        selectedItems = Inventory.instance.GetItem(EquipmentId);
        InventoryManager.instance.currentItemSelected = selectedItems;
        EnhanceButton.onClick.AddListener(() => EnhanceItem(InventoryManager.instance.currentItemSelected));
        int CurrentLevel = selectedItems.GetLevel();
        int CurrentStats = selectedItems.GetBaseStats();
        Debug.Log(CurrentLevel + CurrentStats);
        DisplayItemDetails(CurrentLevel, CurrentStats);
    }
    public void DisplayItemDetails(int CurrentLevel, int CurrentStats)
    {
        imgWeapon[0].sprite = ItemsManager.instance.ImageItems[InventoryManager.instance.currentItemSelected.id];
        imgWeapon[1].sprite = ItemsManager.instance.ImageItems[InventoryManager.instance.currentItemSelected.id];
        NameItems.text = InventoryManager.instance.currentItemSelected.NameItem;
        CurrentLv.text = "LV: " + CurrentLevel.ToString();
        CurrentStatsItem.text = "Tấn công: " + CurrentStats.ToString();
        NextLv.text = "LV: " + (CurrentLevel + 1).ToString();
        NextStatsItem.text = "Tấn công: " + (CurrentStats + 5).ToString();
    }
    public void EnhanceItem(Items selectedItem)
    {
        Inventory.instance.Enhance(selectedItem);
    }
    public void CloseEnhancement()
    {
        Close();
    }
    void Close()
    {
        PanelEnchance.gameObject.SetActive(false);
    }
    public void OpenTradeForm()
    {
        Network.instance.RequestPlayerList();
    }
    public void ShowListPlayer(int[] clientIds, int[] playerIds)
    {
        for (int i = 0; i < clientIds.Length; i++)
        {
            Debug.Log("ClientID: " + clientIds[i] + " PlayerID: " + playerIds[i]);
        }
    }
    public void OpenCharacterStats()
    {
        characters.LoadStatsAndPotentialPoints();
    }
    public void DisplayStats(int[] stats, int point)
    {
        PanelStats.gameObject.SetActive(true);
        Point.text = point.ToString();
        HpPoint.text = stats[0].ToString();
        ManaPoint.text = stats[1].ToString();
        AtkPoint.text = stats[2].ToString();
        DefPoint.text = stats[3].ToString();
    }
    public void IncreaseStatTemp(string TempType)
    {
        characters.AddStatPreview(TempType);
    }
    public void UpdatePreviewDisplay()
    {
        Point.text = StatsManager.instance.potentialPoint.ToString();
        HpPoint.text = StatsManager.instance.stats[0].ToString();
        ManaPoint.text = StatsManager.instance.stats[1].ToString();
        AtkPoint.text = StatsManager.instance.stats[2].ToString();
        DefPoint.text = StatsManager.instance.stats[3].ToString();        
    }
    public void ConfirmStatsUpgrade()
    {
        characters.ApplyUpgrade();
    }
    public void ResetStatsTemp()
    {
        characters.CancelReview();
    }
    public void CreateNewCharacter()
    {
        Characters NewCharacter = new Characters();
        NewCharacter.CreatePlayer(
            AvatarController.instance.BoxName.text,
            AvatarController.instance.hairIndex, 
            AvatarController.instance.eyesIndex, 
            AvatarController.instance.noseIndex, 
            AvatarController.instance.mouthIndex, 
            AvatarController.instance.raceIndex, 
            AvatarController.instance.classIndex
        );
    }
}
