using EntityLib.Entities.Identity;
using EntityLib;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ModelLib.Repositories;
using ModelLib.Utils;
using MudBlazor.Services;
using System.Reflection;
using Web.Server.Utils.ExternalLoginProviders.Facebook;
using MudBlazor;
using FisSst.BlazorMaps.DependencyInjection;
using Entities;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Web.Server;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Files.DataLake;
using Azure.Storage;
using FisSst.BlazorMaps;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var environment = EnvironmentHandler.GetEnvironment(config);

if (environment == EnvironmentHandler.Environment.Local)
{
    // if the environment is develop, then user secrets are automatically added.
    config.AddUserSecrets<Program>();
    builder.WebHost.UseStaticWebAssets();
}


// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


// Third party login providers:
//      Facebook:
FacebookConfig facebookConfiguration = new()
{
    AppId = config["EXTERNALAUTHENTICATION:FACEBOOK:APPID"],
    AppSecret = config["EXTERNALAUTHENTICATION:FACEBOOK:APPSECRET"]
};
builder.Services.AddSingleton(facebookConfiguration);

//TODO add google

// utilities:
builder.Services.AddHttpClient();
builder.Services.AddScoped<IFacebookLoginProvider, FacebookLoginProvider>();

// add database and repositories:
var connectionString = config["POSTGRESQLCONNSTR_LOCAL"];
switch (environment)
{
    case EnvironmentHandler.Environment.Local:
        connectionString = config["POSTGRESQLCONNSTR_LOCAL"];
        break;
    case EnvironmentHandler.Environment.Development:
        //connectionString = config["POSTGRESQLCONNSTR_AZURE_DEVELOP"];
        break;
}

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));

builder.Services.AddSingleton(options =>
{
    var accountName = config["AZURE_STORAGE_ACCOUNT_NAME"];
    var accountKey = config["AZURE_STORAGE_ACCOUNT_KEY"];
    return new StorageSharedKeyCredential(accountName, accountKey);
});

builder.Services.AddSingleton(options =>
{
    Uri accountUri = new Uri(config["AZURE_STORAGE_ACCOUNT_DEV"]);
    var accountName = config["AZURE_STORAGE_ACCOUNT_NAME"];
    var accountKey = config["AZURE_STORAGE_ACCOUNT_KEY"];
    DataLakeServiceClient client = new DataLakeServiceClient(accountUri, new StorageSharedKeyCredential(accountName, accountKey));
    return client;
});

//builder.Services.AddSingleton(options =>
//{
//    Uri accountUri = new Uri(config["AZURE_STORAGE_ACCOUNT_DEV"]);
//    var accountName = config["AZURE_STORAGE_ACCOUNT_NAME"];
//    var accountKey = config["AZURE_STORAGE_ACCOUNT_KEY"];
//    BlobServiceClient client = new BlobServiceClient(accountUri, new DefaultAzureCredential());
//    return client;
//});


builder.Services.AddSingleton<IBlobStorageRepository, BlobStorageRepository>();

builder.Services.AddScoped<IDogParkRepository, DogParkRepository>();
builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<IPlacesRepository, PlacesRepository>();
builder.Services.AddScoped<ICheckInRepository, CheckInRepository>();
builder.Services.AddScoped<IFriendshipsRepository, FriendshipsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IDogInstructorRepository, DogInstructorRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.Configure<EmailClientConfiguration>(config.GetSection("NO-REPLY-EMAIL-CONFIGURATION"));
builder.Services.AddScoped<EmailClient>();


builder.Services.AddSingleton(options =>
{
    var client = new Uri(config["AZURE_STORAGE_ACCOUNT_DEV"]);
    return new DataLakeServiceClient(client, new StorageSharedKeyCredential(config["AZURE_STORAGE_ACCOUNT_NAME"], config["AZURE_STORAGE_ACCOUNT_KEY"]));
});

builder.Services.AddSingleton(options =>
{
    var client = new Uri(config["AZURE_STORAGE_ACCOUNT_DEV"]);
    return new StorageSharedKeyCredential(config["AZURE_STORAGE_ACCOUNT_NAME"], config["AZURE_STORAGE_ACCOUNT_KEY"]);
});

builder.Services.AddSingleton<IBlobStorageRepository, BlobStorageRepository>();


//others:
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789-._@+ ÄäÖöÜüẞß";

    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);


// ===== Configure Identity =======
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "auth_cookie";
    options.Cookie.SameSite = SameSiteMode.None;
    options.LoginPath = new PathString("/api/contests");
    options.AccessDeniedPath = new PathString("/api/contests");

    // Not creating a new object since ASP.NET Identity has created
    // one already and hooked to the OnValidatePrincipal event.
    // See https://github.com/aspnet/AspNetCore/blob/5a64688d8e192cacffda9440e8725c1ed41a30cf/src/Identity/src/Identity/IdentityServiceCollectionExtensions.cs#L56
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DoggoWorld API",
        Description = "The DoggoWorld API to access user data",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddAuthentication()
   .AddFacebook(options =>
   {
       options.ClientId = facebookConfiguration.AppId;
       options.ClientSecret = facebookConfiguration.AppSecret;
   });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
