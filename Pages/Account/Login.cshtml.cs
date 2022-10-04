using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public LoginModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public Models.Account Account { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var acc = await dbContext.Accounts
                .SingleOrDefaultAsync(a => a.Email.Equals(Account.Email) && a.Password.Equals(Account.Password));

            if (acc == null)
            {
                ViewData["msg"] = "Email/ Password is wrong";
                return Page();
            }


            HttpContext.Session.SetString("Account", JsonSerializer.Serialize(acc));

            return RedirectToPage("/index");
        }
    }
}
