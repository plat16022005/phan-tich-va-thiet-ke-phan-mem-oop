using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account
{
    public int id { get; set; }
    public string username {get; set; }
    public string password {get; set; }
    public string gmail {get; set; }
    public LoginType loginType {get; set; }
    public string sub {get; set; }
}
public enum LoginType
{
    LOCAL,
    GOOGLE,
    LOCAL_GOOGLE
}
