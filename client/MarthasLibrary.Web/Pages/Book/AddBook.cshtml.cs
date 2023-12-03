using MarthasLibrary.APIClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarthasLibrary.Web.Pages.Book
{
    [Authorize]
    public class AddBookModel(IMarthasLibraryAPIClient marthasLibraryApiClient) : PageModel
    {

        [BindProperty]
        public Books_Create_Request Book { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await marthasLibraryApiClient.CreateBookAsync(Book);
            return Page();
        }
    }
}
