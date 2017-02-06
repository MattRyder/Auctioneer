using Auctioneer.Infrastructure.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Auctioneer
{
	public partial class Startup
	{
        public void ConfigureAuthentication(IAppBuilder app)
        {
            app.CreatePerOwinContext(EFDbContext.Create);
            app.CreatePerOwinContext<AuctioneerUserManager>(AuctioneerUserManager.Create);
            app.CreatePerOwinContext<AuctioneerSignInManager>(AuctioneerSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //app.UseTwitterAuthentication(new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions
            //{
            //    ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"],
            //    ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"],
            //    Provider = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationProvider
            //    {
            //        OnAuthenticated = (context) =>
            //        {
            //            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_token", context.AccessToken));
            //            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:twitter:access_secret", context.AccessTokenSecret));
            //            return Task.FromResult(0);
            //        }
            //    },
            //    BackchannelCertificateValidator = new Microsoft.Owin.Security.CertificateSubjectKeyIdentifierValidator(new[]
            //    {
            //        "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
            //        "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
            //        "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
            //        "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
            //        "‎add53f6680fe66e383cbac3e60922e3b4c412bed", // Symantec Class 3 EV SSL CA - G3
            //        "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
            //        "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
            //        "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
            //    })
            //});

        }
	}
}