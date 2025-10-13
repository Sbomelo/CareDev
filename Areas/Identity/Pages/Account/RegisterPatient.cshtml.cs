//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//#nullable disable

//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.Extensions.Logging;
//using CareDev.Data;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using CareDev.Models;

//namespace CareDev.Areas.Identity.Pages.Account
//{
//    public class RegisterModel : PageModel
//    {
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IUserStore<ApplicationUser> _userStore;
//        private readonly IUserEmailStore<ApplicationUser> _emailStore;
//        private readonly ILogger<RegisterModel> _logger;
//        private readonly ApplicationDbContext _context;
//        private readonly IEmailSender _emailSender;

//        public RegisterModel(
//            UserManager<ApplicationUser> userManager,
//            IUserStore<ApplicationUser> userStore,
//            SignInManager<ApplicationUser> signInManager,
//            ILogger<RegisterModel> logger,
//            IEmailSender emailSender,
//            ApplicationDbContext context )
//        {
//            _userManager = userManager;
//            _userStore = userStore;
//            _emailStore = (IUserEmailStore<ApplicationUser>)GetEmailStore();
//            _signInManager = signInManager;
//            _logger = logger;
//            _emailSender = emailSender;
//            _context = context;
//        }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        [BindProperty]
//        public InputModel Input { get; set; }

//       // public SelectList GenderList { get; set; }
//        public SelectList MedicationList { get; set; }
//        public SelectList AlleryList { get; set; }
//        public SelectList ChronicConditionList { get; set; }
//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public string ReturnUrl { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public IList<AuthenticationScheme> ExternalLogins { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public class InputModel
//        {

//            [Required]
//            [StringLength(30, ErrorMessage ="The Name Cannnot Be Longer than 30 Characters")] 
//            public string Name{ get; set; } 

//            [Required]
//            [StringLength(30, ErrorMessage = "The Name Cannnot Be Longer than 30 Characters")]
//            public string SurName { get; set; } 

//            [Required]
//            [Range(1, 100, ErrorMessage ="Age Must Be Between 1 AND 100 Years Old")]
//            public int Age { get; set; }

//            [Required]
//            public string Gender { get; set; } 

//            [Required]
//            [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
//            [StringLength(10)]
//            public string PhoneNumber { get; set; }

//            //Lookup tables foreign key
//            //[Display(Name="Gender")]
//            //public int? GenderId { get; set; }

//            [Display(Name="Medication")]
//            public int? MedicationID { get; set; }

//            [Display(Name="Allergy")]
//            public int? AllergyId { get; set; }

//            [Display(Name="Chronic Condition")]
//            public int? ChronicConditionId { get; set; } 

//            [Required]
//            [EmailAddress]
//            [Display(Name = "Email")]
//            public string Email { get; set; }

//            [Required]
//            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
//            [DataType(DataType.Password)]
//            [Display(Name = "Password")]
//            public string Password { get; set; }

//            [DataType(DataType.Password)]
//            [Display(Name = "Confirm password")]
//            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
//            public string ConfirmPassword { get; set; }
//        }


//        public async Task OnGetAsync(string returnUrl = null)
//        {
//           // GenderList = new SelectList(await _context.GenderOptions .ToListAsync(), "GenderId", "GenderType");
//            MedicationList = new SelectList(await _context.Medications.ToListAsync(), "MedicationId", "Name");
//            AlleryList = new SelectList(await _context.Allergies.ToListAsync(), "AllergyId", "Name");
//            ChronicConditionList = new SelectList(await _context.ChronicConditions.ToListAsync(), "ChronicConditionId", "Name");

//            //Make sure Input is not null to avoid null reference exceptions
//            Input ??= new InputModel();

//            ReturnUrl = returnUrl;
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//        }

//        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//        {

//            await PopulateSelectListsAsync();

//            returnUrl ??= Url.Content("~/");
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//            if (ModelState.IsValid)
//            {

//                if ((Input.Email.EndsWith("@site.com", StringComparison.OrdinalIgnoreCase)))
//                {
//                    ModelState.AddModelError(string.Empty, "Admin registration is not allowed here");
//                    return Page();
//                }

//                if ((Input.Email.EndsWith("@Wadmin.com", StringComparison.OrdinalIgnoreCase)))
//                {
//                    ModelState.AddModelError(string.Empty, "Ward Admin registration is not allowed here");
//                    return Page();
//                }

//                if ((Input.Email.EndsWith("@doctor.com", StringComparison.OrdinalIgnoreCase)))
//                {
//                    ModelState.AddModelError(string.Empty, "Doctor registration is not allowed here");
//                    return Page();
//                }

