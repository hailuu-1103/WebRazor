using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Product
{
    public class ImportModel : PageModel
    {
        private PRN221DBContext dbContext;
        public ImportModel(PRN221DBContext context)
        {
            dbContext = context;
        }
        public void OnGet()
        {
        }
        public async Task<List<Models.Product>> OnGetImport(IFormFile file)
        {
            var products = new List<Models.Product>();
            using (var steam = new MemoryStream())
            {
                await file.CopyToAsync(steam);
                using( var package = new ExcelPackage(steam))
                {
                    var workSheet = package.Workbook.Worksheets[0];
                    var rowCount = workSheet.Dimension.Rows;
                    var columnCount = workSheet.Dimension.Columns;
                    for(var row = 2; row <= rowCount; row++)
                    {
                        products.Add(new Models.Product
                        {
                            ProductId = (int)workSheet.Cells[row, 1].Value,
                            ProductName = workSheet.Cells[row, 2].Value.ToString().Trim(),
                            UnitPrice = (decimal?)workSheet.Cells[row, 3].Value,
                            QuantityPerUnit = workSheet.Cells[row, 4].Value.ToString().Trim(),
                            UnitsInStock = (short?)workSheet.Cells[row, 5].Value,
                            Category = dbContext.Categories.FirstOrDefault(cat => cat.CategoryName.Equals(workSheet.Cells[row, 6].ToString().Trim())),
                            Discontinued = (bool)workSheet.Cells[row, 7].Value
                        });
                    }
                    package.Save();

                }
            }
            return products; 
        }
    }
}
