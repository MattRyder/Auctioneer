using Auctioneer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Sets a message to be displayed as an alert in the page
        /// </summary>
        /// <param name="flashKey">Level of warning to be shown</param>
        /// <param name="message">Feedback to show to the user</param>
        protected void SetFlashMessage(FlashKeyType flashKey, string message)
        {
            ViewData.Add($"alert-{flashKey}", message);
        }
    }
}