//                if ((Input.Email.EndsWith("@nurse.com", StringComparison.OrdinalIgnoreCase)))
//                {
//                    ModelState.AddModelError(string.Empty, "Nurse registration is not allowed here");
//                    return Page();
//                }

//                var user = new ApplicationUser
//                {
//                    UserName = Input.Email,
//                    Email = Input.Email,
//                    Name = Input.Name,
//                    SurName = Input.SurName,
//                    Age = Input.Age,
//                    Gender = Input.Gender,
//                    MedicationId = Input.MedicationID,
//                    AllergyId = Input.AllergyId,
//                    ChronicConditionId= Input.ChronicConditionId,
//                    EmailConfirmed= true

//                };
//                //var user2 = CreateUser();

//                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
//                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
//                var result = await _userManager.CreateAsync(user, Input.Password);

//                if (result.Succeeded)
//                {
//                    string role = DetermineRoleFromEmail(Input.Email);
//                    await _userManager.AddToRoleAsync(user, role);

//                    _logger.LogInformation("User created a new account with password.");

//                    return RedirectToAction("Portal", "Patients");

//                    var userId = await _userManager.GetUserIdAsync(user);
//                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//                    var callbackUrl = Url.Page(
//                        "/Account/ConfirmEmail",
//                        pageHandler: null,
//                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
//                        protocol: Request.Scheme);

//                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
//                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

//                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
//                    {
//                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
//                    }
//                    else
//                    {
//                        await _signInManager.SignInAsync(user, isPersistent: false);
//                        return LocalRedirect(returnUrl);
//                    }
//                }
//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError(string.Empty, error.Description);
//                }
//            }

//            // If we got this far, something failed, redisplay form
//            return Page();
//        }

//        public async Task PopulateSelectListsAsync()
//        {
//            // Populate the select lists with data from the database
//            MedicationList = new SelectList(await _context.Medications.ToListAsync(), "MedicationID", "Name");
//            AlleryList = new SelectList(await _context.Allergies.ToListAsync(), "AllergyId", "Name");
//            ChronicConditionList = new SelectList(await _context.ChronicConditions.ToListAsync(), "ChronicConditionId", "Name");
//        }

//        private string DetermineRoleFromEmail(string email )
//        {
//            if (email.EndsWith("@Wadmin.com", StringComparison.OrdinalIgnoreCase))
//                return "Error Cannot Register Ward Admin here";
//            else if (email.EndsWith("@doctor.com", StringComparison.OrdinalIgnoreCase))
//                return "Error Cannot Register Ward Admin here";
//            else if (email.EndsWith("@nurse.com", StringComparison.OrdinalIgnoreCase))
//                return "Error Cannot Register Ward Admin here";
//            else
//                return "Patient";

//        }
//        private IdentityUser CreateUser()
//        {
//            try
//            {
//                return Activator.CreateInstance<IdentityUser>();
//            }
//            catch
//            {
//                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
//                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
//                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
//            }
//        }

//        private IUserEmailStore<ApplicationUser> GetEmailStore()
//        {
//            if (!_userManager.SupportsUserEmail)
//            {
//                throw new NotSupportedException("The default UI requires a user store with email support.");
//            }
//            return (IUserEmailStore<ApplicationUser>)_userStore; 
//        }
//    }
//}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CareDev.Data;
using CareDev.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CareDev.Areas.Identity.Pages.Account
{
    public class RegisterPatientModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterPatientModel> _logger;

        public RegisterPatientModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterPatientModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<SelectListItem> RoleList { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(30)]
            public string Name { get; set; }

            [Required]
            [StringLength(30)]
            public string SurName { get; set; }

            [Required]
            [Range(1, 100)]
            public int Age { get; set; }

            [Required]
            public string Gender { get; set; }

            [Required]
            [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits starting with 0.")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Role { get; set; }
        }

        public async Task OnGetAsync()
        {
            RoleList = new List<SelectListItem>();

            foreach (var role in _roleManager.Roles)
            {
                RoleList.Add(new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name
                });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // repopulate roles
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Name = Input.Name,
                SurName = Input.SurName,
                Age = Input.Age,
                Gender = Input.Gender,
                PhoneNumber = Input.PhoneNumber,
                EmailConfirmed = true
            };

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Input.Role);
                _logger.LogInformation($"New {Input.Role} registered.");

                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect based on role
                return Input.Role switch
                {
                    "Admin" => RedirectToAction("Index", "AdminDashboard"),
                    "Nurse" => RedirectToAction("Index", "Patients"),
                    "Doctor" => RedirectToAction("Index", "DoctorDashboard"),
                    "Patient" => RedirectToAction("Index", "PatientPortal"),
                    _ => RedirectToPage("/Index")
                };
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            await OnGetAsync(); // repopulate roles again if errors
            return Page();
        }
    }
}

