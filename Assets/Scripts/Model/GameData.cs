using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
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
}
