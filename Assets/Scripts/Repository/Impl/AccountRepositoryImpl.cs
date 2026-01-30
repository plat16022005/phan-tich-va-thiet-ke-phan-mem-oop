using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class AccountRepositoryImpl : AccountRepository
{
    public Account findAccountByUsername(string username)
    {
        Account account = new Account();
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM Account WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@username", username);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    account = new Account();
                    account.username = reader.GetString("username");
                    account.password = reader.GetString("password");
                    account.gmail = reader.GetString("gmail");
                    return account;
                }         
            }
        }
        return null;
    }
    public Account findAccountByGmail(string gmail)
    {
        Account account = new Account();
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM Account WHERE gmail = @gmail";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@gmail", gmail);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    account = new Account();
                    account.username = reader.GetString("username");
                    account.password = reader.GetString("password");
                    account.gmail = reader.GetString("gmail");
                    return account;
                }         
            }
        }
        return null;
    }
    public void addAccountToSQL(Account account)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "INSERT INTO Account(username, password, gmail) VALUES (@username, @password, @gmail)";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@username", account.username);
            cmd.Parameters.AddWithValue("@password", account.password);
            cmd.Parameters.AddWithValue("@gmail", account.gmail);
            cmd.ExecuteNonQuery();
        }
    }
}
