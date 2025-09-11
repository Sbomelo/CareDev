using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace CareDev.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterAdminModel : PageModel
    { 
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterAdminModel> _logger;

        public RegisterAdminModel(UserManager<IdentityUser> userManager, ILogger<RegisterAdminModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
            {

            [Required]
            [StringLength(30, ErrorMessage = "The Name Cannnot Be Longer than 30 Characters")]
            public string Name { get; set; }

            [Required]
            [StringLength(30, ErrorMessage = "The Name Cannnot Be Longer than 30 Characters")]
            public string SurName { get; set; }

            [Required]
            [Range(1, 100, ErrorMessage = "Age Must Be Between 1 AND 100 Years Old")]
            public int Age { get; set; }

            [Required]
            public string Gender { get; set; }

            [Required]
            [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
            [StringLength(10)]
            public string PhoneNumber { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


        } 
        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                if(!Input.Email.EndsWith("@site.com", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect email used for admin");
                    return Page();
                }
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    _logger.LogInformation("Admin user created by {UserName}.",User.Identity.Name);
                    return LocalRedirect(returnUrl);

                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page(); 
        }
    }
}
