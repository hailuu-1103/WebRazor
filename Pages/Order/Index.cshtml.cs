using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public Models.Account Auth { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Models.Account acc = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("Account"));

            Auth = await dbContext.Accounts.FirstOrDefaultAsync(c => c.AccountId == acc.AccountId);
            var cus = await dbContext.Customers.ToListAsync();
            var ord = await dbContext.Orders.OrderByDescending(ord => ord.OrderDate).ToListAsync();
            var ordDe = await dbContext.OrderDetails.ToListAsync();
            var pro = await dbContext.Products.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetAdd(int? id)
        {

            var check = HttpContext.User.FindFirst(ClaimTypes.Role);

            if (check != null && check.Value.Equals("Employee"))
            {
                return NotFound();
            }

            if (id == null)
            {
                return NotFound();
            }

            Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id));

            if (product == null || product.UnitsInStock == 0)
            {
                TempData["fail"] = "Quantity = 0";
                return Redirect("/Product/Detail/" + id);
            }
            else
            {
                Dictionary<int, int> list = getCart();

                if ((list.Where(p => p.Key == id)).Count() == 0)
                {
                    list.Add((int)id, 1);
                }


                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(list));
                return Redirect("/Cart/Index");
            }
        }
        private Dictionary<int, int> getCart()
        {
            var cart = HttpContext.Session.GetString("cart");

            Dictionary<int, int> list;

            if (cart != null)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            }
            else
            {
                list = new Dictionary<int, int>();
            }

            return list;
        }


    }
}
