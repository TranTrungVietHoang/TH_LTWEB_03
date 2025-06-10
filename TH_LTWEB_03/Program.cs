using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TH_LTWEB_03.Models;

using TH_LTWEB_03.Repositories;
using Microsoft.AspNetCore.Identity.UI;     
using TranTrungVietHoang.Models;

var builder = WebApplication.CreateBuilder(args);


//  builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

//app.MapControllerRoute(
//  name: "Admin",
//  pattern: "{controller=Products}/{action=Index}/{id?}");

//app.MapControllerRoute(
// name: "default",
// pattern: "{controller=Home}/{action=Index}/{id?}");



app.UseEndpoints(endpoints => {
    
    endpoints.MapControllerRoute(
       name: "Customer",
       pattern: "{area:exists}/{controller=User}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});





/*app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});*/

/*using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Tạo các Role nếu chưa tồn tại
    string[] roleNames = { SD.Role_Admin, SD.Role_Customer, SD.Role_Company, SD.Role_Employee };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Tạo tài khoản Admin mặc định
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Admin User",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
        }
    }
}*/



app.Run();
