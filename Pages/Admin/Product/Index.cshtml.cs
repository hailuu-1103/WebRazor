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
        public string? Category { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool IsShowPrevious => this.CurrentPage > 1;
        public bool IsShowNext => this.CurrentPage < this.TotalPages;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id != 0)
            {
                CurrentPage = (int)id;
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
                Count = Products.Count;
                Categories = new SelectList(dbContext.Categories.Select(cat => cat.CategoryName).ToList());
                Products = GetPaginatedResult(CurrentPage, PageSize);
            }
            return Page();
        }
        public List<Models.Product> GetPaginatedResult(int currentPage, int pageSize)
        {
            if(Category == null)
            {
                return Products.OrderBy(d => d.ProductId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return Products.Where(product => product.Category?.CategoryName == Category).OrderBy(d => d.ProductId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            }
        }
    }
}
