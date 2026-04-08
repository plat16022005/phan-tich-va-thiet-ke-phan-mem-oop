using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private Characters characters;
    private GameData GameData = new GameData();
    private void Awake()
    {
        instance = this;
        characters = PlayerManager.instance.characters;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(characters.id);
        }
    }
    public void AddReward(int rewards)
    {
        characters.gold += rewards;
        Debug.Log(characters.id);
        GameData.UpdateGold(characters.gold, characters.id);
    }
}
