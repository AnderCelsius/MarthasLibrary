using IdentityModel;
using MarthasLibrary.IdentityServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace MarthasLibrary.IdentityServer.Pages.Account.Register
{
    [AllowAnonymous]
    [SecurityHeaders]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IActionResult OnGet(string returnUrl)
        {
            BuildModel(returnUrl);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // something went wrong, show form with error
                BuildModel(Input.ReturnUrl);
                return Page();
            }

            var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, IsActive = false };
            await _userManager.AddClaimsAsync(user, new Claim[]
            {
                new(JwtClaimTypes.Name, $"{Input.GivenName} {Input.FamilyName}"),
                new(JwtClaimTypes.GivenName, $"{Input.GivenName}"),
                new(JwtClaimTypes.FamilyName, $"{Input.FamilyName}"),
                new(JwtClaimTypes.Role, "Customer"),
            });

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                // create an activation link - we need an absolute URL, therefore
                // we use Url.PageLink instead of Url.Page

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private void BuildModel(string returnUrl)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };
        }
    }
}