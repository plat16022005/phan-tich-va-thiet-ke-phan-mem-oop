using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

// using TMPro.EditorUtilities;
using System;
using WebSocketSharp;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;

public class LobbyManager : MonoBehaviour
{
    [Header("Lobby")]
    [SerializeField] private GameObject PanelLobby;
    [SerializeField] private Button ShowListLobbies;
    [SerializeField] private GameObject RoomPrefab;
    [SerializeField] private GameObject LobbiesContent;
    [SerializeField] private TextMeshProUGUI NamePlayer;

    [Header("Create Room")]
    [SerializeField] private GameObject PanelCreateRoom;
    [SerializeField] private TMP_InputField RoomName;
    [SerializeField] private TMP_InputField MaxPlayer;
    [SerializeField] private Button CreateRoom;
    [SerializeField] private Toggle IsPrivateToggle;

    [Header("Room Panel")]
    [SerializeField] private GameObject PanelPanelRoom;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Code;
    [SerializeField] private GameObject PlayerInfoContent;
    [SerializeField] private GameObject PlayerPrefabs;
    [SerializeField] private Button LeaveRoomButton;
    [SerializeField] private Button StartGameButton;

    [Header("Join Room")]
    [SerializeField] private GameObject PanelJoinRoom;
    [SerializeField] private TMP_InputField MaPhong;

    private CharactersRepository charactersRepository = new CharactersRepositoryImpl();
    private Characters characters = new Characters();
    private Equipment equipment = new Equipment();
    private Avatar avatar = new Avatar();
    private Lobby currentLobby;
    private bool isLeavingLobby = false;
    private bool isQuitting = false;


    private string playerId;
    // Start is called before the first frame update
    async void Start()
    {
        characters = charactersRepository.GetCharacterByAccountId(SessionManager.Instance.account.id);
        avatar = charactersRepository.GetAvatarByCharacterId(characters.id);
        equipment = charactersRepository.GetEquipmentByCharacterId(characters.id);
        // Tạo option với profile name khác nhau
        // Nếu bạn muốn test nhiều bản build, hãy truyền profile name qua command line hoặc UI
        InitializationOptions options = new InitializationOptions();
        
        #if !UNITY_EDITOR
            // Ví dụ: Tạo profile ngẫu nhiên cho mỗi lần mở bản build để tránh trùng
            options.SetProfile("Player_" + UnityEngine.Random.Range(0, 10000).ToString());
        #endif

        await UnityServices.InitializeAsync(options); // Truyền options vào đây

        AuthenticationService.Instance.SignedIn += () =>
        {
            playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Player Id: " + playerId);
        };
        
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        NamePlayer.text = characters.nickname;
    }

