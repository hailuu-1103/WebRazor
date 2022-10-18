using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class DeleteModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        public DeleteModel(PRN221DBContext dBContext)
        {
            this.dbContext = dBContext;
        }
        [BindProperty]
        public Models.Account Auth { get; set; }
        public async Task<IActionResult> OnGet(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return Redirect("/Account/Login");
            }
            var product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id));
            try
            {
                dbContext.Products.Remove(product);
                var count = dbContext.SaveChanges();
                if (count >= 1)
                {
                    TempData["success"] = "Remove product successfully!";

                }
            }
            catch (Exception e)
            {
                TempData["fail"] = e.Message;

            }
            return Redirect("~/Admin/Product/Index");
        }
    }
}
