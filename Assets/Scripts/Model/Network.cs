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
            if (client.Key == rpcParams.Receive.SenderClientId)
                continue;
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
    public void RequestTrade(ulong targetClientId)
    {
        RequestTradeServerRpc(targetClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    void RequestTradeServerRpc(ulong targetClientId, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;

        var senderObj = NetworkManager.Singleton.ConnectedClients[senderId].PlayerObject;
        var senderPlayer = senderObj.GetComponent<PlayerNetwork>();

        int senderCharacterId = senderPlayer.CharacterId.Value;

        SendTradeRequestClientRpc(
            senderId,
            senderCharacterId,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { targetClientId }
                }
            }
        );
    }
    [ClientRpc]
    void SendTradeRequestClientRpc(ulong SenderClientId, int senderCharacterId, ClientRpcParams clientRpcParams = default)
    {
        GameUI.instance.ShowTradeRequest(SenderClientId, senderCharacterId);
    }
    public void AcceptTrade(ulong senderClientId)
    {
        AcceptTradeServerRpc(senderClientId);
    }

[ServerRpc(RequireOwnership = false)]
void AcceptTradeServerRpc(ulong senderClientId, ServerRpcParams rpcParams = default)
{
    ulong accepterId = rpcParams.Receive.SenderClientId;

    var senderObj = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
    var accepterObj = NetworkManager.Singleton.ConnectedClients[accepterId].PlayerObject;

    int charA = senderObj.GetComponent<PlayerNetwork>().CharacterId.Value;
    int charB = accepterObj.GetComponent<PlayerNetwork>().CharacterId.Value;

    // 🔥 tạo session
    TradeSession session = new TradeSession
    {
        playerA = senderClientId,
        playerB = accepterId,
        charA = charA,
        charB = charB
    };

    activeTrades[senderClientId] = session;
    activeTrades[accepterId] = session;

    ulong[] targets = new ulong[] { senderClientId, accepterId };

    StartTradeClientRpc(senderClientId, accepterId, charA, charB,
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = targets }
        });
}
[ClientRpc]
void StartTradeClientRpc(
    ulong playerA, 
    ulong playerB,
    int charA,
    int charB,
    ClientRpcParams clientRpcParams = default)
{
    GameUI.instance.OpenTradeUI(playerA, playerB, charA, charB);
}
    public void SelectTradeItem(int inventoryId)
    {
        SelectTradeItemServerRpc(inventoryId);
    }

[ServerRpc(RequireOwnership = false)]
void SelectTradeItemServerRpc(int inventoryId, ServerRpcParams rpcParams = default)
{
    ulong senderId = rpcParams.Receive.SenderClientId;

    if (!activeTrades.ContainsKey(senderId)) return;

    var session = activeTrades[senderId];

    if (senderId == session.playerA)
        session.itemA = inventoryId;
    else
        session.itemB = inventoryId;

    // reset confirm nếu đổi item
    session.confirmA = false;
    session.confirmB = false;

    SyncTradeItemClientRpc(senderId, inventoryId);
}
    [ClientRpc]
    void SyncTradeItemClientRpc(ulong senderId, int inventoryId)
    {
        GameUI.instance.UpdateTradeItem(senderId, inventoryId);
    }
public void ConfirmTrade()
{
    ConfirmTradeServerRpc();
}

[ServerRpc(RequireOwnership = false)]
void ConfirmTradeServerRpc(ServerRpcParams rpcParams = default)
{
    ulong senderId = rpcParams.Receive.SenderClientId;

    if (!activeTrades.ContainsKey(senderId)) return;

    var session = activeTrades[senderId];

    if (senderId == session.playerA)
        session.confirmA = true;
    else
        session.confirmB = true;

    // 🔥 kiểm tra đủ điều kiện trade
    if (session.confirmA && session.confirmB &&
        session.itemA != -1 && session.itemB != -1)
    {
        Debug.Log("Đủ điều kiện trade → thực hiện");

        GameData data = new GameData();
        data.ExecuteTrade(session.charA, session.itemA, session.charB, session.itemB);

        // notify client
        CompleteTradeClientRpc(session.playerA, session.playerB);

        // xoá session
        activeTrades.Remove(session.playerA);
        activeTrades.Remove(session.playerB);
    }
}
[ClientRpc]
void CompleteTradeClientRpc(ulong playerA, ulong playerB)
{
    if (NetworkManager.Singleton.LocalClientId == playerA ||
        NetworkManager.Singleton.LocalClientId == playerB)
    {
        GameUI.instance.OnTradeCompleted();
    }
}

    Dictionary<ulong, TradeSession> activeTrades = new Dictionary<ulong, TradeSession>();
    public void CancelTrade()
    {
        CancelTradeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    void CancelTradeServerRpc(ServerRpcParams rpcParams = default)
    {
        CancelTradeClientRpc();
    }
    [ClientRpc]
    void CancelTradeClientRpc()
    {
        GameUI.instance.CloseTradeUI();
    }
}