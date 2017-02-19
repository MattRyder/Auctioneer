using Auctioneer.Core.Entities;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Infrastructure.SignalR
{
    public class BidHub : Hub
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BidHub>();

        public Task RegisterClient(string auctionId)
        {
            return Groups.Add(Context.ConnectionId, auctionId);
        }


        public static void PushUpdateBidAmount(string auctionId, decimal broadcastAmount)
        {
            hubContext.Clients.Group(auctionId).updateBidAmount(broadcastAmount.ToString("c"));
        }
    }
}