    // Update is called once per frame
    void Update()
    {
        HandleLobbyHeartbeat();   // host gửi heartbeat
        // UpdatePlayerHeartbeat();  // player update lastSeen

        PollLobby();              // luôn refresh data trước
        // HandleRoomUpdate();       // xử lý UI
        // CheckGhostPlayers();      // host kick ghost
        DetectDeadHost();         // client detect host chết
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = RoomName.text;
            int maxPlayer = int.Parse(MaxPlayer.text);
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = IsPrivateToggle.isOn,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member, "false")}
                }
            };
            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);
            EnterRoom();
        } catch (LobbyServiceException e)
        {
            Debug.Log("" + e.Message);
        }
    }
    public void OpenPanelCreateLobby()
    {
        PanelCreateRoom.SetActive(true);
    }
    public void OpenPanelJoinRoom()
    {
        PanelJoinRoom.SetActive(true);
    }
    public void ShutDownPanel(string panel)
    {
        if (panel == "Join")
            PanelJoinRoom.SetActive(false);
        else if (panel == "Create")
            PanelCreateRoom.SetActive(false);
    }
    private void EnterRoom()
    {
        PanelLobby.SetActive(false);
        PanelCreateRoom.SetActive(false);
        PanelPanelRoom.SetActive(true);

        Name.text = currentLobby.Name;
        Code.text = currentLobby.LobbyCode;
        VisualizeRoomdetails();
    }
    private void VisualizeRoomdetails()
    {
        // Clear UI
        for (int i = PlayerInfoContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(PlayerInfoContent.transform.GetChild(i).gameObject);
        }

        if (!IsInLobby() || currentLobby == null) ExitRoom();

        foreach (Player player in currentLobby.Players)
        {
            if (!player.Data.ContainsKey("CharacterId"))
                continue;

            int characterId;
            if (!int.TryParse(player.Data["CharacterId"].Value, out characterId))
                continue;

            Characters otherCharacter = charactersRepository.GetCharacterById(characterId);
            Avatar otherAvatar = charactersRepository.GetAvatarByCharacterId(characterId);
            Equipment otherEquipment = charactersRepository.GetEquipmentByCharacterId(characterId);

            if (otherCharacter == null || otherAvatar == null)
                continue;

            GameObject newPlayerInfo = Instantiate(PlayerPrefabs, PlayerInfoContent.transform);

            // ===== TEXT =====
            newPlayerInfo.transform.Find("Name")
                ?.GetComponent<TextMeshProUGUI>()
                .SetText(otherCharacter.nickname);

            newPlayerInfo.transform.Find("CLASS")
                ?.GetComponent<TextMeshProUGUI>()
                .SetText(otherCharacter.@class.ToString());

            newPlayerInfo.transform.Find("LV")
                ?.GetComponent<TextMeshProUGUI>()
                .SetText("LV." + otherCharacter.level);

            // ===== SPRITES =====
            SetImage(newPlayerInfo, "Weapon",
                SpritesManager.Instance.spritesWeapon[(int)otherCharacter.@class]);

            SetImage(newPlayerInfo, "Head",
                SpritesManager.Instance.spritesHead[(int)otherCharacter.race]);

            SetImage(newPlayerInfo, "Head/Hair",
                SpritesManager.Instance.spritesHair[otherAvatar.hair]);

            SetImage(newPlayerInfo, "Head/Eyes",
                SpritesManager.Instance.spritesEyes[otherAvatar.eyes]);

            SetImage(newPlayerInfo, "Head/Nose",
                SpritesManager.Instance.spritesNose[otherAvatar.nose]);

            SetImage(newPlayerInfo, "Head/Mouth",
                SpritesManager.Instance.spritesMouth[otherAvatar.mouth]);

            SetImage(newPlayerInfo, "Head/Tren",
                SpritesManager.Instance.spritesTren[(int)otherCharacter.race]);

            SetImage(newPlayerInfo, "Body/Ao",
                SpritesManager.Instance.spritesBody[otherEquipment.armor_id]);
            SetImage(newPlayerInfo, "Body/VaiTrai",
                SpritesManager.Instance.spritesVaiTrai[otherEquipment.armor_id]);
            SetImage(newPlayerInfo, "Body/VaiPhai",
                SpritesManager.Instance.spritesVaiPhai[otherEquipment.armor_id]);
            SetImage(newPlayerInfo, "Body/VaiTrai/TayTrai",
                SpritesManager.Instance.spritesTayTrai[otherEquipment.armor_id]);
            SetImage(newPlayerInfo, "Body/VaiPhai/TayPhai",
                SpritesManager.Instance.spritesTayPhai[otherEquipment.armor_id]);
            SetImage(newPlayerInfo, "Body/VaiTrai/TayTrai/BanTayTrai",
                SpritesManager.Instance.spritesBanTayTrai[(int)otherCharacter.race]);
            SetImage(newPlayerInfo, "Body/VaiPhai/TayPhai/BanTayPhai",
                SpritesManager.Instance.spritesBanTayPhai[(int)otherCharacter.race]);
            if (IsHost() && player.Id != playerId)
            {
                Button btnKick = newPlayerInfo.GetComponentInChildren<Button>(true);
                btnKick.onClick.AddListener(() => KickPlayer(player.Id));
                btnKick.gameObject.SetActive(true);   
            }            
        }
        if (IsHost())
        {
            StartGameButton.onClick.AddListener(StartGame);
            StartGameButton.gameObject.SetActive(true);
        }
        else
        {
            if (IsGameStarted())
            {
                StartGameButton.onClick.AddListener(EnterGame);
                StartGameButton.gameObject.SetActive(true);
                StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Vào game";
            }
            else
            {
                StartGameButton.onClick.RemoveAllListeners();
                StartGameButton.gameObject.SetActive(false);
            }
        }
    }
    private void SetImage(GameObject root, string path, Sprite sprite)
    {
        Transform t = root.transform.Find(path);
        if (t == null) return;

        Image img = t.GetComponent<Image>();
        if (img == null) return;

        img.sprite = sprite;
    }

    // private float roomUpdateHandle = 2f;
    // private async void HandleRoomUpdate()
    // {
    //     if (currentLobby == null) return;

    //     roomUpdateHandle -= Time.deltaTime;
    //     if (roomUpdateHandle > 0) return;

    //     roomUpdateHandle = 2f;

    //     try
    //     {
    //         Lobby newLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

    //         bool stillInLobby = false;

    //         foreach (Player p in newLobby.Players)
    //         {
    //             if (p.Id == playerId)
    //             {
    //                 stillInLobby = true;
    //                 break;
    //             }
    //         }

    //         if (!stillInLobby && !isLeavingLobby)
    //         {
    //             GameManager.Instance.HienThongBao("Bạn đã bị Host kick!");
    //             currentLobby = null;
    //             ExitRoom();
    //             return;
    //         }

    //         currentLobby = newLobby;
    //         VisualizeRoomdetails();
    //     }
    //     catch (LobbyServiceException e)
    //     {
    //         if (isQuitting) return;

    //         Debug.Log("Lobby closed or host left: " + e.Message);

    //         if (!isLeavingLobby)
    //         {
    //             GameManager.Instance.HienThongBao("Phòng đã đóng hoặc bạn đã bị kick!");
    //         }

    //         currentLobby = null;
    //         ExitRoom();
    //     }
    // }

    public async Task DeleteLobby()
    {
        if (currentLobby == null) return;

        if (!IsHost())
        {
            Debug.Log("Only host can delete lobby!");
            return;
        }

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

            GameManager.Instance.HienThongBao("Phòng đã bị xóa!");

            currentLobby = null;
            ExitRoom();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("DeleteLobby Error: " + e.Message);
        }
    }

    public async void ShowListLobby()
    {
        try
        {
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
            VisualizeLobbyList(response.Results);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private float HeartBeatTimer = 15f;
    private async void HandleLobbyHeartbeat()
    {
        if (currentLobby != null && IsHost())
        {
            HeartBeatTimer -= Time.deltaTime;
            if (HeartBeatTimer <= 0)
            {
                HeartBeatTimer = 15f;
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
    }
    private async void OnApplicationQuit()
    {
        if (IsHost() && currentLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            }
            catch { }
        }
    }

    private bool IsHost()
    {
        if (currentLobby == null) return false;
        if (string.IsNullOrEmpty(playerId)) return false;

        return currentLobby.HostId == playerId;
    }

    private void VisualizeLobbyList(List<Lobby> _publicLobbies)
    {
        for (int i = 0; i < LobbiesContent.transform.childCount; i++)
        {
            Destroy(LobbiesContent.transform.GetChild(i).gameObject);
        }
        foreach(Lobby _lobby in _publicLobbies)
        {
            GameObject room = Instantiate(RoomPrefab, LobbiesContent.transform);
            var RoomDetail = room.GetComponentsInChildren<TextMeshProUGUI>();
            RoomDetail[0].text = _lobby.Name;
            RoomDetail[1].text = _lobby.Players.Count.ToString() + "/" + _lobby.MaxPlayers.ToString();
            room.GetComponentInChildren<Button>()
                .onClick.AddListener(() => JoinRoom(_lobby));
        }
    }
    private async void JoinRoom(Lobby lobby)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            EnterRoom();
            Debug.Log("" + lobby.Players.Count);
        } catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { 
                    "CharacterId", 
                    new PlayerDataObject(
                        PlayerDataObject.VisibilityOptions.Member,
                        characters.id.ToString()
                    ) 
                }
            }
        };
    }

    public async void LeaveRoom()
    {
        if (currentLobby == null || string.IsNullOrEmpty(playerId)) return;

        try
        {
            isLeavingLobby = true;
            if (currentLobby.HostId == playerId)
            {
                await DeleteLobby();
                return;
            }
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);

            currentLobby = null;
            ExitRoom();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("LeaveRoom Error: " + e.Message);
        }
    }

    private void ExitRoom()
    {
        currentLobby = null;
        isLeavingLobby = false;

        PanelLobby.SetActive(true);
        PanelCreateRoom.SetActive(false);
        PanelPanelRoom.SetActive(false);
    }

    private bool IsInLobby()
    {
        if (currentLobby == null || currentLobby.Players == null)
            return false;

        foreach (Player _player in currentLobby.Players)
        {
            if (_player.Id == playerId)
            {
                return true;
            }
        }

        currentLobby = null;
        return false;
    }
    private async void KickPlayer(string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void JoinRoomByCode()
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(MaPhong.text, options);
            try
            {
                EnterRoom();
            }
            catch
            {
                await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
            }
        } catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }        
    }
    private float playerPingTimer = 5f;

    // private async void UpdatePlayerHeartbeat()
    // {
    //     if (currentLobby == null) return;
    //     if (string.IsNullOrEmpty(playerId)) return;

    //     playerPingTimer -= Time.deltaTime;
    //     if (playerPingTimer > 0) return;

    //     playerPingTimer = 5f;

    //     try
    //     {
    //         await LobbyService.Instance.UpdatePlayerAsync(
    //             currentLobby.Id,
    //             playerId,
    //             new UpdatePlayerOptions
    //             {
    //                 Data = new Dictionary<string, PlayerDataObject>
    //                 {
    //                     {
    //                         "lastSeen",
    //                         new PlayerDataObject(
    //                             PlayerDataObject.VisibilityOptions.Member,
    //                             DateTime.UtcNow.Ticks.ToString()
    //                         )
    //                     }
    //                 }
    //             }
    //         );
    //     }
    //     catch (LobbyServiceException e)
    //     {
    //         if (e.Reason == LobbyExceptionReason.RateLimited)
    //         {
    //             playerPingTimer = 8f; // tự giảm nhịp nếu bị spam limit
    //         }
    //     }
    // }

    // private float checkGhostTimer = 3f;

    // private async void CheckGhostPlayers()
    // {
    //     if (currentLobby == null) return;
    //     if (!IsHost()) return;

    //     checkGhostTimer -= Time.deltaTime;
    //     if (checkGhostTimer <= 0)
    //     {
    //         checkGhostTimer = 3f;

    //         var now = DateTime.UtcNow;

    //         foreach (var player in currentLobby.Players)
    //         {
    //             if (player.Id == playerId) continue;

    //             if (player.Data.TryGetValue("lastSeen", out var data))
    //             {
    //                 var lastSeenTicks = long.Parse(data.Value);
    //                 var lastSeen = new DateTime(lastSeenTicks);

    //                 if ((now - lastSeen).TotalSeconds > 5)
    //                 {
    //                     await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, player.Id);
    //                 }
    //             }
    //         }
    //     }
    // }
    private float pollTimer = 8f;

    private async void PollLobby()
    {
        if (currentLobby == null) return;

        pollTimer -= Time.deltaTime;
        if (pollTimer > 0) return;

        pollTimer = 8f;

        try
        {
            Lobby newLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

            bool stillInLobby = newLobby.Players.Any(p => p.Id == playerId);

            if (!stillInLobby && !isLeavingLobby)
            {
                GameManager.Instance.HienThongBao("Bạn đã bị Host kick!");
                currentLobby = null;
                ExitRoom();
                return;
            }

            currentLobby = newLobby;
            VisualizeRoomdetails();
        }
        catch (LobbyServiceException e)
        {
            if (e.Reason == LobbyExceptionReason.RateLimited)
            {
                Debug.Log("Rate limited - slowing down polling");
                pollTimer = 12f; // nếu bị 429 thì tự chậm lại
                return;
            }

            Debug.Log("Lobby error: " + e.Message);

            if (!isLeavingLobby)
            {
                GameManager.Instance.HienThongBao("Phòng đã đóng!");
            }

            currentLobby = null;
            ExitRoom();
        }
    }

    private async void DetectDeadHost()
    {
        if (currentLobby == null) return;
        if (IsHost()) return;

        var host = currentLobby.Players
            .FirstOrDefault(p => p.Id == currentLobby.HostId);

        if (host == null) return;

        if (host.Data.TryGetValue("lastSeen", out var data))
        {
            var lastSeen = new DateTime(long.Parse(data.Value));
            if ((DateTime.UtcNow - lastSeen).TotalSeconds > 20)
            {
                Debug.Log("Host dead. Deleting lobby...");

                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                }
                catch { }
            }
        }
    }
    private async void StartGame()
    {
        if (currentLobby != null && IsHost())
        {
            try
            {
                UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {"IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member, "true")}
                    }
                };
                currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateLobbyOptions);
                EnterGame();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

    }
    private bool IsGameStarted()
    {
        if (currentLobby != null)
        {
            if (currentLobby.Data["IsGameStarted"].Value == "true")
            {
                return true;
            }
        }
        return false;
    }
    private async void EnterGame()
    {
        if (IsHost())
        {
            ushort port = (ushort)UnityEngine.Random.Range(10000, 60000);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            string hostIP = GetLocalIPAddress();

            transport.SetConnectionData(hostIP, port);

            await LobbyService.Instance.UpdateLobbyAsync(
                currentLobby.Id,
                new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "port", new DataObject(DataObject.VisibilityOptions.Member, port.ToString()) },
                        { "ip", new DataObject(DataObject.VisibilityOptions.Member, hostIP) }
                    }
                });

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene(
                "GameScene",
                LoadSceneMode.Single
            );
        }
        else
        {
            string hostIP = currentLobby.Data["ip"].Value;
            ushort port = ushort.Parse(currentLobby.Data["port"].Value);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(hostIP, port);

            NetworkManager.Singleton.StartClient();
        }
    }
    private string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }


}
