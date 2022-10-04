using WebRazor.Models;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.Services.AddRazorPages();
builder.Services.AddSession(otp => otp.IdleTimeout = TimeSpan.FromMinutes(5));
builder.Services.AddDbContext<PRN221DBContext>();

var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.MapRazorPages();

app.Run();