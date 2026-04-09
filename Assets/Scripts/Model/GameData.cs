using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameData
{
    public GameData()
    {
        
    }
    public QuestContent ViewDetail(int QuestId)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM questcontent WHERE id_quest = @id_quest";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id_quest", QuestId);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    QuestContent questContent = new QuestContent
                    {
                        id = reader.GetInt32(0),
                        id_quest = reader.GetInt32(1),
                        NameQuest = reader.GetString(2),
                        content = reader.GetString(3),
                        required = reader.GetInt32(4)
                    };
                    return questContent;
                }
            }
        }
        return null;
    }
    public List<Quests> FindAllQuest()
    {
        List<Quests> quests = new List<Quests>();

        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM quest";

            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Quests q = new Quests
                        {
                            id = reader.GetInt32("id"),
                            NameQuest = reader.GetString("NameQuest"),
                            TypeQuest = (QuestType)reader.GetInt32("TypeQuest"),
                        };

                        quests.Add(q);
                    }
                }
            }
        }

        return quests;
    }
    public void UpdateGold(int gold, int CharactersId)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            var cmd = new MySqlCommand("UPDATE characters SET gold = @gold WHERE id = @CharactersId", connection);
            cmd.Parameters.AddWithValue("@gold", gold);
            cmd.Parameters.AddWithValue("@CharactersId", CharactersId);
            cmd.ExecuteNonQuery();
        }
    }
    public List<Dungeon> FindAllDungeon()
    {
        List<Dungeon> DungeonList = new List<Dungeon>();
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM dungeon";
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dungeon dungeon = new Dungeon
                        {
                            id = reader.GetInt32(0),
                            NameDungeon = reader.GetString(1),
                            required = reader.GetInt32(2)  
                        };
                        DungeonList.Add(dungeon);
                    }
                }
            }
        }
        return DungeonList;
    }
    public Rewards FindRewardofDungeon(int DungeonId)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM rewards WHERE id_dungeon = @id_dungeon";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id_dungeon", DungeonId);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Rewards rewards = new Rewards
                    {
                        id = reader.GetInt32(0),
                        id_dungeon = reader.GetInt32(1),
                        gold = reader.GetInt32(2)
                    };
                    return rewards;
                }
            }
        }
        return null;        
    }
    // public List<Items> GetInventoryData()
    // {
    //     List<Items> items = new List<Items>();

    //     using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
    //     {
    //         connection.Open();
    //         string sql = "SELECT id_item FROM inventory WHERE id_player = @id_player";

    //         using (MySqlCommand cmd = new MySqlCommand(sql, connection))
    //         {
    //             cmd.Parameters.AddWithValue("@id_player", PlayerManager.instance.characters.id);
    //             using (MySqlDataReader reader = cmd.ExecuteReader())
    //             {
    //                 while (reader.Read())
    //                 {
    //                     Items i = new Items
    //                     {
    //                         id = reader.GetInt32(0),
    //                         NameItem = ItemsManager.instance.NameItems[reader.GetInt32(0)]
    //                     };

    //                     items.Add(i);
    //                 }
    //             }
    //         }
    //     }

    //     return items;
    // }
    public List<Inventory> GetInventoryData()
    {
        List<Inventory> inventory = new List<Inventory>();

        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM inventory WHERE id_player = @id_player";

            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id_player", PlayerManager.instance.characters.id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Inventory i = new Inventory
                        {
                            id = reader.GetInt32(0),
                            id_player = reader.GetInt32(1),
                            id_item = reader.GetInt32(2)
                        };

                        inventory.Add(i);
                    }
                }
            }
        }

        return inventory;
    }
    public int FindLvEquipmentofPlayer(int InventoryId)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT lv FROM enchancement WHERE id_inventory = @id_inventory";
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id_inventory", InventoryId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }
            }
        }
        return 0;
    }
    public Dictionary<int, (Items, int)> FindItemsWithLvofPlayer()
    {
        Dictionary<int, (Items, int)> ItemsWithLv = new Dictionary<int, (Items, int)>();

        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT it.id AS item_id, it.NameItem, e.lv FROM inventory i JOIN items it ON i.id_item = it.id LEFT JOIN enchancement e ON e.id_inventory = i.id WHERE i.id_player = @id_player";

            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id_player", PlayerManager.instance.characters.id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Items i = new Items
                        {
                            id = reader.GetInt32(0),
                            NameItem = reader.GetString(1),
                        };
                        int lv = reader.GetInt32(2);
                        Debug.Log(reader.GetInt32(0) + reader.GetString(1) + reader.GetInt32(2));
                        ItemsWithLv.Add(reader.GetInt32(0), (i,lv));
                    }
                }
            }
        }

        return ItemsWithLv;        
    }
    public void SaveProgress(Items selectedItems, Resource goldRes)
    {
        
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            var cmd1 = new MySqlCommand("UPDATE characters SET gold = @gold WHERE id = @CharactersId", connection);
            cmd1.Parameters.AddWithValue("@gold", PlayerManager.instance.characters.gold);
            cmd1.Parameters.AddWithValue("@CharactersId", PlayerManager.instance.characters.id);
            cmd1.ExecuteNonQuery();
            var cmd2 = new MySqlCommand("UPDATE enchancement e JOIN inventory i ON e.id_inventory = i.id SET e.lv = e.lv + 1 WHERE i.id_player = @id_player AND i.id_item = @id_item;", connection);
            cmd2.Parameters.AddWithValue("@id_player", PlayerManager.instance.characters.id);
            cmd2.Parameters.AddWithValue("@id_item", selectedItems.id);
            cmd2.ExecuteNonQuery();
            InventoryManager.instance.itemsLv[selectedItems.id] = (selectedItems, selectedItems.GetLevel() + 1);
        }        
    }
}
