using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebRazor.Pages.Account
{
    public class ForgotModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            /*if (ModelState.IsValid)
            {
                var account = _context.Persons;
            }*/
            return Redirect("");
        }
    }
}
