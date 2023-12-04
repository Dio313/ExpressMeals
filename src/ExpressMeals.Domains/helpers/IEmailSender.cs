using System.Net.Mail;

namespace ExpressMeals.Domains.helpers
{
    public interface IEmailSender
    {
        List<MailAddress> GetEmailList(string emails);
        bool IsValidEmail(string email);
        Task<bool> SendMail(string to, string subject, string body, string attachments = null, string cc = null);
    }
}