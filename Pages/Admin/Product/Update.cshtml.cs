using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class UpdateModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        public UpdateModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty]
        public Models.Product Product { get; set; }
        public void OnGet(int? id)
        {
            ViewData["CategoryId"] = new SelectList(dbContext.Categories, "CategoryId", "CategoryName");
            Product = dbContext.Products.Include(p => p.Category).FirstOrDefault(product => product.ProductId == id);
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var productToUpdate = await dbContext.Products.FindAsync(id);
                if(productToUpdate == null)
                {
                    return NotFound();
                }
                if(await TryUpdateModelAsync<Models.Product>(productToUpdate, "product", 
                    s => s.ProductName, 
                    s => s.UnitPrice,
                    s => s.QuantityPerUnit,
                    s => s.UnitsInStock,
                    s => s.CategoryId))
                {
                    var count = await dbContext.SaveChangesAsync();
                    if (count > 0)
                    {
                        TempData["success"] = "Update a product successfully";
                    }
                }
                
            }
            catch(Exception e)
            {
                TempData["fail"] = e.Message;
            }
            return RedirectToPage("./Index");
        }
    }
}
