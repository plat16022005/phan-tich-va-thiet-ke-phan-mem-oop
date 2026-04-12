using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    public Resource(){
    }
    public int GetQuantity()
    {
        return PlayerManager.instance.characters.gold;
    }
    public void SetQuantity(int NewAmount)
    {
        PlayerManager.instance.characters.gold = NewAmount;
    }
}
