using UnityEngine;
using System.Net;
using System.Text;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GoogleOAuthListener : MonoBehaviour
{
    HttpListener listener;
    private AccountService accountService;

    async void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Debug.Log("⏳ Waiting Google redirect...");

        var ctx = await listener.GetContextAsync();
        var req = ctx.Request;
        var res = ctx.Response;

        string html = "<html><body>Login success! Return to game.</body></html>";
        byte[] buf = Encoding.UTF8.GetBytes(html);
        res.OutputStream.Write(buf, 0, buf.Length);
        res.Close();

        string code = req.QueryString["code"];
        Debug.Log("🔥 AUTH CODE = " + code);
        Debug.Log("Nhận code nè");

        // 🔥 QUAN TRỌNG: ĐÓNG LISTENER TRƯỚC KHI GỌI BACKEND
        listener.Stop();
        listener.Close();
        listener = null;

        StartCoroutine(SendCodeToBackend(code));
    }

    IEnumerator SendCodeToBackend(string code)
    {
        string url = "http://localhost:3000/google-login?code=" + code;
        Debug.Log("➡️ Sending code to backend...");

        using UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 10;

        yield return www.SendWebRequest();

        Debug.Log("⬅️ Backend responded");

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ BACKEND ERROR: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        Debug.Log("RAW JSON = " + json);

        GoogleLoginResponse data =
            JsonUtility.FromJson<GoogleLoginResponse>(json);

        // Debug.Log("🆔 GOOGLE SUB = " + data.sub);
        // Debug.Log("📧 EMAIL = " + data.email);
        AccountService.Instance.loginWithGoogle(data.email, data.sub);
        SceneManager.LoadScene("CreateCharacter");
    }
}

[System.Serializable]
public class GoogleLoginResponse
{
    public string sub;      // 🔥 THÊM
    public string email;
    public string token;
}

