using MailKit.Net.Smtp;
using MimeKit;

namespace Infrastructure.ViewModels
{
    public class EmailHelper
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // استبدل بالقيمة الحقيقية
        private readonly int _smtpPort = 587; // أو 465 للاتصال الآمن
        private readonly string _smtpUser = "admin@gmail.com"; // البريد الإلكتروني
        private readonly string _smtpPass = "P@ssword123"; // كلمة المرور

        public bool SendEmail(string toEmail, string confirmationLink, string subject)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Your App Name", _smtpUser)); // استبدل "Your App Name" باسم تطبيقك
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                // إنشاء محتوى الرسالة
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
                        <p><a href='{confirmationLink}'>Confirm Email</a></p>"
                };
                message.Body = bodyBuilder.ToMessageBody();

                // إرسال الرسالة باستخدام SMTP
                using var client = new SmtpClient();
                client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(_smtpUser, _smtpPass);
                client.Send(message);
                client.Disconnect(true);

                return true; // نجاح الإرسال
            }
            catch (Exception ex)
            {
                // سجل الخطأ باستخدام Logger
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return false;
            }
        }
    }
}
