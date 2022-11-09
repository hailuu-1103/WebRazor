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
        [BindProperty] public List<Models.Order> Orders { get; set; }
        public List<String> PagesLink { get; set; } = new List<string>();

        [FromQuery(Name = "txtStartOrderDate")] public DateTime StartDate { get; set; }
        [FromQuery(Name = "txtEndOrderDate")] public DateTime EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool IsShowPrevious => this.CurrentPage > 1;
        public bool IsShowNext => this.CurrentPage < this.TotalPages;

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

        public async Task LoadData(bool paging = true)
        {
            var queryRaw = dbContext.Orders
                .Where(q => true);

            SqlDateTime? sqlStartDate = validateDateTime(StartDate);
            SqlDateTime? sqlEndDate = validateDateTime(EndDate);


            if (sqlStartDate != null)
            {
                queryRaw = queryRaw.Where(q => (DateTime)q.OrderDate >= StartDate);
            }

            if (sqlEndDate != null)
            {
                queryRaw = queryRaw.Where(q => (DateTime)q.OrderDate <= EndDate);
            }

            var query = queryRaw;
            query = query.Include(q => q.Customer).Include(q => q.Employee)
                .OrderByDescending(o => o.OrderDate);

            if (paging)
                query = query.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

            Orders = await query.ToListAsync();

            ViewData["StartDate"] = sqlStartDate != null ? StartDate.Date.ToString("yyyy-MM-dd") : "";
            ViewData["EndDate"] = sqlEndDate != null ? EndDate.Date.ToString("yyyy-MM-dd") : "";


        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id != null)
            {
                CurrentPage = (int)id;
                SqlDateTime? sqlStartDate = validateDateTime(StartDate);
                SqlDateTime? sqlEndDate = validateDateTime(EndDate);
                if (sqlStartDate != null)
                {
                    Orders = dbContext.Orders.Where(o => (DateTime)o.OrderDate >= StartDate).ToList();
                } else if(sqlEndDate != null)
                {
                    Orders = dbContext.Orders.Where(o => (DateTime)o.OrderDate <= EndDate).ToList();
                } else
                {
                    Orders = dbContext.Orders.Include(p => p.Customer).Include(p => p.Employee).ToList();
                }
                Count = Orders.Count;
                Orders = GetPaginatedResult(CurrentPage, PageSize);
            }
            else
            {
                Orders = dbContext.Orders.Include(p => p.Customer).Include(p => p.Employee).ToList();
            }

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

        public List<Models.Order> GetPaginatedResult(int currentPage, int pageSize)
        {
            return Orders.OrderBy(d => d.OrderId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
