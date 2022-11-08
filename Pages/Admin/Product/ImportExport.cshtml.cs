using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebRazor.Pages.Admin.Product
{
    public class ImportExportModel : PageModel
    {
        private IHostingEnvironment hostingEnvironment;
        public ImportExportModel(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public void OnGet()
        {
        }
        
    }
}
