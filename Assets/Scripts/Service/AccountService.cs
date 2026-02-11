using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountService: MonoBehaviour
{
    private AccountRepository accountRepository = new AccountRepositoryImpl();
    private CharactersRepository charactersRepository = new CharactersRepositoryImpl();
    public static AccountService Instance;

    void Awake()
    {
        Instance = this;
    }
    public void login(string username, string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            GameManager.Instance.HienThongBao("Mật khẩu không được để trống!");
            return;
        }

        Account account = accountRepository.findAccountByUsername(username);

        if (account == null)
        {
            GameManager.Instance.HienThongBao("Tài khoản không tồn tại!");
            return;
        }

        if (password != account.password)
        {
            GameManager.Instance.HienThongBao("Mật khẩu không đúng!");
            return;
        }

        SessionManager.Instance.account = account;
        if (charactersRepository.GetCharacterByAccountId(account.id) == null)
        {
            SceneManager.LoadScene("CreateCharacter");
            GameManager.Instance.HienThongBao("Chào mừng người chơi mới! Hãy mau tạo nhân vật cho mình và bắt đầu cuộc phiêu lưu nào!");
        }
        else
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    public void loginWithGoogle(string gmail, string sub)
    {
        Account acc1 = accountRepository.findAccountByGmail(gmail);
        Account acc2 = accountRepository.findAccountBySub(sub);
        if (acc1 != null && acc2 != null && acc1.id == acc2.id)
        {
            SessionManager.Instance.account = acc1;
            if (charactersRepository.GetCharacterByAccountId(SessionManager.Instance.account.id) == null)
            {
                Debug.Log("Heheboi");
                SceneManager.LoadScene("CreateCharacter");
                GameManager.Instance.HienThongBao("Chào mừng người chơi mới! Hãy mau tạo nhân vật cho mình và bắt đầu cuộc phiêu lưu nào!");
            }
            else
            {
                SceneManager.LoadScene("Lobby");
            }            
            return;
        }
        if (acc1 != null && acc2 == null)
        {
            accountRepository.LinkGoogle(acc1, sub);
            SessionManager.Instance.account = acc1;
            if (charactersRepository.GetCharacterByAccountId(SessionManager.Instance.account.id) == null)
            {
                Debug.Log("Heheboi");
                SceneManager.LoadScene("CreateCharacter");
                GameManager.Instance.HienThongBao("Chào mừng người chơi mới! Hãy mau tạo nhân vật cho mình và bắt đầu cuộc phiêu lưu nào!");
            }
            else
            {
                SceneManager.LoadScene("Lobby");
            }
            return;
        }
        accountRepository.CreateGoogleAccount(gmail, sub);
        SessionManager.Instance.account = accountRepository.findAccountByGmail(gmail);
        if (charactersRepository.GetCharacterByAccountId(SessionManager.Instance.account.id) == null)
        {
            Debug.Log("Heheboi");
            SceneManager.LoadScene("CreateCharacter");
            GameManager.Instance.HienThongBao("Chào mừng người chơi mới! Hãy mau tạo nhân vật cho mình và bắt đầu cuộc phiêu lưu nào!");
        }
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
        SessionManager.Instance.account = accountRepository.findAccountByGmail(gmail);
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
