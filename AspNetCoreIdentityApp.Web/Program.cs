using AspNetCoreIdentityApp.Core.OptionsModels;
using AspNetCoreIdentityApp.Core.PermissionsRoot;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Repository.Seeds;
using AspNetCoreIdentityApp.Web.ClaimProvider;
using AspNetCoreIdentityApp.Web.Extansions;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.Service.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentityApp.Service.TwoFactorService;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"),options
        =>
    {
        options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
    });
});


builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});



builder.Services.Configure<TwoFactorOptions>(builder.Configuration.GetSection("TwoFactorOptions"));


// Uygulama hizmetlerine (Dependency Injection container'a) bir dosya sa�lay�c�s� (IFileProvider) ekler.  
// PhysicalFileProvider, belirtilen dizindeki (bu durumda, uygulaman�n �al��t��� dizin) fiziksel dosya sistemine eri�im sa�lar.  
// Bu sayede uygulama, �al��ma dizinindeki dosya ve dizinleri y�netebilir.
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddIdentityWithExtensions();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation,UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<EmailSender>();

builder.Services.AddScoped<TwoFactorService, TwoFactorService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "MainSession";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "ankara"); 
    });
    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });
    options.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement() { ThresholAge=18});
    });
    options.AddPolicy("OrderPermissionReadAndDelete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Read, Permissions.Order.Delete, Permissions.Stock.Delete);
    });
});

builder.Services.AddAuthentication()
    .AddFacebook(opt =>
    {
        opt.AppId = "appId giriniz";
        opt.AppSecret = "appSecret giriniz";
    }).AddGoogle(opt =>
    {
        opt.ClientId = "clientId giriniz";
        opt.ClientSecret = "ClientSecret giriniz";
    });

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";
    opt.LoginPath = new PathString("/Home/SignIn");
    opt.LogoutPath = new PathString("/Member/Logout");
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    opt.SlidingExpiration = true;
});

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();
app.UseAuthentication();    
app.UseAuthorization();



app.MapControllerRoute(
    name: "areas",
   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
