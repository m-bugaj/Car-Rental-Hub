﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CarRentalHub.Data;
using Microsoft.AspNetCore.Identity;
using CarRentalHub.Areas.Identity.Data;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.Core.Extensions;
using React.AspNet;
using System.Runtime.ConstrainedExecution;
using System.Globalization;

using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
var configuration = builder.Configuration;

builder.Services.AddDbContext<CarRentalHubContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarRentalHubContext") ?? throw new InvalidOperationException("Connection string 'CarRentalHubContext' not found.")));

// Add DbContext for Identity
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContext' not found.")));

// In the future, change RequireConfirmedAccount to true and handle confirmation of account creation by email.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AuthDbContext>();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddReact();

// Make sure a JS engine is registered, or you will get an error!
builder.Services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
  .AddV8();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<CarAvailabilityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarAvailabilityContext") ?? throw new InvalidOperationException("Connection string 'CarAvailabilityContext' not found.")));
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
    options.SignIn.RequireConfirmedEmail = false;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
});

services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
});

//builder.Services.AddAuthentication()
//   .AddGoogle(options =>
//   {
//       IConfigurationSection googleAuthNSection =
//       config.GetSection("Authentication:Google");
//       options.ClientId = googleAuthNSection["ClientId"];
//       options.ClientSecret = googleAuthNSection["ClientSecret"];
//   });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication(); // Dodaj to

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// ... (reszta Twojego kodu)


//// Initialise ReactJS.NET. Must be before static files.
//app.UseReact(config =>
//{
//    // If you want to use server-side rendering of React components,
//    // add all the necessary JavaScript files here. This includes
//    // your components as well as all of their dependencies.
//    // See http://reactjs.net/ for more information. Example:
//    //config
//    //  .AddScript("~/js/First.jsx")
//    //  .AddScript("~/js/Second.jsx");

//    // If you use an external build too (for example, Babel, Webpack,
//    // Browserify or Gulp), you can improve performance by disabling
//    // ReactJS.NET's version of Babel and loading the pre-transpiled
//    // scripts. Example:
//    //config
//    //  .SetLoadBabel(false)
//    //  .AddScriptWithoutTransform("~/js/bundle.server.js");
//});

//var culture = new CultureInfo("en-US");  // Lub inna kultura, np. "en-US"
//culture.NumberFormat.NumberDecimalSeparator = ".";
//CultureInfo.DefaultThreadCurrentCulture = culture;
//CultureInfo.DefaultThreadCurrentUICulture = culture;

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FilterData}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();