using System.Net;
using System.Net.Mail;
using UnityEngine;
using System.Text;

public class EmailService : MonoBehaviour
{
    private string fromEmail = "mylv3555@gmail.com";
    private string password = "vclu pobx ivyh llhe";
    public static EmailService Instance;
    void Awake()
    {
        Instance = this;
    }
    public void SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail, "Heroic Chronicles Support");
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(fromEmail, password);
            smtp.EnableSsl = true;

            smtp.Send(mail);

            Debug.Log("✅ Gửi email thành công!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Lỗi gửi email: " + ex.Message);
        }
    }
    public string GenerateOTP(int length)
    {
        string chars = "0123456789";
        StringBuilder otp = new StringBuilder();
        System.Random rnd = new System.Random();

        for (int i = 0; i < length; i++)
        {
            otp.Append(chars[rnd.Next(chars.Length)]);
        }

        return otp.ToString();
    }
}
