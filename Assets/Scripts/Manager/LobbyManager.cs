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
// using TMPro.EditorUtilities;
using System;
using WebSocketSharp;

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

    [Header("Room Panel")]
    [SerializeField] private GameObject PanelPanelRoom;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Code;
    [SerializeField] private GameObject PlayerInfoContent;
    [SerializeField] private GameObject PlayerPrefabs;
    [SerializeField] private Button LeaveRoomButton;

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
        HandleLobbyHeartbeat();
        HandleRoomUpdate();
    }
    public async void CreateLobby()
    {
        try
        {
            string lobbyName = RoomName.text;
            int maxPlayer = int.Parse(MaxPlayer.text);
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = GetPlayer()
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
    public void ShutDownPanelCreateLobby()
    {
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
        // Destroy children từ cuối lên để tránh index issues
        for (int i = PlayerInfoContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = PlayerInfoContent.transform.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
        if (IsInLobby())
        {
            foreach (Player player in currentLobby.Players)
            {
                GameObject newPlayerInfo = Instantiate(PlayerPrefabs, PlayerInfoContent.transform);

                string playerName = player.Data["PlayerName"].Value;
                int classId = int.Parse(player.Data["Class"].Value);
                int level = int.Parse(player.Data["Level"].Value);
                int raceId = int.Parse(player.Data["Race"].Value);
                int hairId = int.Parse(player.Data["Hair"].Value);
                int eyesId = int.Parse(player.Data["Eyes"].Value);
                int noseId = int.Parse(player.Data["Nose"].Value);
                int mouthId = int.Parse(player.Data["Mouth"].Value);

                newPlayerInfo.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = playerName;
                newPlayerInfo.transform.Find("CLASS").GetComponent<TextMeshProUGUI>().text = ((TypeClass)classId).ToString();
                newPlayerInfo.transform.Find("LV").GetComponent<TextMeshProUGUI>().text = "LV." + level;

                newPlayerInfo.transform.Find("Weapon").GetComponent<Image>().sprite =
                    SpritesManager.Instance.spritesWeapon[classId];

                newPlayerInfo.transform.Find("Head").GetComponent<Image>().sprite =
                    SpritesManager.Instance.spritesHead[raceId];

                newPlayerInfo.transform.Find("Head/Hair").GetComponent<Image>().sprite =
                    SpritesManager.Instance.spritesHair[hairId];

                newPlayerInfo.transform.Find("Head/Eyes").GetComponent<Image>().sprite =
                    SpritesManager.Instance.spritesEyes[eyesId];

                newPlayerInfo.transform.Find("Head/Nose").GetComponent<Image>().sprite =
                    SpritesManager.Instance.spritesNose[noseId];

                newPlayerInfo.transform.Find("Head/Mouth").GetComponent<Image>().sprite =
                    SpritesManager.Instance.spritesMouth[mouthId];
                
                newPlayerInfo.transform.Find("Head/Tren").GetComponent<Image>().sprite =
                    SpritesManager.Instance.spritesTren[raceId];
            }
            
        }

    }
    private float roomUpdateHandle = 2f;
    private async void HandleRoomUpdate()
    {
        if (currentLobby == null || !IsInLobby()) return;

        roomUpdateHandle -= Time.deltaTime;

        if (roomUpdateHandle > 0) return;

        roomUpdateHandle = 2f;

        try
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            VisualizeRoomdetails();
        }
        catch (LobbyServiceException e)
        {
            if (isQuitting) return;

            Debug.Log("Lobby closed or host left: " + e.Message);

            if (!isLeavingLobby)
            {
                currentLobby = null;
                ExitRoom();
            }
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
    private bool IsHost()
    {
        if (currentLobby.Id != null && currentLobby.HostId == playerId)
        {
            return true;
        }
        return false;
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
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, characters.nickname) },
                { "Class", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)characters.@class).ToString()) },
                { "Level", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, characters.level.ToString()) },
                { "Race", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)characters.race).ToString()) },
                { "Hair", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, avatar.hair.ToString()) },
                { "Eyes", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, avatar.eyes.ToString()) },
                { "Nose", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, avatar.nose.ToString()) },
                { "Mouth", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, avatar.mouth.ToString()) },
                { "Tren", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)characters.race).ToString()) }
            }
        };
    }

    public async void LeaveRoom()
    {
        if (currentLobby == null || string.IsNullOrEmpty(playerId)) return;

        try
        {
            isLeavingLobby = true;

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
}
