using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;
using WebRazor.Hubs;
using WebRazor.Models;

namespace WebRazor.Pages.Product
{
    public class IndexModel : PageModel
    {
        private PRN221DBContext dbContext;
        private readonly IHubContext<HubServer> hubContext;
        public IndexModel(PRN221DBContext prn221DBContext, IHubContext<HubServer> hubContext)
        {
            this.dbContext = prn221DBContext;
            this.hubContext = hubContext;
        }
        [FromQuery(Name = "txtSearch")] public string SearchString { get; set; } = "";
        [FromQuery(Name = "categoryId")] public int CatId { get; set; } = 0;
        [BindProperty]
        public IFormFile UploadedExcelFile { get; set; }
        public List<Models.Product> Products { get; set; }
        public List<Models.Category> Categories { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool IsShowPrevious => this.CurrentPage > 1;
        public bool IsShowNext => this.CurrentPage < this.TotalPages;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id != null)
            {
                CurrentPage = (int)id;
                if (!string.IsNullOrEmpty(SearchString))
                {
                    Products = dbContext.Products.Include(p => p.Category).Where(p => p.ProductName.Contains(SearchString)).ToList();
                }
                else
                {
                    Products = dbContext.Products.Include(p => p.Category).ToList();
                }
                if (!string.IsNullOrEmpty(Category))
                {
                    Products = dbContext.Products.Include(p => p.Category).Where(p => p.Category.CategoryName.Equals(Category)).ToList();
                }
                Count = Products.Count;
                Categories = await dbContext.Categories.ToListAsync();
                Products = GetPaginatedResult(CurrentPage, PageSize);
            }else
            {
                Products = dbContext.Products.Include(p => p.Category).ToList();
                Categories = await dbContext.Categories.ToListAsync();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            await ReadFile();
            return Page();
        }
        public List<Models.Product> GetPaginatedResult(int currentPage, int pageSize)
        {
            if(Category == null)
            {
                return Products.OrderBy(d => d.ProductId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return Products.Where(product => product.Category?.CategoryName == Category).OrderBy(d => d.ProductId).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            }
        }

        public async Task<bool> ReadFile()
        {
            if (!ModelState.IsValid)
            {
                return false;
            }
            DataSet ds = new DataSet();
            IExcelDataReader reader = null;
            Stream FileStream = null;

            ViewData["Fail"] = "Have some error with data. Need include ProductName, CategoryId, QuantityPerUnit,"
+ " UnitPrice, UnitsInStock, Discontinued";
            ViewData["Success"] = "";

            try
            {
                FileStream = UploadedExcelFile.OpenReadStream();
                if (FileStream != null)
                {
                    if (UploadedExcelFile.FileName.EndsWith(".xls"))
                        reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                    else if (UploadedExcelFile.FileName.EndsWith(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);

                    ds = reader.AsDataSet();
                    reader.Close();
                }

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dtRecords = ds.Tables[0];
                    for (int i = 0; i < dtRecords.Rows.Count; i++)
                    {
                        Models.Product product = new Models.Product();
                        product.ProductName = Convert.ToString(dtRecords.Rows[i][0]);
                        product.CategoryId = dtRecords.Rows[i][1].Equals("NULL") ? null : Convert.ToInt32(dtRecords.Rows[i][1]);
                        product.QuantityPerUnit = dtRecords.Rows[i][2].Equals("NULL") ? null : Convert.ToString(dtRecords.Rows[i][2]);
                        product.UnitPrice = dtRecords.Rows[i][3].Equals("NULL") ? null : Convert.ToDecimal(dtRecords.Rows[i][3]);
                        product.UnitsInStock = dtRecords.Rows[i][4].Equals("NULL") ? null : Convert.ToInt16(dtRecords.Rows[i][4]);
                        product.Discontinued = Convert.ToBoolean(dtRecords.Rows[i][5]);

                        await dbContext.Products.AddAsync(product);
                    }

                    await dbContext.SaveChangesAsync();
                    await hubContext.Clients.All.SendAsync("Reload");
                    ViewData["Fail"] = "";
                    ViewData["Success"] = "Upload Success";
                }
            }
            catch (Exception e)
            {

            }
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> OnGetExport()
        {
            Products = await dbContext.Products.Include(product => product.Category).ToListAsync();
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Sheet1");
                //Header
                ws.Cell(1, 1).Value = "ProductID";
                ws.Cell(1, 2).Value = "ProductName";
                ws.Cell(1, 3).Value = "UnitPrice";
                ws.Cell(1, 4).Value = "Unit";
                ws.Cell(1, 5).Value = "UnitInStock";
                ws.Cell(1, 6).Value = "Category";
                ws.Cell(1, 7).Value = "Discontinued";

                ws.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.Alizarin;

                int i = 2;
                foreach (Models.Product product in Products)
                {
                    ws.Cell(i, 1).Value = product.ProductId.ToString();
                    ws.Cell(i, 2).Value = product.ProductName;
                    ws.Cell(i, 3).Value = ((decimal)product.UnitPrice).ToString("G29");
                    ws.Cell(i, 4).Value = product.QuantityPerUnit;
                    ws.Cell(i, 5).Value = product.UnitsInStock;
                    ws.Cell(i, 6).Value = product.Category.CategoryName;
                    ws.Cell(i, 7).Value = product.Discontinued;
                    i++;
                }
                i--;

                ws.Cells("A1:G" + i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cells("A1:G" + i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cells("A1:G" + i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cells("A1:G" + i).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument-speadsheetml.sheet",
                        "Product.xlsx"
                        );
                }
            }
        }
    }
}
