using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Order
{
    public class DetailModel : PageModel
    {
        public Models.Order Order { get; set;}
        public Models.OrderDetail OrderDetail { get; set;}

        private readonly PRN221DBContext dbContext;

        public int ID;

        public DetailModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<IActionResult> OnGet(int? id)
        {
            ID = (int)id;
            if(!dbContext.Orders.Select(order => order.OrderId).ToList().Contains(ID))
            {
                return NotFound();
            }

            Order = await dbContext.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            OrderDetail = await dbContext.OrderDetails.Include(od => od.Product).FirstOrDefaultAsync(od => od.OrderId == id);

            return Page();
            /*foreach (var item in Order.OrderDetails)
            {
                item.Product = await dbContext.Products.Where(p => p.DeletedAt == null).FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
            }*/
        }
    }
}
