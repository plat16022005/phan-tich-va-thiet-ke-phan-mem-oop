using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonItemUI : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    private Dungeon DungeonData;

    public void Init(Dungeon dungeon)
    {
        DungeonData = dungeon;
        txtName.text = $"{dungeon.NameDungeon} - LV: {dungeon.required}";
        Debug.Log(PlayerManager.instance.characters.id);
        GetComponent<Button>().onClick.AddListener(() => DungeonData.StartDungeon(dungeon, PlayerManager.instance.characters));
    }
}
