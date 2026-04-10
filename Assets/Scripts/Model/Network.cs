using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Network : NetworkBehaviour
{
    public static Network instance;
    private void Awake()
    {
        instance = this;
    }
    public IReadOnlyDictionary<ulong, NetworkClient> GetAllPlayerOnline()
    {
        return NetworkManager.Singleton.ConnectedClients;
    }
    public void RequestPlayerList()
    {
        RequestPlayerListServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    void RequestPlayerListServerRpc(ServerRpcParams rpcParams = default)
    {
        var clients = NetworkManager.Singleton.ConnectedClients;

        List<int> clientIds = new List<int>();
        List<int> playerIds = new List<int>();

        foreach (var client in clients)
        {
            var playerObj = client.Value.PlayerObject;
            if (playerObj == null) continue;

            PlayerNetwork playerId = playerObj.GetComponent<PlayerNetwork>();

            clientIds.Add((int)client.Key);
            playerIds.Add(playerId.CharacterId.Value);
        }

        SendPlayerListClientRpc(
            clientIds.ToArray(),
            playerIds.ToArray(),
            rpcParams.Receive.SenderClientId
        );
    }
    [ClientRpc]
    void SendPlayerListClientRpc(int[] clientIds, int[] playerIds, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
            GameUI.instance.ShowListPlayer(clientIds, playerIds);  
    }
}