using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class PlayerService : MonoBehaviour
{  
    public static PlayerService Instance { get; private set;}
    private CharactersRepository charactersRepository = new CharactersRepositoryImpl();
    void Awake()
    {
        Instance = this;
    }
    public bool IsValidCharacterName(string name)
    {
        // Chỉ cho chữ và số, dài từ 1–15 ký tự
        string pattern = @"^[a-zA-Z0-9]{1,15}$";
        return Regex.IsMatch(name, pattern);
    }
    public void CreatePlayer(string name, int hairIndex, int eyesIndex, int noseIndex, int mouthIndex, int raceIndex, int classIndex)
    {
        if (!IsValidCharacterName(name))
        {
            GameManager.Instance.HienThongBao(
                "Tên chỉ gồm chữ và số, không ký tự đặc biệt, tối đa 15 ký tự"
            );
            return;
        }

        if (!charactersRepository.ConfirmName(name))
        {
            GameManager.Instance.HienThongBao("Tên nhân vật đã có người sử dụng");
            return;
        }
        if (string.IsNullOrEmpty(name))
        {
            GameManager.Instance.HienThongBao("Tên nhân vật không được để trống");
            return;            
        }

        try{
            Characters characters = new Characters();
            characters.account_id = SessionManager.Instance.account.id;
            characters.nickname = name;
            characters.race = (TypeRace)raceIndex;
            characters.@class = (TypeClass)classIndex;
            characters.currenthp = characters.hp;
            charactersRepository.CreateCharacter(characters);
            Characters currentCharacters = charactersRepository.GetCharacterByAccountId(characters.account_id);
            Avatar avatar = new Avatar();
            avatar.character_id = currentCharacters.id;
            avatar.hair = hairIndex;
            avatar.eyes = eyesIndex;
            avatar.nose = noseIndex;
            avatar.mouth = mouthIndex;
            charactersRepository.CreateAvatar(avatar);
            Equipment equipment = new Equipment();
            equipment.character_id = currentCharacters.id;
            equipment.weapon_id = classIndex + 1;
            equipment.armor_id = 1;
            equipment.pants_id = 1;
            equipment.boots_id = 1;
            charactersRepository.CreateEquipment(equipment);
            SceneManager.LoadScene("Lobby");
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
}
