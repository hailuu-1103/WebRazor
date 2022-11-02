using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using WebRazor.Models;

namespace WebRazor.Pages.Cart
{
    public class AddModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public AddModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public Models.Account Auth { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*if (HttpContext.Session.GetString("Account") == null)
            {
                return Redirect("/Account/Login");
            }

            Auth = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("Account"));

            if (Auth == null)
            {
                return Redirect("/Account/Login");
            }*/

            Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id));

            if (product == null || product.UnitsInStock == 0)
            {
                TempData["fail"] = "Out of stock";
            }
            else

                try
                {
                    Dictionary<int, int> list;
                    var session = HttpContext.Session.GetString("cart");

                    if (session == null)
                    {
                        list = new Dictionary<int, int>();
                    }
                    else
                    {
                        list = JsonSerializer.Deserialize<Dictionary<int, int>>(session);
                    }

                    if ((list.Where(p => p.Key == id)).Count() == 0)
                    {
                        list.Add((int)id, 1);
                    }

                    // If the item already in cart
                    foreach(var kvp in list)
                    {
                        if(kvp.Key == id)
                        {
                            var currentValue = kvp.Value;
                            currentValue++;
                            list[kvp.Key] = currentValue;
                        }
                    }
                    var itemCount = list.Count;

                    HttpContext.Session.SetString("cart", JsonSerializer.Serialize(list));
                    HttpContext.Session.SetString("itemCount", JsonSerializer.Serialize(itemCount));
                    
                    TempData["success"] = "Add to cart successfull";
                }
                catch (Exception e)
                {
                    TempData["fail"] = e.Message;
                }

            return Redirect("/Product/Detail/" + id);
        }
    }
}
