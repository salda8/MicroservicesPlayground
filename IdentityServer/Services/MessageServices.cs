using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using StsServerIdentity.Settings;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace StsServerIdentity.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IOptions<EmailSenderOptions> emailSenderOptions;
        private readonly IOptions<SmsSenderOptions> smsSenderOptions;

        public AuthMessageSender(IOptions<EmailSenderOptions> emailSenderOptions, IOptions<SmsSenderOptions> smsSenderOptions)
        {
            this.emailSenderOptions = emailSenderOptions;
            this.smsSenderOptions = smsSenderOptions;
        }

        public async Task SendEmail(string email, string subject, string message, string toUsername)
        {
            var client = new SendGridClient(emailSenderOptions.Value.SendGridKey);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("Joe@contoso.com", "Joe Smith"));
            msg.AddTo(new EmailAddress(email, toUsername));
            msg.SetSubject(subject);
            msg.AddContent(MimeType.Text, message);
            //msg.AddContent(MimeType.Html, message);

            msg.SetReplyTo(new EmailAddress("Joe@contoso.com", "Joe Smith"));

            Response response = await client.SendEmailAsync(msg);
        }

        public Task SendEmailAsync(string email, string subject, string message, string empty)
        {
            var client = new SendGridClient(new SendGridClientOptions { ApiKey = emailSenderOptions.Value.SendGridKey });
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("Joe@contoso.com", "Joe Smith"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }

        public Task SendSmsAsync(string number, string message)
        {
            string accountSid = smsSenderOptions.Value.SMSAccountIdentification;
            // Your Auth Token from twilio.com/console
            string authToken = smsSenderOptions.Value.SMSAccountPassword;

            TwilioClient.Init(accountSid, authToken);

            return MessageResource.CreateAsync(
              to: new PhoneNumber(number),
              from: new PhoneNumber(smsSenderOptions.Value.SMSAccountFrom),
              body: message);
        }
    }
}