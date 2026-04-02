using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkBehaviour
{
    public SpriteRenderer head;
    public SpriteRenderer bantaytrai;
    public SpriteRenderer bantayphai;
    public SpriteRenderer tren;
    public SpriteRenderer hair;
    public SpriteRenderer eyes;
    public SpriteRenderer nose;
    public SpriteRenderer mouth;
    public SpriteRenderer weaponLeft;
    public SpriteRenderer weaponRight;
    public NetworkVariable<CustomData> customData = new NetworkVariable<CustomData>();
    public static CustomNetworkManager Instance {get; private set;}
    void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        customData.OnValueChanged += OnCustomDataChanged;
        ApplyCustomData(customData.Value);
        // Server load data khi player spawn
        if (IsOwner)
        {
            int accountId = SessionManager.Instance.account.id;

            LoadCharacterServerRpc(accountId);
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (Camera.main.GetComponent<CameraFollow>().target == null)
            {
                var follow = Camera.main.GetComponent<CameraFollow>();
                if (follow != null)
                {
                    follow.target = transform;
                }
            }
        }

    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Đã chuyển scene");
        if (!IsOwner) return;

        if (Camera.main != null)
        {
            var follow = Camera.main.GetComponent<CameraFollow>();
            if (follow != null)
            {
                follow.target = transform;
            }
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (SessionManager.Instance == null || SessionManager.Instance.account == null)
        {
            Debug.LogError("SessionManager chưa có dữ liệu!");
            return;
        }
    }
    void OnCustomDataChanged(CustomData oldValue, CustomData newValue)
    {
        ApplyCustomData(newValue);
    }

    void ApplyCustomData(CustomData customData)
    {
        head.sprite = SpritesManager.Instance.spritesHead[customData.race];
        bantaytrai.sprite = SpritesManager.Instance.spritesBanTayTrai[customData.race];
        bantayphai.sprite = SpritesManager.Instance.spritesBanTayPhai[customData.race];
        tren.sprite = SpritesManager.Instance.spritesTren[customData.race];
        hair.sprite = SpritesManager.Instance.spritesHair[customData.hair];
        eyes.sprite = SpritesManager.Instance.spritesEyes[customData.eyes];
        nose.sprite = SpritesManager.Instance.spritesNose[customData.nose];
        mouth.sprite = SpritesManager.Instance.spritesMouth[customData.mouth];
        weaponLeft.sprite = SpritesManager.Instance.spritesWeaponLeft[customData.@class];
        weaponRight.sprite = SpritesManager.Instance.spritesWeaponRight[customData.@class];
    }
    [ServerRpc]
    void LoadCharacterServerRpc(int accountId)
    {
        Debug.Log("SERVER LOAD CHARACTER: " + accountId);
        CharactersRepository repo = new CharactersRepositoryImpl();
        Characters characters = repo.GetCharacterByAccountId(accountId);
        Avatar avatar = repo.GetAvatarByCharacterId(characters.id);

        CustomData data = DataNetworkService.Instance.CreateCustomData(
            avatar.hair,
            avatar.eyes,
            avatar.nose,
            avatar.mouth,
            (int)characters.race,
            (int)characters.@class
        );

        customData.Value = data;
    }
}