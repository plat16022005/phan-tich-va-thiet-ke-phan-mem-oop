using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoogleLogin : ILogin
{
    string clientId =
        "796867223192-dtvudrhoemvbpsnoqgeoummirloholrt.apps.googleusercontent.com";

    string redirectUri = "http://localhost:8080/";
    string scope = "openid email profile";
    public void Login()
    {
        string url =
            "https://accounts.google.com/o/oauth2/v2/auth" +
            "?client_id=" + clientId +
            "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
            "&response_type=code" +
            "&scope=" + Uri.EscapeDataString(scope);

        Application.OpenURL(url);
    }
}
