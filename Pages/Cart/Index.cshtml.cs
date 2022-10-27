using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Specialized;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Cart
{
    public class IndexModel : PageModel
    {
        public Dictionary<Models.Product, int> Cart { get; set; } = new Dictionary<Models.Product, int>();

        public Models.Account Auth { get; set; }
        public Models.Customer Customer { get; set; }

        public Anonymous Anym { get; set; }

        public decimal Sum { get; set; } = 0;

        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> OnGet()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (HttpContext.Session.GetString("Account") != null)
            {
                Auth = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("Account"));
                Customer = dbContext.Customers.FirstOrDefault(cus => cus.CustomerId == Auth.CustomerId);
            }
            Dictionary<int, int> list;
            if (cart != null)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            } else
            {
                list = new Dictionary<int, int>();
            }

            foreach(var item in list)
            {
                Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.Key));

                Cart.Add(product, item.Value);

                Sum += (decimal) product.UnitPrice * item.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (HttpContext.Session.GetString("Account") != null)
            {
                Auth = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("Account"));
            }
            Dictionary<int, int> list;
            if (cart != null)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            }
            else
            {
                list = new Dictionary<int, int>();
            }
            try
            {
                Models.Order order = new Models.Order();
                order.CustomerId = Auth == null ? null : Auth.CustomerId;
                order.OrderDate = DateTime.Now;
                order.RequiredDate = DateTime.Today.AddDays(3);
                if(Anym != null)
                {
                    order.ShipName = Anym.ContactName;
                    order.ShipAddress = Anym.Address;
                    order.ShipCity = Anym.ContactName;
                }
                else
                {
                    order.ShipName = Customer.ContactName;
                    order.ShipAddress = Customer.Address;
                    order.ShipCity = Customer.ContactName;
                }
                await dbContext.Orders.AddAsync(order);
                await dbContext.SaveChangesAsync();
                order = await dbContext.Orders.OrderBy(o => o.OrderDate).LastOrDefaultAsync();

                foreach(var item in list)
                {
                    Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.Key));
                    OrderDetail od = new OrderDetail();
                    od.OrderId = order.OrderId;
                    od.ProductId = product.ProductId;
                    od.Quantity = (short)item.Value;
                    od.UnitPrice = (decimal)product.UnitPrice;
                    od.Discount = 0;
                    await dbContext.OrderDetails.AddAsync(od);
                    product.UnitsInStock -= (short)item.Value;
                    dbContext.Update(product);
                }
                await dbContext.SaveChangesAsync();

                ViewData["success"] = "Order successfull";

                HttpContext.Session.Remove("cart");
            }
            catch (Exception e)
            {
                ViewData["fail"] = e.Message;
            }

            return Page();
        }

        public class Anonymous
        {
            public string CompanyName { get; set; } = null!;
            public string? ContactName { get; set; }
            public string? ContactTitle { get; set; }
            public string? Address { get; set; }
        }
    }
}
