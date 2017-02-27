using Auctioneer.Core.Abstract;
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
    [Authorize]
    public class AccountController : BaseController
    {
        private static readonly string FlashMessageLoginSuccess = "Successfully logged in, welcome back to Auctioneer.";
        private static readonly string FlashMessageLoginFailure = "Failed to login, please review your login details and try again.";

        private AuctioneerSignInManager signInManager;
        private AuctioneerUserManager userManager;
        private EmailServiceBase emailService;

        public IAuthenticationManager AuthManager
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

        public EmailServiceBase EmailService {
            get { return emailService;  }
            private set { emailService = value; }
        }

        public AccountController(AuctioneerUserManager userManager, AuctioneerSignInManager signInManager, EmailServiceBase emailService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            EmailService = emailService;
        }

        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
                return HttpNotFound();

            AccountIndexViewModel viewModel = new AccountIndexViewModel
            {
                Buying = user.Bids.Where(bid => bid.Auction.IsActive()).Select(bid => bid.Auction).Distinct(),
                Selling = user.Auctions.Where(auc => auc.EndDate > DateTime.Now),
                User = user
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            Func<ViewResult> loginFailureFunc = () =>
            {
                SetFlashMessage(FlashKeyType.Danger, FlashMessageLoginFailure);
                return View();
            };

            if (!ModelState.IsValid)
            {
                return loginFailureFunc();
            }

            var loginResult = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            switch (loginResult)
            {
                case SignInStatus.Success:
                    SetFlashMessage(FlashKeyType.Success, FlashMessageLoginSuccess);
                    return RedirectToAction("Index", "Auction");
                default:
                    return loginFailureFunc();
            }
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
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AuctioneerUser { Name = model.Name, UserName = model.Email, Email = model.Email };
                var registrationResult = await UserManager.CreateAsync(user, model.Password);

                // If user registration hits the DB ok, confirm on the UI:
                if (registrationResult.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    SetFlashMessage(FlashKeyType.Success, $"Successfully registered, welcome to Auctioneer, {user.Name}!");
                    return RedirectToAction("Index", "Auction");
                }
                else
                {
                    SetFlashMessage(FlashKeyType.Danger, "Failed to register your account, please verify the information and try again.");
                    foreach(string err in registrationResult.Errors)
                    {
                        if (err.Split(' ').FirstOrDefault().Equals("Email"))
                            ModelState.AddModelError("Email", err);
                    }
                }
            }

            return View(model);
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
        public ActionResult SendPasswordReset()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendPasswordReset(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    string resetCode = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = resetCode }, Request.Url.Scheme);
                    await EmailService.SendEmailAsync(user.Email, user.Name, "Reset your Auctioneer Password", $"Reset your Auctioneer password: <a href='{callbackUrl}'>Click here</a><br/>");
                }
            }

            SetFlashMessage(FlashKeyType.Success, "We've sent a password reset email to that email, check your mail!");
            return RedirectToAction("Index", "Auction");
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            if(User.Identity.IsAuthenticated || code != null)
            {
                return View();
            }

            SetFlashMessage(FlashKeyType.Danger, "Failed to authenticate request, please try again.");
            return RedirectToAction("Index", "Auction");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(model.UserId);
            if(user == null)
            {
                return View(model);
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                SetFlashMessage(FlashKeyType.Success, "Successfully reset your password.");
                return RedirectToAction("Index", "Account");
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthManager.GetExternalLoginInfoAsync();
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
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                SetFlashMessage(FlashKeyType.Success, "Successfully signed in to Auctioneer, welcome back!");
                return RedirectToAction("Index", "Auction");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AuctioneerUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return Redirect(returnUrl);
                    }
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        public ActionResult SignOut()
        {
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            SetFlashMessage(FlashKeyType.Info, "You've been logged out successfully, you can now close the window.");
            return RedirectToAction("Index", "Auction");
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