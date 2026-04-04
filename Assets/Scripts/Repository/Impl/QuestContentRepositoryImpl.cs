using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using UnityEngine;

public class QuestContentRepositoryImpl : QuestContentRepository
{
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
}
