using System;
using TMPro;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public TMP_InputField BoxName;
    [Header("Avatar")]
    public SpriteRenderer hair;
    public Sprite[] hairOptions;
    public TMP_InputField BoxHair;

    public SpriteRenderer eyes;
    public Sprite[] eyesOptions;
    public TMP_InputField BoxEyes;

    public SpriteRenderer nose;
    public Sprite[] noseOptions;
    public TMP_InputField BoxNose;

    public SpriteRenderer mouth;
    public Sprite[] mouthOptions;
    public TMP_InputField BoxMouth;

    public SpriteRenderer tren;
    public Sprite[] trenOptions;

    [Header("Race")]
    public SpriteRenderer head;
    public Sprite[] headOptions;

    public SpriteRenderer BanTayTrai;
    public Sprite[] BanTayTraiOptions;

    public SpriteRenderer BanTayPhai;
    public Sprite[] BanTayPhaiOptions;
    public TMP_InputField BoxRace;
    public String[] RaceString;
    public TMP_InputField RaceDetail;

    [Header("Class")]
    public SpriteRenderer weaponRight;
    public SpriteRenderer weaponLeft;
    public Sprite[] weaponOptions;
    public TMP_InputField BoxClass;
    public String[] ClassString;
    public TMP_InputField ClassDetail;

    int hairIndex = 0;
    int eyesIndex = 0;
    int noseIndex = 0;
    int mouthIndex = 0;

    int raceIndex = 0;
    int classIndex = 0;

    void Start()
    {
        UpdateAll();
    }

    public void NextHair()
    {
        hairIndex = (hairIndex + 1) % hairOptions.Length;
        hair.sprite = hairOptions[hairIndex];
        BoxHair.text = "Kiểu tóc " + (hairIndex + 1);
    }

    public void PreviousHair()
    {
        hairIndex = (hairIndex - 1 + hairOptions.Length) % hairOptions.Length;
        hair.sprite = hairOptions[hairIndex];
        BoxHair.text = "Kiểu tóc " + (hairIndex + 1);
    }

    public void NextEyes()
    {
        eyesIndex = (eyesIndex + 1) % eyesOptions.Length;
        eyes.sprite = eyesOptions[eyesIndex];
        BoxEyes.text = "Kiểu mắt " + (eyesIndex + 1);
    }

    public void NextNose()
    {
        noseIndex = (noseIndex + 1) % noseOptions.Length;
        nose.sprite = noseOptions[noseIndex];
        BoxNose.text = "Kiểu mũi " + (noseIndex + 1);
    }

    public void NextMouth()
    {
        mouthIndex = (mouthIndex + 1) % mouthOptions.Length;
        mouth.sprite = mouthOptions[mouthIndex];
        BoxMouth.text = "Kiểu miệng " + (mouthIndex + 1);
    }

    public void NextRace()
    {
        raceIndex = (raceIndex + 1) % headOptions.Length;
        BoxRace.text = RaceString[raceIndex];
        UpdateRace();
    }

    void UpdateRace()
    {
        head.sprite = headOptions[raceIndex];
        BanTayTrai.sprite = BanTayTraiOptions[raceIndex];
        BanTayPhai.sprite = BanTayPhaiOptions[raceIndex];
        UpdateRaceDetail();

        UpdateTren();
    }

    void UpdateTren()
    {
        // chỉ race 3 và 4 mới có "tren"
        if (raceIndex == 3 || raceIndex == 4)
        {
            if (raceIndex == 3)
                tren.sprite = trenOptions[0];
            if (raceIndex == 4)
                tren.sprite = trenOptions[1];
        }
        else
        {
            tren.sprite = null;
        }
    }

    public void NextClass()
    {
        classIndex = (classIndex + 1) % weaponOptions.Length;
        BoxClass.text = ClassString[classIndex];
        UpdateWeapon();
    }

    void UpdateWeapon()
    {
        // tắt hết trước
        weaponLeft.sprite = null;
        weaponRight.sprite = null;
        UpdateClassDetail();

        if (classIndex == 1)
        {
            weaponRight.sprite = weaponOptions[classIndex];
        }
        else if (classIndex == 4 || classIndex == 6)
        {
            weaponRight.sprite = weaponOptions[classIndex];
            weaponLeft.sprite = weaponOptions[classIndex];
        }
        else
        {
            weaponLeft.sprite = weaponOptions[classIndex];
        }
    }
    public void PreviousEyes()
    {
        eyesIndex = (eyesIndex - 1 + eyesOptions.Length) % eyesOptions.Length;
        eyes.sprite = eyesOptions[eyesIndex];
        BoxEyes.text = "Kiểu mắt " + (eyesIndex + 1);
    }

    public void PreviousNose()
    {
        noseIndex = (noseIndex - 1 + noseOptions.Length) % noseOptions.Length;
        nose.sprite = noseOptions[noseIndex];
        BoxNose.text = "Kiểu mũi " + (noseIndex + 1);
    }

    public void PreviousMouth()
    {
        mouthIndex = (mouthIndex - 1 + mouthOptions.Length) % mouthOptions.Length;
        mouth.sprite = mouthOptions[mouthIndex];
        BoxMouth.text = "Kiểu miệng " + (mouthIndex + 1);
    }

    public void PreviousRace()
    {
        raceIndex = (raceIndex - 1 + headOptions.Length) % headOptions.Length;
        BoxRace.text = RaceString[raceIndex];
        UpdateRace();
    }

    public void PreviousClass()
    {
        classIndex = (classIndex - 1 + weaponOptions.Length) % weaponOptions.Length;
        BoxClass.text = ClassString[classIndex];
        UpdateWeapon();
    }
    void UpdateRaceDetail()
    {
        if (raceIndex == 0)
        {
            RaceDetail.text = "Tộc người, với tuổi đời thấp hơn so với các tộc khác. Vì thế, họ có nhu cầu học hỏi cao và nhanh, dường như cân bằng mọi thứ. Do đó: +10% Exp, +5% mọi chỉ số";
        }
        if (raceIndex == 1)
        {
            RaceDetail.text = "Tộc elf, với tuổi đời cao hơn so với các tộc khác, họ hấp thụ được nhiều linh khí đất trời. Do đó: +20% Mana, Hồi 1% Mana/5s";
        }
        if (raceIndex == 2)
        {
            RaceDetail.text = "Tộc Goblin, họ vô cùng gian xảo và nhanh nhẹn. Do đó: +10% vàng, +10% tốc độ, +50% STCM";
        }
        if (raceIndex == 3)
        {
            RaceDetail.text = "Tộc quỷ, họ vô cùng tàn ác và khát máu. Do đó: +20% ATK, mỗi 10% HP mất đi tăng 1% sát thương";
        }
        if (raceIndex == 4)
        {
            RaceDetail.text = "Tộc thú, với sự hoang dã của mình, họ có khả năng bền bỉ, sinh tồn cao. Do đó: +20% HP, khả năng hồi máu nhận được +50%";
        }
    }
    void UpdateClassDetail()
    {
        if (classIndex == 0)
        {
            ClassDetail.text = "Warrior là một Class chuyên về kiếm với bộ kỹ năng cân bằng mọi thứ.";
        }
        if (classIndex == 1)
        {
            ClassDetail.text = "Guardian là một Class chuyên về khiên với bộ kỹ năng chuyên về sinh tồn nhưng tốc độ đánh khá thấp và kém cơ động.";
        }
        if (classIndex == 2)
        {
            ClassDetail.text = "Archer là một Class chuyên về cung với bộ kỹ năng chuyên về tầm xa nhưng khả năng phòng thủ thấp.";
        }
        if (classIndex == 3)
        {
            ClassDetail.text = "Mage là một Class chuyên về trượng với bộ kỹ năng chuyên về phép với sát thương cao nhưng tiêu tốn mana rất nhanh.";
        }
        if (classIndex == 4)
        {
            ClassDetail.text = "Assassin là một Class chuyên về dao với bộ kỹ năng có sát thương khá cao và nhanh nhưng phòng thủ kém.";
        }
        if (classIndex == 5)
        {
            ClassDetail.text = "Berserker là một Class chuyên về rìu với bộ kỹ năng điên cuồng, sát thương cao nhưng tốc độ đánh chậm.";
        }
        if (classIndex == 6)
        {
            ClassDetail.text = "Monk là một Class chuyên về găng với bộ kỹ năng chuyên về khống chế, tốc độ đánh nhanh nhưng sát thương không cao";
        }
    }
    void UpdateAll()
    {
        hair.sprite = hairOptions[hairIndex];
        BoxHair.text = "Kiểu tóc " + (hairIndex + 1);
        eyes.sprite = eyesOptions[eyesIndex];
        BoxEyes.text = "Kiểu mắt " + (eyesIndex + 1);
        nose.sprite = noseOptions[noseIndex];
        BoxNose.text = "Kiểu mũi " + (noseIndex + 1);
        mouth.sprite = mouthOptions[mouthIndex];
        BoxMouth.text = "Kiểu miệng " + (mouthIndex + 1);

        UpdateRace();
        BoxRace.text = RaceString[raceIndex];
        UpdateWeapon();
        BoxClass.text = ClassString[classIndex];
        UpdateRaceDetail();
        UpdateClassDetail();
    }
    public void CreateNewCharacter()
    {
        PlayerService.Instance.CreatePlayer(BoxName.text, hairIndex, eyesIndex, noseIndex, mouthIndex, raceIndex, classIndex);
    }
}
