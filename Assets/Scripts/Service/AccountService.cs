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
        if (string.IsNullOrEmpty(password))
        {
            GameManager.Instance.HienThongBao("Mật khẩu không được để trống!");
            return false;
        }

        Account account = accountRepository.findAccountByUsername(username);

        if (account == null)
        {
            GameManager.Instance.HienThongBao("Tài khoản không tồn tại!");
            return false;
        }

        if (password == account.password)
        {
            SessionManager.Instance.account = account;
            return true;
        }

        GameManager.Instance.HienThongBao("Mật khẩu không đúng!");
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
            GameManager.Instance.HienThongBao("Tên tài khoản đã có người đăng ký!");
            return false;
        }
        if (account == accountRepository.findAccountByGmail(gmail))
        {
            GameManager.Instance.HienThongBao("Gmail này đã có người đăng ký!");
            return false;
        }
        if (rePassword != password)
        {
            GameManager.Instance.HienThongBao("Vui lòng nhập lại mật khẩu đúng với mật khẩu đã đăng ký!");
            return false;
        }
        accountRepository.addAccountToSQL(account);
        SessionManager.Instance.account = account;
        return true;
    }
    public bool ChangePassFromResetPass(string NewPassword, string ReNewPassword, string gmail)
    {
        if (NewPassword != ReNewPassword)
        {
            GameManager.Instance.HienThongBao("Vui lòng nhập lại mật khẩu đúng với mật khẩu đã nhập!");
            return false;
        }
        if (string.IsNullOrEmpty(NewPassword))
        {
            GameManager.Instance.HienThongBao("Mật khẩu mới không được để trống!");
            return false;
        }
        Account acc = accountRepository.findAccountByGmail(gmail);
        accountRepository.ChangePass(acc.id, NewPassword);
        GameManager.Instance.HienThongBao("Đổi mật khẩu thành công vui lòng đăng nhập lại!");
        return true;
    }
}
