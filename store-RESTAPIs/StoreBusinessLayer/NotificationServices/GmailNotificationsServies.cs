using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using StoreServices.NotificationServices;

namespace StoreBusinessLayer.NotificationServices
{
    public class GmailServes : INotifications
    {
        public async Task SendNotification(string Subject, string Body, string EmailToSend)
        {
            try
            {
                string fromAddress = "commercialprokerskaramalsalama@gmail.com"; // بريدك الإلكتروني
                string appPassword = "tabr lqzr mavn omvc"; // كلمة مرور التطبيق
                string toAddress = EmailToSend; // البريد الإلكتروني للمستل
                string subject = Subject;
                string fromName = "جومانجو | Gomango";
                
                // تحويل النص العادي إلى HTML مع تصميم برتقالي وخلفية زرقاء غامقة
                string htmlBody = $@"
<!DOCTYPE html>
<html dir='rtl' lang='ar'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, Helvetica, sans-serif; background-color: #f5f5f5;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse; background-color: #f5f5f5; padding: 20px;'>
        <tr>
            <td align='center' style='padding: 20px 0;'>
                <table role='presentation' style='max-width: 600px; width: 100%; border-collapse: collapse; background: linear-gradient(135deg, #ff7a00 0%, #ea580c 100%); border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='padding: 30px; text-align: center; background: linear-gradient(135deg, #ff7a00 0%, #ea580c 100%);'>
                            <h1 style='margin: 0; color: #0a2540; font-size: 28px; font-weight: bold;'>جومانجو</h1>
                            <p style='margin: 5px 0 0 0; color: #13345d; font-size: 16px; font-weight: 600;'>Gomango</p>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 30px; background-color: #fff7ed;'>
                            <div style='color: #0a2540; font-size: 16px; line-height: 1.8;'>
                                {Body.Replace("\n", "<br>")}
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 20px 30px; background: linear-gradient(135deg, #ff7a00 0%, #ea580c 100%); text-align: center;'>
                            <p style='margin: 0; color: #0a2540; font-size: 14px; font-weight: 600;'>
                                مع أطيب التحيات،<br>
                                فريق جومانجو
                            </p>
                            <p style='margin: 10px 0 0 0; color: #13345d; font-size: 12px;'>
                                © {DateTime.Now.Year} جومانجو - جميع الحقوق محفوظة
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress, appPassword)
                };

                MailAddress from = new MailAddress(fromAddress, fromName);
                MailMessage message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    From = from,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                await smtp.SendMailAsync(message);  
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending email: {ex.Message.ToString()}");
            }
        }
    }
}
