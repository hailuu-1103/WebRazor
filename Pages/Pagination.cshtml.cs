using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazor.Models;

namespace WebRazor.Pages
{
    public class PaginationModel : PageModel
    {

        private readonly PRN221DBContext dbContext;
        public PaginationModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        [BindProperty]
        public List<Category> Categories { get; set; }
        [BindProperty]
        public List<Models.Product> Products { get; set; }
        [FromQuery(Name = "id")]
        public string CatId { get; set; }
        public void OnGet()
        {
            Categories = dbContext.Categories.ToList();
            if (CatId != null)
            {
                Products = GetPaginatedResult(CurrentPage, PageSize);
                Count = GetCount();
            }
        }
        public List<Models.Product> GetPaginatedResult(int currentPage, int pageSize)
        {
            return dbContext.Products.Where(product => product.CategoryId == int.Parse(CatId)).OrderBy(d => d.ProductId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }
        public int GetCount()
        {
            return dbContext.Products.Count();
        }
    }
}
