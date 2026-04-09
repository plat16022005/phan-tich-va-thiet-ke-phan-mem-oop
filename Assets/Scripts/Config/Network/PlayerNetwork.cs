using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<int> CharacterId = new NetworkVariable<int>();
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // gửi AccountId từ client lên server
            SetAccountIdServerRpc(PlayerManager.instance.characters.id);
        }
    }

    [ServerRpc]
    void SetAccountIdServerRpc(int id)
    {
        CharacterId.Value = id;
    }
}
