using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace TemplatePustokApp.Services;

	public class EmailService
{
    public void SendEmail(string to,string subject,string body)
    {
        // create email message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("tagizadeaysu002@gmail.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
       
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        // send email
        using var smtp = new SmtpClient();
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate("tagizadeaysu002@gmail.com", "dbsm haxg wsjn mrgv");
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
