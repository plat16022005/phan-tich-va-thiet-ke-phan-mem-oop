using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AccountRepository
{
    public Account findAccountByUsername(string username);
    public Account findAccountByGmail(string gmail);
    public void addAccountToSQL(Account account);
}
