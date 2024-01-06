using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CarRentalHub.Data;
using Microsoft.AspNetCore.Identity;
using CarRentalHub.Areas.Identity.Data;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CarRentalHubContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarRentalHubContext") ?? throw new InvalidOperationException("Connection string 'CarRentalHubContext' not found.")));

// Add DbContext for Identity
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContext' not found.")));

// In the future, change RequireConfirmedAccount to true and handle confirmation of account creation by email.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AuthDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<FilterDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FilterDataContext") ?? throw new InvalidOperationException("Connection string 'FilterDataContext' not found.")));
builder.Services.AddDbContext<PhotoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhotoContext") ?? throw new InvalidOperationException("Connection string 'PhotoContext' not found.")));

// Settings of required password
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
