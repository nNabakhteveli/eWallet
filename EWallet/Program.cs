using EBank;
using EWallet;
using EWallet.Data;
using EWallet.Domain.Data;
using EWallet.Domain.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IWalletRepository>(s => new WalletRepository(defaultConnectionString));
builder.Services.AddScoped<ITransactionsRepository>(s => new TransactionsRepository(defaultConnectionString));
builder.Services.AddScoped<ITokenRepository>(s => new TokenRepository(defaultConnectionString));

builder.Services.AddScoped<IEBank, EBank.EBank>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(defaultConnectionString);
});

builder.Services.AddDefaultIdentity<UserEntity>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

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
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

DatabaseInitializer.AddStoredProcedures(defaultConnectionString);
app.Run();