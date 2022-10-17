using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class IndexModel : PageModel
    {
        private PRN221DBContext dbContext;
        public IndexModel(PRN221DBContext prn221DBContext)
        {
            this.dbContext = prn221DBContext;
        }

        public List<Models.Product> Products { get; set; } = new();
        public async void OnGet()
        {
            Products = await dbContext.Products.ToListAsync();
        }
    }
}
