using UnityEngine;
using System.Net;
using System.Text;
using System.Collections;
using UnityEngine.Networking;
using System.IO;

public class GoogleOAuthListener : MonoBehaviour
{
    private HttpListener listener;
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

        // 🔥 Đọc file HTML từ StreamingAssets
        string filePath = Path.Combine(Application.streamingAssetsPath, "success.html");

        string html;

        if (File.Exists(filePath))
        {
            html = File.ReadAllText(filePath);
        }
        else
        {
            html = "<html><body>Login success! Return to game.</body></html>";
            Debug.LogWarning("⚠ success.html not found! Using fallback HTML.");
        }

        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentLength64 = buffer.Length;
        res.ContentType = "text/html";
        res.OutputStream.Write(buffer, 0, buffer.Length);
        res.OutputStream.Close();

        string code = req.QueryString["code"];
        Debug.Log("🔥 AUTH CODE = " + code);

        // 🔥 Đóng listener trước khi gọi backend
        listener.Stop();
        listener.Close();
        listener = null;

        if (!string.IsNullOrEmpty(code))
        {
            StartCoroutine(SendCodeToBackend(code));
        }
        else
        {
            Debug.LogError("❌ No auth code received!");
        }
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

        AccountService.Instance.loginWithGoogle(data.email, data.sub);
    }

    private void OnApplicationQuit()
    {
        if (listener != null && listener.IsListening)
        {
            listener.Stop();
            listener.Close();
        }
    }
}

[System.Serializable]
public class GoogleLoginResponse
{
    public string sub;
    public string email;
    public string token;
}