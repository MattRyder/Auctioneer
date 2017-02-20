using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Core.Abstract
{
    public abstract class EmailServiceBase
    {
        abstract public Task SendEmailAsync(string recipientEmailAddress, string recipientEmailDisplayName, string emailSubject, string emailBody);
    }
}
