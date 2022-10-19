using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public List<Models.Product> Products { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public SelectList? Categories { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Category {get; set;}
        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                Products = dbContext.Products.Include(p => p.Category).Where(p => p.ProductName.Contains(SearchString)).ToList();
            }
            else
            {
                Products = dbContext.Products.Include(p => p.Category).ToList();
            }
            if (!string.IsNullOrEmpty(Category))
            {
                Products = dbContext.Products.Include(p => p.Category).Where(p => p.Category.CategoryName.Equals(Category)).ToList();
            }
            Categories = new SelectList(dbContext.Categories.Select(cat => cat.CategoryName).ToList());

            return Page();
        }
    }
}
