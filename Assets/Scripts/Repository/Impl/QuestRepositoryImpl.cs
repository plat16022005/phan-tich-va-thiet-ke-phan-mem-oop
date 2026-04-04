using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using UnityEngine;

public class QuestRepositoryImpl : QuestRepository
{
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
}
