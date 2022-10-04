using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        public ProfileModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Models.Account Account { get; set; }
        public void OnGet()
        {
            Account = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("Account"));
            Customer = dbContext.Customers.FirstOrDefault(cus => cus.CustomerId == Account.CustomerId);
            HttpContext.Session.SetString("Customer", JsonSerializer.Serialize(Customer));
        }
    }
}
