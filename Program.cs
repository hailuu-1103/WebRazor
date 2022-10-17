using WebRazor.Hubs;
using WebRazor.Models;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.Services.AddRazorPages();
builder.Services.AddSession(otp => otp.IdleTimeout = TimeSpan.FromMinutes(5));
builder.Services.AddDbContext<PRN221DBContext>();
builder.Services.AddSignalR();
var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapHub<HubServer>("/hubs");
app.Run();