using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLogin : ILogin
{
    string username;
    string password;
    public AccountLogin(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
    public void Login()
    {
        AccountService.Instance.login(username, password);
    }
}
