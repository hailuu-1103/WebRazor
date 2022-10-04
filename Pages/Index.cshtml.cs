using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.Linq;
using WebRazor.Models;

namespace WebRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public List<Category> Categories { get; set; }

        [BindProperty]
        public List<Models.Product> Products { get; set; } = new List<Models.Product>();
        [BindProperty]
        public List<Models.Product> BestSellerProducts { get; set; } = new List<Models.Product>();
        [BindProperty]
        public List<Models.Product> BestSaleProducts { get; set; } = new List<Models.Product>();

        [BindProperty]
        public List<Models.Product> NewProducts { get; set; } = new List<Models.Product>();

        [FromQuery(Name = "id")]
        public string CatId { get; set; }

        public void OnGet()
        {
            Categories = dbContext.Categories.ToList();

            var products = dbContext.Products.ToList();

            if (CatId != null)
            {
                Products = dbContext.Products
                    .Where(p => p.CategoryId == Int32.Parse(CatId))
                    .ToList();
            }
            else
            {
                var idsBestSale = dbContext.OrderDetails
                    .GroupBy(d => d.ProductId)
                    .Select(g => new {ProductId = g.Key, Sum = g.Sum(d => d.Quantity) })
                    .OrderByDescending(o => o.Sum)
                    .Take(4);
                foreach (var id in idsBestSale)
                {
                    BestSaleProducts.Add(products.First(p => p.ProductId == id.ProductId));
                }

                NewProducts = dbContext.Products
                    .OrderByDescending(p => p.ProductId).Take(4).ToList();

                var product = dbContext.Orders.Join(
                    dbContext.OrderDetails,
                    order => order.OrderId,
                    orderDetail => orderDetail.OrderId,
                    (order, orderDetail) => new
                    {
                        orderDetail.ProductId
                    }).ToList().GroupBy(product => product.ProductId).ToList();
                var productIDToCount = new Dictionary<int, int>();
                foreach(var item in product)
                {
                    productIDToCount.Add(item.Key, item.Count());
                }
                var top4ProductIDToCount =
                    (from entry in productIDToCount orderby entry.Value descending select entry)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value).Take(4);
                foreach(var productID in top4ProductIDToCount)
                {
                    var item = (Models.Product) dbContext.Products.FirstOrDefault(product => product.ProductId == productID.Key);
                    BestSellerProducts.Add(item);
                }

            }
        }

    }
}
