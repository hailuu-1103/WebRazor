using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebRazor.Materials;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Customer
{
    [Authorize(Roles = "Employee")]
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Models.Customer> Customers { get; set; }

        public List<String> PagesLink { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            Customers = dbContext.Customers.ToList();

            return Page();
        }

        
    }
}
