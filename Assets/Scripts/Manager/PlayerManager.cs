using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private CharactersRepository charactersRepository;
    public Characters characters;
    public static PlayerManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        charactersRepository = new CharactersRepositoryImpl();
        characters = charactersRepository.GetCharacterByAccountId(SessionManager.Instance.account.id);
    }
}
