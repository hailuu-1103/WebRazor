using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        private PRN221DBContext dbContext;
        public IndexModel(PRN221DBContext context)
        {
            dbContext = context;
        }
        public List<OrderInMonth> ordersInMonth { get; set; } = new();
        public void OnGet()
        {
            for(var i = 1; i <= 12; i++)
            {
                ordersInMonth.Add(new OrderInMonth() { orders = dbContext.Orders.Where(order => order.OrderDate.Value.Month == i && order.OrderDate.Value.Year == DateTime.Today.Year).ToList()}); 
            }
        }
        public class OrderInMonth
        {
            public List<Models.Order> orders = new();
        }
    }
}
