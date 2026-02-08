using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject ThongBao;
    private TextMeshProUGUI textThongBao;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // bật nếu cần dùng xuyên scene
    }

    private void Start()
    {
        if (ThongBao != null)
        {
            textThongBao = ThongBao.GetComponentInChildren<TextMeshProUGUI>();
            ThongBao.SetActive(false);
        }
        else
        {
            Debug.LogError("Chưa gán ThongBao trong GameManager!");
        }
    }

    public void HienThongBao(string text)
    {
        if (ThongBao == null || textThongBao == null) return;

        ThongBao.SetActive(true);
        textThongBao.text = text;
    }

    public void AnThongBao()
    {
        if (ThongBao == null) return;
        ThongBao.SetActive(false);
    }
}
