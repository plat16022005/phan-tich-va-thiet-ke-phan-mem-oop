using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class AccountController : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] private TMP_InputField username_login;
    [SerializeField] private TMP_InputField password_login;
    [Header("Register")]
    [SerializeField] private TMP_InputField username_register;
    [SerializeField] private TMP_InputField password_register;
    [SerializeField] private TMP_InputField rePassword_register;
    [SerializeField] private TMP_InputField gmail_register;

    // private AccountService accountService;

    // private void Awake()
    // {
    //     accountService = new AccountService();
    // }

    public void onClickLogin()
    {
        AccountService.Instance.login(username_login.text, password_login.text);
    }
    public void onClickRegister()
    {
        if(AccountService.Instance.register(username_register.text, password_register.text, rePassword_register.text, gmail_register.text))
        {
            SceneManager.LoadScene("CreateCharacter");
            Debug.Log("Đăng kí thành công");
        }
        else
        {
            Debug.Log("Đăng kí thất bại");
        }
    }
}

