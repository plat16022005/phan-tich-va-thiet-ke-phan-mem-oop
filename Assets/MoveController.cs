using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MoveController : NetworkBehaviour
{
    // [SerializeField] private GameObject spawnedObjectPrefab;
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData
    {
        _int = 56, _bool = true, message = ""
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        }; 
    }
    public struct MyCustomData: INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public string message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     Transform spawnedGameObjectTransfrom = Instantiate(spawnedObjectPrefab.transform);
        //     spawnedGameObjectTransfrom.GetComponent<NetworkObject>().Spawn(true);
        //     // TestServerRpc(new ServerRpcParams());
        //     // TestClientRpc(new ClientRpcParams{Send = new ClientRpcSendParams
        //     // {
        //     //     TargetClientIds = new List<ulong> {1}
        //     // }});
        //     // randomNumber.Value = new MyCustomData
        //     // {
        //     //     _int = Random.Range(0,100),
        //     //     _bool = false,
        //     //     message = "Phạm Lê Anh Tuấn"
        //     // };
        // }
        Vector3 moveDir = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W))
        {
            moveDir.y = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1f;
        }
        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log($"Test ServerRPC: {OwnerClientId}; {serverRpcParams.Receive.SenderClientId}");
    }
    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log($"Test ClientRPC: {clientRpcParams.Send.TargetClientIds}");
    }
}
