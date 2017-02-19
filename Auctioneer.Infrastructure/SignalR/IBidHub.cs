using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Infrastructure.SignalR
{
    public interface IBidHub
    {
        Task RegisterClient(string auctionId);
        void UpdateBidAmount(string auctionId, decimal broadcastAmount);
        Task DeregisterClient(string auctionId);
    }
}
