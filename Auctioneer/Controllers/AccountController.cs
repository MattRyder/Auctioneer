using Auctioneer.Core.Entities;
using Auctioneer.Infrastructure.Entities;
using Auctioneer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Controllers
{
    public class AccountController : Controller
    {
        private AuctioneerSignInManager signInManager;
        private AuctioneerUserManager userManager;

        public IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public AuctioneerSignInManager SignInManager
        {
            get { return signInManager ?? HttpContext.GetOwinContext().Get<AuctioneerSignInManager>(); }
            private set { signInManager = value; }
        }

        public AuctioneerUserManager UserManager
        {
            get { return userManager ?? HttpContext.GetOwinContext().GetUserManager<AuctioneerUserManager>(); }
            private set { userManager = value; }
        }

        public AccountController(AuctioneerUserManager userManager, AuctioneerSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Redirect(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    //if (loginInfo.Login.LoginProvider.Equals("twitter"))
                    //{
                    //    var twitterResult = await twitterService.GetEmailForUserAsync("", "");
                    //}
                    return Redirect(returnUrl);
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary["XsrfId"] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}