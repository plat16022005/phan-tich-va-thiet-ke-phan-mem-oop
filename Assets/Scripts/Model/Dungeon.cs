using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public static Dungeon instance;
    private Characters characters;
    private Monster monster;
    private Rewards rewards;
    public Dungeon(){
    }
    public int id;
    public string NameDungeon;
    public int required;
    private void Awake()
    {
        instance = this;
        characters = PlayerManager.instance.characters;
    }
    public void LoadDungeon()
    {
        GameData GameData = new GameData();
        List<Dungeon> ListDungeon = GameData.FindAllDungeon();
        GameUI.instance.DisplayListDungeonn(ListDungeon);
    }
    public void StartDungeon(Dungeon dungeon, Characters characters)
    {
        
        if (dungeon.required <= characters.level)
        {
            characters.UpdateState(dungeon.id);
            GameUI.instance.EnterDungeon();
            EnterDungeon(dungeon.id);
            DungeonManager.instance.process = 5;
            for (int MonsterQuantity = 0; MonsterQuantity < 5; MonsterQuantity++)
            {
                Vector3 randomPos = DungeonManager.instance.SpawnMonster.position 
                                    + new Vector3(
                                        Random.Range(-3f, 3f),  // lệch X
                                        0,
                                        0
                                    );
                Instantiate(
                    DungeonManager.instance.ListMonster[DungeonManager.instance.CurrentDungeon],
                    randomPos,
                    Quaternion.identity
                );
            }
        }
        else
        {
            GameUI.instance.DisplayMessage("Không đủ level");
        }
    }
    public void FinishDungeon()
    {
        if (characters.currenthp > 0)
        {
            if (DungeonManager.instance.process == 0)
            {
                GameUI.instance.DisplayMessage("Chúc mừng bạn hoàn thành phó bản");
                Rewards.instance.LoadReward();
            }
        }
        else
        {
            GameUI.instance.DisplayMessage("Bạn đã thất bại");
        }
    }

    // [ServerRpc]
    // void StartDungeonServerRpc(int dungeonId, ServerRpcParams rpcParams = default)
    // {
    //     GameObject obj = Instantiate(
    //         DungeonManager.instance.DungeonMap[dun],
    //         DungeonManager.instance.SpawnMap.position,
    //         Quaternion.identity
    //     );

    //     NetworkObject netObj = obj.GetComponent<NetworkObject>();
    //     netObj.Spawn();
    // }
    public void EnterDungeon(int DungeonId)
    {
        GameObject obj = Instantiate(
            DungeonManager.instance.DungeonMap[DungeonId],
            DungeonManager.instance.SpawnMap.position,
            Quaternion.identity
        );

        // NetworkObject netObj = obj.GetComponent<NetworkObject>();
        // netObj.Spawn();
        Transform spawnPoint1 = obj.transform.Find("Grid/Map/Spawn");
        Transform spawnPoint2 = obj.transform.Find("Grid/Map/SpawnEnemy");
        DungeonManager.instance.SpawnPlayer = spawnPoint1;
        DungeonManager.instance.SpawnMonster = spawnPoint2;
        GameObject player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
        player.transform.position = DungeonManager.instance.SpawnPlayer.localPosition;
        // GameUI.instance.DisplayMessage("Bắt đầu");
    }
    public void ExitDungeon()
    {
        GameUI.instance.DisplayMessage("Bạn đã thất bại");
        Monster.instance.DestroyMonsterofDungeon();
        characters.UpdateState(0);
        characters.ResetHp();
        GameUI.instance.ExitDungeon();
    }
}
