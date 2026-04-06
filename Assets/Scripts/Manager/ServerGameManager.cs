using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ServerGameManager : MonoBehaviour
{
    private CharactersRepository charactersRepository = new CharactersRepositoryImpl();

    // map clientId → accountId
    private Dictionary<ulong, int> clientAccounts = new Dictionary<ulong, int>();

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    public void RegisterClientAccount(ulong clientId, int accountId)
    {
        clientAccounts[clientId] = accountId;
    }

    void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        NetworkObject playerObject =
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (!clientAccounts.ContainsKey(clientId)) return;

        int accountId = clientAccounts[clientId];

        Characters characters = charactersRepository.GetCharacterByAccountId(accountId);
        Avatar avatar = charactersRepository.GetAvatarByCharacterId(characters.id);
        Debug.Log(characters.id);

        CustomData data = DataNetworkService.Instance.CreateCustomData(
            avatar.hair,
            avatar.eyes,
            avatar.nose,
            avatar.mouth,
            (int)characters.race,
            (int)characters.@class
        );

        playerObject
            .GetComponentInChildren<CustomNetworkManager>()
            .customData.Value = data;
    }
}