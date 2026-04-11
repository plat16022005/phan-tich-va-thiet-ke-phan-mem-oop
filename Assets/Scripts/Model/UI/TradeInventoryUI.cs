using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeInventoryUI : MonoBehaviour
{
    public Image sprite;
    public TextMeshProUGUI txtLv;
    private Items itemData;
    private Inventory inventoryData;
    private GameData GameData = new GameData();
    public void Init(Inventory inventory)
    {
        inventoryData = inventory;
        sprite.sprite = ItemsManager.instance.ImageItems[inventoryData.id_item];
        txtLv.text = $"{GameData.FindLvEquipmentofPlayer(inventoryData.id).ToString()}";

        GetComponent<Button>().onClick.AddListener(() => GameUI.instance.SelectItem(inventoryData));
    }
}
