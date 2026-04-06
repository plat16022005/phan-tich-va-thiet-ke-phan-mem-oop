public class Characters
{
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