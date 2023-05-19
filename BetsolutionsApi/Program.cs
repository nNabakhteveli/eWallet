using EWallet.Domain.Data;

var builder = WebApplication.CreateBuilder(args);
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddScoped<ITokenRepository>(s => new TokenRepository(defaultConnectionString));
builder.Services.AddScoped<IWalletRepository>(s => new WalletRepository(defaultConnectionString));
builder.Services.AddScoped<ITransactionsRepository>(s => new TransactionsRepository(defaultConnectionString));
builder.Services.AddScoped<IUserRepository>(s => new UserRepository(defaultConnectionString));
builder.Services.AddScoped<ISeamlessRepository>(s => new SeamlessRepository(defaultConnectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();