using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    LoginFactory factory;

    public void SelectAccountLogin()
    {
        factory = new AccountLoginFactory();
    }

    public void SelectGoogleLogin()
    {
        factory = new GoogleLoginFactory();
    }

    public void OnLoginButton()
    {
        ILogin login = factory.CreateLogin(
            usernameInput.text,
            passwordInput.text
        );

        login.Login();
    }
}
