using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }
    public string joinCode;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);        
    }
    // private async void Start()
    // {

    //     await UnityServices.InitializeAsync();
    //     AuthenticationService.Instance.SignedIn += () =>
    //     {
    //         Debug.Log("Sign In" + AuthenticationService.Instance.PlayerId);
    //     };
    //     await AuthenticationService.Instance.SignInAnonymouslyAsync();

    // }
    public async Task InitializeServices()
    {
        InitializationOptions options = new InitializationOptions();
        
        #if !UNITY_EDITOR
            // Ví dụ: Tạo profile ngẫu nhiên cho mỗi lần mở bản build để tránh trùng
            options.SetProfile("Player_" + UnityEngine.Random.Range(0, 10000).ToString());
        #endif
        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
    }
    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            RelayServerData relayServerData= new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }
    public async void JoinRelay(string joinCode)
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("Network already running!");
            return;
        }
        try
        {
            Debug.Log("Joining Relay with "+joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData= new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
