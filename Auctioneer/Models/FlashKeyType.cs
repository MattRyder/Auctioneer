using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auctioneer.Models
{
    /// <summary>
    /// Each possible type of feedback flash message
    /// Derived and based off Bootstrap alert classes
    /// </summary>
    public enum FlashKeyType
    {
        Info,
        Warning,
        Danger,
        Success
    };
}