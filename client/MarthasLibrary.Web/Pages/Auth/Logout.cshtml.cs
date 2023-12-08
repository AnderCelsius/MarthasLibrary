using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarthasLibrary.Web.Pages.Auth;

public class LoginModel : PageModel
{

    public IActionResult OnGet()
    {
        // Handle GET request (e.g., rendering the login form)
        return Page();
    }

    public IActionResult OnPostLogin()
    {
        // Handle POST request (e.g., authentication logic)
        if (ModelState.IsValid)
        {
            // Implement your authentication logic here
            // For example, validate the user's credentials

            // If authentication is successful, redirect to a different page
            return RedirectToPage("/Dashboard"); // Replace with your desired page
        }

        // If authentication fails, return to the login page with errors
        return Page();
    }
}