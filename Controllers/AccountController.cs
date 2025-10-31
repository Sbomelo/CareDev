using CareDev.Models;
using CareDev.Models.ViewModels;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareDev.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordHistoryService _passwordHistory;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IPasswordHistoryService passwordHistory)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _passwordHistory = passwordHistory;
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Find user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            // Always show the same message for security (do not reveal whether email exists)
            var messageShown = "If an account with that email exists, we have sent password reset instructions.";

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Note: do NOT send any info; show same view
                return RedirectToAction(nameof(ForgotPasswordConfirmation), new { message = messageShown });
            }

            // generate token and email with link
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { token, email = model.Email }, protocol: Request.Scheme);

            var emailBody = $@"<p>We received a request to reset your password. Click the link below to choose a new password:</p>
                           <p><a href=""{callbackUrl}"">Reset password</a></p>
                           <p>If you didn't request this, you can ignore this email.</p>";

            await _emailSender.SendEmailAsync(model.Email, "Reset your password", emailBody);

            return RedirectToAction(nameof(ForgotPasswordConfirmation), new { message = messageShown });
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation(string message)
        {
            ViewBag.Message = message ?? "If an account with that email exists, we have sent password reset instructions.";
            return View();
        }

        // GET: /Account/ResetPassword?token=...&email=...
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            if (token == null || email == null) return RedirectToAction(nameof(ForgotPassword));
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // do not reveal that user is missing
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // Check password history BEFORE calling ResetPasswordAsync
            var isInHistory = await _passwordHistory.IsInHistoryAsync(user, model.Password);
            if (isInHistory)
            {
                ModelState.AddModelError(string.Empty, "The chosen password was used recently. Please choose a different password.");
                return View(model);
            }

            // Attempt reset
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                // Save the new hashed password to history (use user.PasswordHash after change)
                // But we must re-fetch the user so we have updated PasswordHash (ResetPasswordAsync updates user)
                var updatedUser = await _userManager.FindByIdAsync(user.Id);
                if (updatedUser != null && !string.IsNullOrEmpty(updatedUser.PasswordHash))
                {
                    await _passwordHistory.AddToHistoryAsync(updatedUser, updatedUser.PasswordHash);
                }

                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
