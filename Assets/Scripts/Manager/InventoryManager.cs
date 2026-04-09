using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private GameData GameData = new GameData();
    public static InventoryManager instance;
    public Dictionary<int,(Items, int)> itemsLv;
    public Items currentItemSelected;
    public Inventory currentInventorySelected;
    private void Awake()
    {
        instance = this;
        itemsLv = GameData.FindItemsWithLvofPlayer();
    }
}
