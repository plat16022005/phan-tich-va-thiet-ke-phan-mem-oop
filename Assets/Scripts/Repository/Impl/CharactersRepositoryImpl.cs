using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using UnityEngine;

public class CharactersRepositoryImpl : CharactersRepository
{
    public bool ConfirmName(string name)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();
            string sql = "SELECT nickname FROM Characters WHERE nickname = @name";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", name);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return false;
                }
            }
        }
        return true;
    }
    public Characters GetCharacterByAccountId(int account_id)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectSQL.connectionString))
        {
            connection.Open();

            string sql = "SELECT * FROM Characters WHERE account_id = @account_id LIMIT 1";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@account_id", account_id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Characters character = new Characters();

                    character.id = reader.GetInt32("id");
                    character.account_id = reader.GetInt32("account_id");
                    character.nickname = reader.GetString("nickname");

                    character.hp = reader.GetInt32("hp");
                    character.mana = reader.GetInt32("mana");
                    character.atk = reader.GetInt32("atk");
                    character.def = reader.GetInt32("def");
                    character.speed = reader.GetInt32("speed");

                    character.crit_rate = reader.GetFloat("crit_rate");
                    character.crit = reader.GetFloat("crit");

                    character.race = (TypeRace)reader.GetInt32("race");
                    character.@class = (TypeClass)reader.GetInt32("class");

                    character.level = reader.GetInt32("level");
                    character.exp = reader.GetInt32("exp");
                    character.gold = reader.GetInt32("gold");
                    character.currenthp = reader.GetInt32("currenthp");

                    return character;
                }
            }
        }
        return null;
    }

    public void CreateCharacter(Characters characters)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();

            string sql = @"
            INSERT INTO Characters
            (
                account_id,
                nickname,
                hp,
                mana,
                atk,
                def,
                speed,
                crit_rate,
                crit,
                race,
                `class`,
                level,
                exp,
                gold,
                currenthp
            )
            VALUES
            (
                @account_id,
                @nickname,
                @hp,
                @mana,
                @atk,
                @def,
                @speed,
                @crit_rate,
                @crit,
                @race,
                @class,
                @level,
                @exp,
                @gold,
                @currenthp
            )";

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@account_id", characters.account_id);
            cmd.Parameters.AddWithValue("@nickname", characters.nickname);

            cmd.Parameters.AddWithValue("@hp", characters.hp);
            cmd.Parameters.AddWithValue("@mana", characters.mana);
            cmd.Parameters.AddWithValue("@atk", characters.atk);
            cmd.Parameters.AddWithValue("@def", characters.def);
            cmd.Parameters.AddWithValue("@speed", characters.speed);

            cmd.Parameters.AddWithValue("@crit_rate", characters.crit_rate);
            cmd.Parameters.AddWithValue("@crit", characters.crit);

            cmd.Parameters.AddWithValue("@race", (int)characters.race);
            cmd.Parameters.AddWithValue("@class", (int)characters.@class);

            cmd.Parameters.AddWithValue("@level", characters.level);
            cmd.Parameters.AddWithValue("@exp", characters.exp);
            cmd.Parameters.AddWithValue("@gold", characters.gold);
            cmd.Parameters.AddWithValue("@currenthp", characters.currenthp);

            cmd.ExecuteNonQuery();
        }
    }
    public void CreateEquipment(Equipment equipment)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();

            string sql = @"
            INSERT INTO Equipment
            (
                character_id,
                weapon_id,
                armor_id,
                boots_id,
                pants_id
            )
            VALUES
            (
                @character_id,
                @weapon_id,
                @armor_id,
                @boots_id,
                @pants_id
            )";

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@character_id", equipment.character_id);
            cmd.Parameters.AddWithValue("@weapon_id", equipment.weapon_id);
            cmd.Parameters.AddWithValue("@armor_id", equipment.armor_id);
            cmd.Parameters.AddWithValue("@boots_id", equipment.boots_id);
            cmd.Parameters.AddWithValue("@pants_id", equipment.pants_id);

            cmd.ExecuteNonQuery();
        }
    }
    public Equipment GetEquipmentByCharacterId(int character_id)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();

            string sql = "SELECT * FROM Equipment WHERE character_id = @character_id";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@character_id", character_id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Equipment
                    {
                        character_id = reader.GetInt32("character_id"),
                        weapon_id = reader["weapon_id"] as int?,
                        armor_id = reader["armor_id"] as int?,
                        boots_id = reader["boots_id"] as int?,
                        pants_id = reader["pants_id"] as int?
                    };
                }
            }
        }
        return null;
    }
    public void CreateAvatar(Avatar avatar)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();

            string sql = @"
            INSERT INTO Avatar
            (
                character_id,
                hair,
                eyes,
                nose,
                mouth
            )
            VALUES
            (
                @character_id,
                @hair,
                @eyes,
                @nose,
                @mouth
            )";

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@character_id", avatar.character_id);
            cmd.Parameters.AddWithValue("@hair", avatar.hair);
            cmd.Parameters.AddWithValue("@eyes", avatar.eyes);
            cmd.Parameters.AddWithValue("@nose", avatar.nose);
            cmd.Parameters.AddWithValue("@mouth", avatar.mouth);

            cmd.ExecuteNonQuery();
        }
    }
    public Avatar GetAvatarByCharacterId(int character_id)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectSQL.connectionString))
        {
            conn.Open();

            string sql = "SELECT * FROM Avatar WHERE character_id = @character_id";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@character_id", character_id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Avatar
                    {
                        character_id = reader.GetInt32("character_id"),
                        hair = reader.GetInt32("hair"),
                        eyes = reader.GetInt32("eyes"),
                        nose = reader.GetInt32("nose"),
                        mouth = reader.GetInt32("mouth")
                    };
                }
            }
        }
        return null;
    }

}