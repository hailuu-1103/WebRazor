using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
        [BindProperty]
        public IFormFile UploadedExcelFile { get; set; }
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

        public async Task<IActionResult> OnPostImport(IFormFile file) 
        {
            var productInExcel = new List<Models.Product>();
            using (var steam = new MemoryStream())
            {
                await file.CopyToAsync(steam);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(steam))
                {
                    var workSheet = package.Workbook.Worksheets[0];
                    var rowCount = workSheet.Dimension.Rows;
                    for (var row = 2; row <= rowCount; row++)
                    {
                        productInExcel.Add(new Models.Product
                        {
                            ProductId = Convert.ToInt32(workSheet.Cells[row, 1].Value,
                            ProductName = workSheet.Cells[row, 2].Value.ToString().Trim(),
                            UnitPrice = (decimal?)workSheet.Cells[row, 3].Value,
                            QuantityPerUnit = workSheet.Cells[row, 4].Value.ToString().Trim(),
                            UnitsInStock = (short?)workSheet.Cells[row, 5].Value,
                            Category = dbContext.Categories.FirstOrDefault(cat => cat.CategoryName.Equals(workSheet.Cells[row, 6].ToString().Trim())),
                            Discontinued = (bool)workSheet.Cells[row, 7].Value
                        });
                    }
                }
            }
            Products = productInExcel;
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
