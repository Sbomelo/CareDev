using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CareDev.Services
{
    public class CustomSignInManager : SignInManager<IdentityUser>

    {
        public CustomSignInManager
            (
              UserManager<IdentityUser> userManager,
              IHttpContextAccessor contextAccessor,
              IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
              IOptions<IdentityOptions> optionsAccessor,
              ILogger<SignInManager<IdentityUser>> logger,
              IAuthenticationSchemeProvider schemas,
              IUserConfirmation<IdentityUser> confirmation)
            :base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemas, confirmation)
        {

        }
            
    }
}
