using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vereyon.Web;

namespace Auctioneer.Infrastructure
{
    /// <summary>
    /// The sanitizer for HTML coming into the app, via Auction Description etc.
    /// </summary>
    public class AuctioneerHtmlSanitizer : HtmlSanitizer
    {
        public AuctioneerHtmlSanitizer()
        {
            this.Tag("strong").RemoveEmpty();

            this.Tag("b").Rename("strong").RemoveEmpty();

            this.Tag("i").RemoveEmpty();

            // nofollow the urls to prevent any spambots trying anything clever
            this.Tag("a").SetAttribute("target", "_blank")
                .SetAttribute("rel", "nofollow")
                .CheckAttribute("href", HtmlSanitizerCheckType.Url)
                .RemoveEmpty();

            this.Tag("ul").RemoveEmpty();

            this.Tag("p").RemoveEmpty();
        }
    }
}