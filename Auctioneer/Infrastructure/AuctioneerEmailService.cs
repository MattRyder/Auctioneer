using Auctioneer.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Auctioneer.Infrastructure.Services
{
    public class AuctioneerEmailService : EmailServiceBase
    {
        private SmtpClient client;
        private string emailFrom;
        private string emailFromDisplay;

        public string EmailFrom
        {
            get { return !string.IsNullOrEmpty(emailFrom) ? emailFrom : "noreply@auctioneer.co"; }
            protected set { emailFrom = value; }
        }

        public string EmailFromDisplay
        {
            get { return !string.IsNullOrEmpty(emailFromDisplay) ? emailFromDisplay : "Auctioneer"; }
            protected set { emailFromDisplay = value; }
        }

        public AuctioneerEmailService(string smtpHost, string smtpPort, string smtpUsername, string smtpPassword)
        {
            this.client = new SmtpClient(smtpHost, int.Parse(smtpPort));
            this.client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            this.client.EnableSsl = false;
        }

        public AuctioneerEmailService(string smtpHost, string smtpPort, string smtpUsername, string smtpPassword, string emailFrom, string emailFromDisplay) : this(smtpHost, smtpPort, smtpUsername, smtpPassword)
        {
            this.emailFrom = emailFrom;
            this.emailFromDisplay = emailFromDisplay;
        }

        public override async Task SendEmailAsync(string recipientEmailAddress, string recipientEmailDisplayName, string emailSubject, string emailBody)
        {
            using(MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(EmailFrom, EmailFromDisplay);
                message.To.Add(new MailAddress(recipientEmailAddress, recipientEmailDisplayName));
                message.Subject = emailSubject;
                message.Body = emailBody;
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);
            }
        }
    }
}
