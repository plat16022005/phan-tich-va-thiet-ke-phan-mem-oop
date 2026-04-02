using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLoginFactory : LoginFactory
{
    public override ILogin CreateLogin(string username, string password)
    {
        return new AccountLogin(username, password);
    }
}
