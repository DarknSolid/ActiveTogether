using EntityLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using ModelLib;
using WebApp.Areas.Identity;
using WebApp.Entities;
using WebApp.Utils.ExternalLoginProviders.Facebook;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddHttpClient();

// Third party login providers:
//      Facebook:
FacebookConfig facebookConfiguration = new()
{
    AppId = config["ExternalAuthentication:Facebook:AppId"],
    AppSecret = config["ExternalAuthentication:Facebook:AppSecret"]
};
builder.Services.AddSingleton(facebookConfiguration);

//TODO add google

// utilities:
builder.Services.AddScoped<IFacebookLoginProvider, FacebookLoginProvider>();

// add database and repositories:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IApplicationDbContext,ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IDogParkRepository, DogParkRepository>();


//others:
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789-._@+ ÄäÖöÜüẞß";
        }
    )
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthentication()
   .AddFacebook(options =>
   {
       options.ClientId = facebookConfiguration.AppId;
       options.ClientSecret = facebookConfiguration.AppSecret;
   });

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
