using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AccountRepository
{
    public Account findAccountByUsername(string username);
    public Account findAccountByGmail(string gmail);
    public void addAccountToSQL(Account account);
    public Account CreateGoogleAccount(string email, string sub);
    public Account findAccountBySub(string sub);
    public void LinkGoogle(Account acc, string sub);
    public void ChangePass(int account_id, string newpass);
}
