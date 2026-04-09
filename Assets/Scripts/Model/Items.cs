using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public int id;
    public string NameItem;
    public int GetLevel()
    {
        return InventoryManager.instance.itemsLv[id].Item2;
    }
    public int GetBaseStats()
    {
        return 20 + InventoryManager.instance.itemsLv[id].Item2 * 5;
    }
    public void UpgradeLevel()
    {
        UpdateStats();
    }
    public void UpdateStats()
    {
        int id = InventoryManager.instance.currentItemSelected.id;

        if (!InventoryManager.instance.itemsLv.ContainsKey(id))
        {
            Debug.LogError("Item not found in dictionary");
            return;
        }

        var currentData = InventoryManager.instance.itemsLv[id];

        InventoryManager.instance.itemsLv[id] = (currentData.Item1, currentData.Item2 + 1);
    }
}
