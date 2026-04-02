using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LoginFactory
{
    public abstract ILogin CreateLogin(string username, string password);
}
