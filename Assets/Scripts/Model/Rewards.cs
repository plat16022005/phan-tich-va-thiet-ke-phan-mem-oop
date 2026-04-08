using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    public static Rewards instance;
    public Rewards()
    {
        
    }
    public int id;
    public int id_dungeon;
    public int gold;
    private void Awake()
    {
        instance = this;
    }
    public void LoadReward()
    {
        GameData GameData = new GameData();
        Rewards rewards = GameData.FindRewardofDungeon(DungeonManager.instance.CurrentDungeon);
        GameUI.instance.DisplayRewards(rewards);
        GameData.UpdateGold(PlayerManager.instance.characters.gold + rewards.gold, PlayerManager.instance.characters.id);
    }
}
