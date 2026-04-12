using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private Resource goldRes = new Resource();
    private Characters characters;
    private List<Items> items;
    private GameData GameData = new GameData();
    public Inventory(){
    }
    public int id;
    public int id_player;
    public int id_item;

    private void Awake()
    {
        instance = this;
        characters = PlayerManager.instance.characters;
        
    }
    public void AddReward(int rewards)
    {
        characters.gold += rewards;
        Debug.Log(characters.id);
        GameData.UpdateGold(characters.gold, characters.id);
    }
    public List<Inventory> GetEquipmentList()
    {
        return GameData.GetInventoryData();
    }
    public Items GetItem(int EquipmentId)
    {
        return InventoryManager.instance.itemsLv[EquipmentId].Item1;
    }
    
    public void Enhance(Items selectedItem)
    {
        int currentGoldQuantity = goldRes.GetQuantity();
        Debug.Log(currentGoldQuantity);
        if (currentGoldQuantity >= EnchanceManager.instance.RequiredQuantity)
        {
            int NewAmount = currentGoldQuantity - EnchanceManager.instance.RequiredQuantity;
            goldRes.SetQuantity(NewAmount);
            GameData.SaveProgress(selectedItem, goldRes);
            GameUI.instance.DisplayMessage("Cường hóa thành công");
        }
        else
        {
            GameUI.instance.DisplayMessage("Cường hóa thất bại vì không đủ tài nguyên");
        }
    }
}
