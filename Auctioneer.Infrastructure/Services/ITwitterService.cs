using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Infrastructure.Services
{
    public interface ITwitterService
    {
        Task<string> GetEmailForUserAsync(string accessToken, string accessTokenSecret);
    }
}
