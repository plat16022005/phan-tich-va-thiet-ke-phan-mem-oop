using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Linq.Expressions;

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
                    account.id = reader.GetInt32(0);
                    account.username = reader.GetString("username");
                    account.password = reader.GetString("password");
                    account.gmail = reader.GetString("gmail");
                    if (reader.GetString("logintype") == "LOCAL")
                    {
                        account.loginType = LoginType.LOCAL;
                    }
                    else if (reader.GetString("logintype") == "GOOGLE")
                    {
                        account.loginType = LoginType.GOOGLE;
                    }
                    else
                    {
                        account.loginType = LoginType.LOCAL_GOOGLE;
                    }
                    account.sub = reader.GetString("sub");
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
                    account.id = reader.GetInt32(0);
                    account.username = reader.GetString("username");
                    account.password = reader.GetString("password");
                    account.gmail = reader.GetString("gmail");
                    if (reader.GetString("logintype") == "LOCAL")
                    {
                        account.loginType = LoginType.LOCAL;
                    }
                    else if (reader.GetString("logintype") == "GOOGLE")
                    {
                        account.loginType = LoginType.GOOGLE;
                    }
                    else
                    {
                        account.loginType = LoginType.LOCAL_GOOGLE;
                    }
                    account.sub = reader.GetString("sub");
                    return account;
                }          
            }
        }
        return null;
    }
    public Account findAccountBySub(string sub)
    {
        Account account = new Account();
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM Account WHERE sub = @sub";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@sub", sub);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    account = new Account();
                    account.id = reader.GetInt32(0);
                    account.username = reader.GetString("username");
                    account.password = reader.GetString("password");
                    account.gmail = reader.GetString("gmail");
                    if (reader.GetString("logintype") == "LOCAL")
                    {
                        account.loginType = LoginType.LOCAL;
                    }
                    else if (reader.GetString("logintype") == "GOOGLE")
                    {
                        account.loginType = LoginType.GOOGLE;
                    }
                    else
                    {
                        account.loginType = LoginType.LOCAL_GOOGLE;
                    }
                    account.sub = reader.GetString("sub");
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
            string sql = "INSERT INTO Account(username, password, gmail, logintype, sub) VALUES (@username, @password, @gmail, @logintype, @sub)";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@username", account.username);
            cmd.Parameters.AddWithValue("@password", account.password);
            cmd.Parameters.AddWithValue("@gmail", account.gmail);
            cmd.Parameters.AddWithValue("@logintype", "LOCAL");
            cmd.Parameters.AddWithValue("@sub", "");

            cmd.ExecuteNonQuery();
        }
    }
    public Account CreateGoogleAccount(string email, string sub)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();

            string username = "google_" + Guid.NewGuid().ToString("N")[..6];

            var cmd = new MySqlCommand(@"
                INSERT INTO account (username, password, gmail, logintype, sub)
                VALUES (@u, '', @g, 'GOOGLE', @sub);
            ", conn);

            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@g", email);
            cmd.Parameters.AddWithValue("@sub", sub);

            cmd.ExecuteNonQuery();

            return new Account
            {
                id = 0,
                username = username,
                password = "",
                gmail = email,
                loginType = LoginType.GOOGLE,
                sub = sub
            };
        }
    }
    public void LinkGoogle(Account acc, string sub)
    {
        using(MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE Account
                SET sub = @sub,
                    logintype = 'LOCAL_GOOGLE'
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@sub", sub);
            cmd.Parameters.AddWithValue("@id", acc.id);
            cmd.ExecuteNonQuery();
        }
    }
    public void ChangePass(int account_id, string newpass)
    {
        using(MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();
            var cmd = new MySqlCommand(@"UPDATE Account SET password = @newpass WHERE id = @account_id", conn);
            cmd.Parameters.AddWithValue("@newpass", newpass);
            cmd.Parameters.AddWithValue("@account_id", account_id);
            cmd.ExecuteNonQuery();
        }
    }
}
