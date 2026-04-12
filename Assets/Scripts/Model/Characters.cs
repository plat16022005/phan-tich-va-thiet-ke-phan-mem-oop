using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.SceneManagement;

public class Characters
{
    private Avatar avatar;
    private Equipment equipment;
    private Inventory inventory;
    private List<Quests> quests;
    private GameData GameData = new GameData();
    public Characters(){
    }
    public int id { get; set; }
    public int account_id { get; set; }

    public string nickname { get; set; }

    // Base Stats
    public int hp { get; set; } = 100;
    public int mana { get; set; } = 100;
    public int atk { get; set; } = 10;
    public int def { get; set; } = 5;
    public int speed { get; set; } = 5;

    public float crit_rate { get; set; } = 0f;
    public float crit { get; set; } = 1f;

    // Identity
    public TypeRace race { get; set; }
    public TypeClass @class { get; set; }   // class là keyword => phải dùng @

    // Progress
    public int level { get; set; } = 1;
    public int exp { get; set; } = 0;
    public int gold { get; set; } = 0;
    public int currenthp {get; set;}
    public void StartQuest(int QuestId)
    {
        Quests.instance.Track(QuestId);
        Quests.instance.UpdateState("Đang thực hiện");
        string local = Quests.instance.GetLocation(QuestId);
        GameUI.instance.DisplayLocation(local);
    }
    public void ResultQuest(int QuestId)
    {
        bool con = Quests.instance.CheckConditions(QuestId);
        if (con == true)
        {
            int rewards = Quests.instance.GetReward(QuestId);
            Inventory.instance.AddReward(rewards);
            Quests.instance.UpdateState("Đã hoàn thành");
            GameUI.instance.DisplayMessage("Chúc mừng bạn đã hoàn thành nhiệm vụ");
        }
        else
        {
            Quests.instance.UpdateState("Thất bại");
            GameUI.instance.DisplayMessage("Bạn đã thất bại");
        }
    }
    public void UpdateState(int In_Dungeon)
    {
        DungeonManager.instance.CurrentDungeon = In_Dungeon;
    }
    public void ResetHp()
    {
        currenthp = hp;
    }
    public void LoadStatsAndPotentialPoints()
    {
        StatsManager.instance.potentialPoint = PlayerManager.instance.potentialPoint;
        StatsManager.instance.stats = new int[] { hp, mana, atk, def };
        GameUI.instance.DisplayStats(StatsManager.instance.stats, StatsManager.instance.potentialPoint);
    }
    public void AddStatPreview(string StatType)
    {
        if (StatsManager.instance.potentialPoint > 0)
        {
            UpdateTempStatsAndPoint(StatType);
            GameUI.instance.UpdatePreviewDisplay();
        }
        else
        {
            GameUI.instance.DisplayMessage("Hết điểm tiềm năng");
        }
    }
    public void UpdateTempStatsAndPoint(string StatType)
    {
        StatsManager.instance.potentialPoint -= 1;
        if (StatType == "HP")
        {
            StatsManager.instance.stats[0] += 5;
        }
        else if (StatType == "Mana")
        {
            StatsManager.instance.stats[1] += 5;
        }
        else if (StatType == "Atk")
        {
            StatsManager.instance.stats[2] += 1;
        }
        else if (StatType == "Def")
        {
            StatsManager.instance.stats[3] += 1;
        }
    }
    public void ApplyUpgrade()
    {
        CommitStats();
        GameData.SaveStats();
        GameUI.instance.DisplayMessage("Nâng cấp thành công");
    }
    public void CommitStats()
    {
        PlayerManager.instance.potentialPoint = StatsManager.instance.potentialPoint;
        PlayerManager.instance.characters.hp = StatsManager.instance.stats[0];
        PlayerManager.instance.characters.mana = StatsManager.instance.stats[1];
        PlayerManager.instance.characters.atk = StatsManager.instance.stats[2];
        PlayerManager.instance.characters.def = StatsManager.instance.stats[3];
    }
    public void CancelReview()
    {
        RollBackStats();
        GameUI.instance.UpdatePreviewDisplay();
    }
    public void RollBackStats()
    {
        StatsManager.instance.potentialPoint = PlayerManager.instance.potentialPoint;
        StatsManager.instance.stats[0] = PlayerManager.instance.characters.hp;
        StatsManager.instance.stats[1] = PlayerManager.instance.characters.mana;
        StatsManager.instance.stats[2] = PlayerManager.instance.characters.atk;
        StatsManager.instance.stats[3] = PlayerManager.instance.characters.def;
    }
    public void CreatePlayer(string name, int HairIndex, int EyesIndex, int NoseIndex, int MouthIndex, int RaceIndex, int ClassIndex)
    {
        bool con1 = IsValidCharacterName(name);
        bool con2 = GameData.ConfirmName(name);
        bool con3 = CheckNullName(name);
        if (!con1)
        {
            GameUI.instance.DisplayMessage(
                "Tên chỉ gồm chữ và số, không ký tự đặc biệt, tối đa 15 ký tự"
            );
            return;
        }

        else if (!con2)
        {
            GameUI.instance.DisplayMessage("Tên nhân vật đã có người sử dụng");
            return;
        }
        else if (con3)
        {
            GameUI.instance.DisplayMessage("Tên nhân vật không được để trống");
            return;            
        }
        else
        {
            CreateNewPlayer(name, HairIndex, EyesIndex, NoseIndex, MouthIndex, RaceIndex, ClassIndex);
        }

    }
    public void CreateNewPlayer(string name, int hairIndex, int eyesIndex, int noseIndex, int mouthIndex, int raceIndex, int classIndex)
    {
        Characters characters = new Characters{
            account_id = SessionManager.Instance.account.id,
            nickname = name,
            race = (TypeRace)raceIndex,
            @class = (TypeClass)classIndex,
            currenthp = hp
        };
        GameData.CreateCharacter(characters);
        Characters currentCharacters = GameData.GetCharacterByAccountId(characters.account_id);
        avatar = new Avatar{
            character_id = currentCharacters.id,
            hair = hairIndex,
            eyes = eyesIndex,
            nose = noseIndex,
            mouth = mouthIndex
        };
        GameData.CreateAvatar(avatar);
        equipment = new Equipment{
            character_id = currentCharacters.id,
            weapon_id = classIndex + 1,
            armor_id = 1,
            pants_id = 1,
            boots_id = 1
        };
        GameData.CreateEquipment(equipment);
        SceneManager.LoadScene("Lobby");     
    }
    public bool IsValidCharacterName(string name)
    {
        // Chỉ cho chữ và số, dài từ 1–15 ký tự
        string pattern = @"^[a-zA-Z0-9]{1,15}$";
        return Regex.IsMatch(name, pattern);
    }
    public bool CheckNullName(string name)
    {
        return string.IsNullOrEmpty(name);
    }
}


public enum TypeRace
{
    HUMAN = 0,
    ELF = 1,
    GOBLIN = 2,
    DEMON = 3,
    ANIMAL= 4
}
public enum TypeClass 
{
    Warrior = 0,
    Guardian = 1,
    Archer = 2,
    Mage = 3,
    Assassin = 4,
    Berserker = 5,
    Monk = 6
}