using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountService
{
    private AccountRepository accountRepository = new AccountRepositoryImpl();
    public bool login(string username, string password)
    {
        Account account = accountRepository.findAccountByUsername(username);
        Debug.Log(account);
        if (account != null)
        {
            SessionManager.Instance.account = account;
            return true;
        }
        return false;
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
