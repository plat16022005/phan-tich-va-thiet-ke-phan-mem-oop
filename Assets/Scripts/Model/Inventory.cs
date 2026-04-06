using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private Characters characters;
    private CharactersRepository CharactersRepository;
    private void Awake()
    {
        instance = this;
        CharactersRepository = new CharactersRepositoryImpl();
        characters = CharactersRepository.GetCharacterByAccountId(SessionManager.Instance.account.id);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(characters.id);
        }
    }
    public void AddReward(int rewards)
    {
        characters.gold += rewards;
        Debug.Log(characters.id);
        CharactersRepository.UpdateGold(characters.gold, characters.id);
    }
}
