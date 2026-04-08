using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;
    public int CurrentDungeon;
    public Transform SpawnPlayer;
    public Transform SpawnMonster;
    public int process;
    public Transform SpawnMap;
    public List<GameObject> DungeonMap;
    public List<GameObject> ListMonster;
    private void Awake()
    {
        instance = this;
    }
}
