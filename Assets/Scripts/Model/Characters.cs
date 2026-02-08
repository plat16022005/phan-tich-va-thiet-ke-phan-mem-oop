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
    public float rate { get; set; } = 1f;

    // Identity
    public Race race { get; set; }
    public Class @class { get; set; }   // class là keyword => phải dùng @

    // Progress
    public int level { get; set; } = 1;
    public int exp { get; set; } = 0;
    public int gold { get; set; } = 0;
}

public enum Race
{
    HUMAN = 0,
    ELF = 1,
    GOBLIN = 2,
    DEMON = 3,
    ANIMAL= 4
}
public enum Class 
{
    Warrior = 0,
    Guardian = 1,
    Archer = 2,
    Mage = 3,
    Assassin = 4,
    Berserker = 5,
    Monk = 6
}