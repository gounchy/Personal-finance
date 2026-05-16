using System.Net;
using System.Net.Mail;

namespace CNPM_Nhom12.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "huynguyenvungoc@gmail.com";
        private readonly string _appPassword = "ufqwvlrgtqegesoq";
        private readonly string _displayName = "PersonalFinance";

        public async Task<bool> SendOtpAsync(string toEmail, string otp)
        {
            string subject = "Mã xác thực OTP";
            string body = $@"
                <div style='font-family:Arial,sans-serif;max-width:480px;margin:auto;padding:24px;border:1px solid #eee;border-radius:8px'>
                    <h2 style='color:#1D9E75'>Xác thực tài khoản</h2>
                    <p>Mã OTP của bạn là:</p>
                    <div style='font-size:32px;font-weight:bold;letter-spacing:8px;color:#1D9E75;padding:16px 0'>{otp}</div>
                    <p style='color:#888'>Mã có hiệu lực trong <strong>5 phút</strong>. Không chia sẻ mã này cho ai.</p>
                </div>";

            return await SendMailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendForgotPasswordOtpAsync(string toEmail, string otp)
        {
            string subject = "Đặt lại mật khẩu";
            string body = $@"
                <div style='font-family:Arial,sans-serif;max-width:480px;margin:auto;padding:24px;border:1px solid #eee;border-radius:8px'>
                    <h2 style='color:#E74C3C'>Đặt lại mật khẩu</h2>
                    <p>Bạn vừa yêu cầu đặt lại mật khẩu. Mã OTP của bạn là:</p>
                    <div style='font-size:32px;font-weight:bold;letter-spacing:8px;color:#E74C3C;padding:16px 0'>{otp}</div>
                    <p style='color:#888'>Mã có hiệu lực trong <strong>5 phút</strong>. Nếu không phải bạn yêu cầu, hãy bỏ qua email này.</p>
                </div>";

            return await SendMailAsync(toEmail, subject, body);
        }

        private async Task<bool> SendMailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_fromEmail, _appPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(toEmail));

                await smtpClient.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}