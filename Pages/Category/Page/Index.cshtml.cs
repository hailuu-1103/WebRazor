using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazor.Models;

namespace WebRazor.Pages.Category.Page
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        [BindProperty]
        public List<Models.Category> Categories { get; set; }
        [BindProperty]
        public List<Models.Product> Products { get; set; }
        private int? catID;
        public void OnGet(int? id)
        {
            catID = id;
            Products = dbContext.Products.Where(product => product.CategoryId == catID).ToList();
            Count = Products.Count;
            Categories = dbContext.Categories.ToList();
            if (id != 0)
            {
                Products = GetPaginatedResult(CurrentPage, PageSize);
            }
        }
        public List<Models.Product> GetPaginatedResult(int currentPage, int pageSize)
        {
            return dbContext.Products.Where(product => product.CategoryId == catID).OrderBy(d => d.ProductId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
