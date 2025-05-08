using EventPortal.Constants;
using EventPortal.FormModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

namespace EventPortal.Controllers
{
    public class UserSurfaceController : SurfaceController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IMemberSignInManager _memberSignInManager;
        private readonly IEmailSender _emailSender;

        public UserSurfaceController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IMemberManager memberManager,
            IMemberService memberService,
            IMemberSignInManager memberSignInManager,
            IEmailSender emailSender)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager;
            _memberService = memberService;
            _memberSignInManager = memberSignInManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        [ValidateUmbracoFormRouteString]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            // [Note] this is a docker command to add smtp server
            // docker run -it -p 1025:1025 -p 8025:8025 --name smtpserver axllent/mailpit
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (await _memberManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("", "Email already exists.");
                return CurrentUmbracoPage();
            }

            if (await _memberManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return CurrentUmbracoPage();
            }

            try
            {
                // Create MemberIdentityUser for ASP.NET Identity
                MemberIdentityUser identityUser = new MemberIdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.UserName, 
                    MemberTypeAlias = MemberTypes.Member,
                    IsApproved = true
                };

                IdentityResult memberResult = await _memberManager.CreateAsync(identityUser, model.Password); // this step where password validation is called
                if (!memberResult.Succeeded)
                {
                    foreach (IdentityError error in memberResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return CurrentUmbracoPage();
                }

                _memberService.AssignRole(model.UserName, MemberGroupTypes.RegisteredUsers);

                string? token = await _memberManager.GenerateEmailConfirmationTokenAsync(identityUser);
                string? callbackUrl = Url.Action("ConfirmEmail", "UserSurface", new { userId = identityUser.Id, code = token }, protocol: Request.Scheme);
                string mailBodyTemplate = $@"
                    <html>
                        <body>
                            <h2>Hello,</h2>
                            <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
                            <p><a href=""{callbackUrl}"">Confirm Email</a></p>
                            <p>If you did not request this, please ignore this email.</p>
                        </body>
                    </html>";

                await SendConfirmationEmail(identityUser.Email, "Confirm Registration", mailBodyTemplate);

                return CurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration failed: {ex.Message}");
                return CurrentUmbracoPage();
            }
        }

        [HttpPost]
        [ValidateUmbracoFormRouteString]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            try
            {
                MemberIdentityUser? user = await _memberManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return CurrentUmbracoPage();
                }

                Microsoft.AspNetCore.Identity.SignInResult result = await _memberSignInManager.PasswordSignInAsync(user.UserName!, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Redirect("/"); // Redirect to a protected page
                }

                ModelState.AddModelError("", "Invalid email or password.");
                return CurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login failed: {ex.Message}");
                return CurrentUmbracoPage();
            }
        }

        public async Task SendConfirmationEmail(string toEmail, string subject, string htmlBody)
        {
            MailMessage message = new MailMessage("ahmedalas977@gmail.com", toEmail, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            using SmtpClient client = new SmtpClient("localhost", 1025);
            await client.SendMailAsync(message);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            ConfirmEmailViewModel confirmMail = new ConfirmEmailViewModel();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                confirmMail.Message = "Invalid confirmation link.";
                confirmMail.IsSuccess = false;
                return View("~/Views/UserSurface/ConfirmEmail.cshtml", confirmMail);
            }

            MemberIdentityUser? member = await _memberManager.FindByIdAsync(userId);
            if (member == null)
            {
                confirmMail.Message = "Member not found.";
                confirmMail.IsSuccess = false;
                return View("~/Views/UserSurface/ConfirmEmail.cshtml", confirmMail);
            }

            IdentityResult result = await _memberManager.ConfirmEmailAsync(member, code);
            if (result.Succeeded)
            {
                confirmMail.Message = "Email confirmed successfully. You can now log in.";
                confirmMail.IsSuccess = true;
            }
            else
            {
                confirmMail.Message = "Email confirmation failed.";
                confirmMail.IsSuccess = false;
            }

            return View("~/Views/UserSurface/ConfirmEmail.cshtml", confirmMail);
        }
    }
}
