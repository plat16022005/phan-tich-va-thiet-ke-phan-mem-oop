using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountService: MonoBehaviour
{
    private AccountRepository accountRepository = new AccountRepositoryImpl();
    public static AccountService Instance;

    void Awake()
    {
        Instance = this;
    }
    public bool login(string username, string password)
    {
        Account account = accountRepository.findAccountByUsername(username);
        Debug.Log(account);
        if (account != null && (password != "" || password != null) && password == account.password )
        {
            SessionManager.Instance.account = account;
            return true;
        }
        return false;
    }
    public void loginWithGoogle(string gmail, string sub)
    {
        Account acc1 = accountRepository.findAccountByGmail(gmail);
        Account acc2 = accountRepository.findAccountBySub(sub);
        if (acc1 != null && acc2 != null && acc1.id == acc2.id)
        {
            return;
        }
        if (acc1 != null && acc2 == null)
        {
            accountRepository.LinkGoogle(acc1, sub);
            return;
        }
        accountRepository.CreateGoogleAccount(gmail, sub);
    }
    public bool register(string username, string password, string rePassword, string gmail)
    {
        Account account = new Account();
        account.username = username;
        account.password = password;
        account.gmail = gmail;
        if (account == accountRepository.findAccountByUsername(username))
        {
            return false;
        }
        if (account == accountRepository.findAccountByGmail(gmail))
        {
            return false;
        }
        if (rePassword != password)
        {
            return false;
        }
        accountRepository.addAccountToSQL(account);
        SessionManager.Instance.account = account;
        return true;
    }
}
