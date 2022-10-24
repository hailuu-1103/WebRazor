using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using WebRazor.Materials;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Order
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        [FromQuery(Name = "page")] public int Page { get; set; } = 1;
        [BindProperty] public List<Models.Order> Orders { get; set; }
        public List<String> PagesLink { get; set; } = new List<string>();

        [FromQuery(Name = "txtStartOrderDate")] public DateTime StartDate { get; set; }
        [FromQuery(Name = "txtEndOrderDate")] public DateTime EndDate { get; set; }
        private int perPage = 10;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private SqlDateTime? validateDateTime(DateTime time)
        {
            DateTime Min = (DateTime)SqlDateTime.MinValue;
            DateTime Max = (DateTime)SqlDateTime.MaxValue;
            

            if (time >= Min && time <= Max)
            {
                return SqlDateTime.Parse(time.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return null;
        }

        public async Task LoadData()
        {
            var query = dbContext.Orders
                .Where(q => true);

            SqlDateTime? sqlStartDate = validateDateTime(StartDate);
            SqlDateTime? sqlEndDate = validateDateTime(EndDate);


            if (sqlStartDate != null)
            {
                query = query.Where(q => (DateTime)q.OrderDate >= StartDate);
            }

            if (sqlEndDate != null)
            {
                query = query.Where(q => (DateTime)q.OrderDate <= EndDate);
            }

            Orders = await query
                .Include(q => q.Customer).Include(q => q.Employee)
                .OrderByDescending(o => o.OrderDate)
                .Skip((Page - 1) * perPage).Take(perPage)
                .ToListAsync();

            ViewData["StartDate"] = sqlStartDate != null ? StartDate.Date.ToString("yyyy-MM-dd") : "";
            ViewData["EndDate"] = sqlEndDate != null ? EndDate.Date.ToString("yyyy-MM-dd") : "";

            /*PageLink page = new PageLink(perPage);*/
            String param = (!(ViewData["StartDate"].Equals("") && ViewData["EndDate"].Equals("")) 
                ? "txtStartOrderDate=" + ViewData["StartDate"] 
                + "&txtEndOrderDate=" + ViewData["EndDate"] : "") + "&";
            /*PagesLink = page.getLink(Page, await query.CountAsync(), "/Admin/Order/Index?" + param);*/
        }


        public async Task<IActionResult> OnGetAsync()
        {
            await LoadData();

            return Page();
        }

        public async Task<IActionResult> OnGetCancel(int? id, string target)
        {
            Models.Order order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

            await LoadData();

            if (order == null)
            {
                return Redirect(target);
            }
            order.RequiredDate = null;
            await dbContext.SaveChangesAsync();
            return Redirect(target);
        }
    }
}
