using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        public EditModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public void OnGet()
        {
        }
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Models.Account Account { get; set; }
        public async Task<IActionResult> OnPost()
        {
            var customer = JsonSerializer.Deserialize<Customer>(HttpContext.Session.GetString("Customer"));
            customer.CompanyName = Customer.CompanyName;
            customer.ContactName = Customer.ContactName;
            customer.ContactTitle = Customer.ContactTitle;
            customer.Address = Customer.Address;
            dbContext.Entry(customer).State = EntityState.Modified;
            var account = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("Account"));
            account.Email = Account.Email;
            dbContext.Entry(account).State = EntityState.Modified;
            dbContext.SaveChanges();

            return RedirectToPage("/index");
        }
    }
}
