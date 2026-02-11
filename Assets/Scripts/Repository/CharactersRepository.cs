using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CharactersRepository
{
    public bool ConfirmName(string name);
    public Characters GetCharacterByAccountId(int account_id);
    public void CreateCharacter(Characters characters);
    public void CreateEquipment(Equipment equipment);
    public Equipment GetEquipmentByCharacterId(int character_id);
    public void CreateAvatar(Avatar avatar);
    public Avatar GetAvatarByCharacterId(int character_id);
}
