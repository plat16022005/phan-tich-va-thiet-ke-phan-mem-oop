using TMPro;
using UnityEngine;

public class EmailController : MonoBehaviour
{
    [SerializeField] private TMP_InputField TargetGmail;
    [SerializeField] private TMP_InputField InputOTP;
    [Header("Reset Password")]
    [SerializeField] private PanelResetPass PanelReset;
    [SerializeField] private PanelButton PanelButton;
    [SerializeField] private TMP_InputField NewPass;
    [SerializeField] private TMP_InputField ReNewPass;
    private string otp = null;

    public void SendOtpMail()
    {
        otp = EmailService.Instance.GenerateOTP(6);

        string subject = "Mã xác thực OTP";
        string body =
            "Mã OTP của bạn là: " + otp +
            "\nCó hiệu lực trong 5 phút.";

        EmailService.Instance.SendEmail(TargetGmail.text, subject, body);
        GameManager.Instance.HienThongBao("Mã OTP đã được gửi về gmail " + TargetGmail.text);
        Debug.Log("OTP là: " + otp);
    }
    public void ResetPass()
    {
        if (otp != InputOTP.text)
        {
            GameManager.Instance.HienThongBao("Mã OTP không đúng! Vui lòng kiểm tra lại!");
            return;
        }
        if (string.IsNullOrEmpty(InputOTP.text))
        {
            GameManager.Instance.HienThongBao("Mã OTP không được để trống!");
            return;
        }
        if (string.IsNullOrEmpty(otp))
        {
            GameManager.Instance.HienThongBao("Vui lòng nhập gmail và nhấn nút xác nhận!");
            return;
        }
        PanelController.Instance.Show(PanelReset.GetType());
    }
    public void ChangePass()
    {
        if(AccountService.Instance.ChangePassFromResetPass(NewPass.text,ReNewPass.text,TargetGmail.text))
        {
            PanelController.Instance.Show(PanelButton.GetType());
        }
    }

}
