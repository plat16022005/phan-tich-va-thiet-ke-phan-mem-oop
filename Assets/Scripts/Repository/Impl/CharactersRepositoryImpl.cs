using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using UnityEngine;

public class CharactersRepositoryImpl : CharactersRepository
{
    public Characters IsCreateCharacters(int account_id)
    {
        Characters characters = new Characters();
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM Characters WHERE account_id = @account_id";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@account_id", account_id);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    characters.id = reader.GetInt32(0);
                    characters.account_id = reader.GetInt32("account_id");
                    characters.nickname = reader.GetString("nickname");
                    characters.hp = reader.GetInt32("hp");
                }
            }
            
        }
        return null;
    } 
}