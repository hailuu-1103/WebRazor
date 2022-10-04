using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class CartModel : PageModel
    {
        private PRN221DBContext dbContext { get; set; }
        public CartModel(PRN221DBContext context)
        {
            dbContext = context;
        }

        [BindProperty]
        public Models.Product Product { get; set; }
        public void OnGet()
        {
        }
    }
}
