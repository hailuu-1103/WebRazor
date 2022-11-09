using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebRazor.Hubs;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class CreateModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;
        public CreateModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public Models.Product Product { get; set; }
        public IActionResult OnGet()
        {
            ViewData["CategoryId"] = new SelectList(dBContext.Categories, "CategoryId", "CategoryName");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            dBContext.Products.Add(Product);
            await dBContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ReloadProduct", await dBContext.Products.ToListAsync());
            return Redirect("~/Admin/Product/1");
        }
    }
}
