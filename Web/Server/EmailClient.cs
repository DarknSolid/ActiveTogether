using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using EntityLib.Entities.Identity;
using ModelLib.ApiDTOs;

namespace Web.Server
{
    public class EmailClient
    {
        private readonly EmailClientConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public EmailClient(IOptions<EmailClientConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _smtpClient = new SmtpClient()
            {
                Host = _configuration.SmtpServer,
                Port = _configuration.SmtpPort,
                Credentials = new NetworkCredential(_configuration.Email, _configuration.Password),
                
                EnableSsl = true,
            };
        }

        public void SendEmailConfirmationMail(string recipient, string userFirstName, string tokenUrl, string webAppBaseUri)
        {
            string html = File.ReadAllText("./Html/ConfirmEmail.html");
            html = html.Replace("_REPLACE_CONFIRM_LINK", tokenUrl);
            html = html.Replace("_REPLACE_BASE_URL", webAppBaseUri);
            html = html.Replace("_REPLACE_USER_NAME", userFirstName);
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration.Email),
                Subject = "Confirm your email - DoggoWorld",
                Body = html,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(recipient);
            _smtpClient.Send(mailMessage);
        }

        public void SendChangeEmailConfirmationMail(string recipient, string newEmail, string userFirstName, string tokenUrl, string webAppBaseUri)
        {
            string html = File.ReadAllText("./Html/ChangeEmail.html");
            html = html.Replace("_REPLACE_CONFIRM_LINK", tokenUrl);
            html = html.Replace("_REPLACE_BASE_URL", webAppBaseUri);
            html = html.Replace("_REPLACE_USER_NAME", userFirstName);
            html = html.Replace("_REPLACE_NEW_EMAIL", newEmail);
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration.Email),
                Subject = "Confirm changing your email - DoggoWorld",
                Body = html,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(recipient);
            _smtpClient.Send(mailMessage);
        }

        public void SendFeedback(ApplicationUser? user, FeedbackCreateDTO dto)
        {
            string html = File.ReadAllText("./Html/Feedback.html");
            var username = user is null ? "anonymous" : user?.FirstName + " " + user?.LastName;
            html = html.Replace("_REPLACE_AUTHOR", username);
            html = html.Replace("_REPLACE_USER_ID", user?.Id.ToString() ?? "anonymous");
            html = html.Replace("_REPLACE_USER_EMAIL", user?.Email ?? "anonymous");
            html = html.Replace("_REPLACE_SEVERITY", dto.Severity.ToString());
            html = html.Replace("_REPLACE_URI", dto.Uri);
            html = html.Replace("_REPLACE_TITLE", dto.Title);
            html = html.Replace("_REPLACE_DESCRIPTION", dto.Description.Replace("\n", "<br/>"));
            html = html.Replace("_REPLACE_MAY_CONTACT", dto.MayContact.ToString());

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration.Email),
                Subject = "Feedback - " + dto.Severity.ToString(),
                Body = html,
                Priority = dto.Severity == FeedbackSeverity.Bug ? MailPriority.High : MailPriority.Normal,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(_configuration.FeedbackEmailRecipient);
            _smtpClient.Send(mailMessage);
        }

        public void SendPasswordResetEmail(string recipient, string userFirstName, string tokenUrl, string webAppBaseUri)
        {
            string html = File.ReadAllText("./Html/ResetPassword.html");
            html = html.Replace("_REPLACE_CONFIRM_LINK", tokenUrl);
            html = html.Replace("_REPLACE_BASE_URL", webAppBaseUri);
            html = html.Replace("_REPLACE_USER_NAME", userFirstName);
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration.Email),
                Subject = "Reset Password - DoggoWorld",
                Body = html,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(recipient);
            _smtpClient.Send(mailMessage);
        }
    }

    public class EmailClientConfiguration
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string FeedbackEmailRecipient { get; set; }

        public EmailClientConfiguration() { }

        public EmailClientConfiguration(string email, string password, string smtpServer, int smtpPort, string feedbackEmailRecipient)
        {
            Email = email;
            Password = password;
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            FeedbackEmailRecipient = feedbackEmailRecipient;
        }
    }
}
