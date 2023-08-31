using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using myVendingMachine.Application;
using myVendingMachine.Application.Contracts;
using myVendingMachine.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IVendingService, VendingService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VendingMachineDbContext>(option =>
    option.UseSqlite("Filename=C:\\sqlite\\database\\myworks.db")); // builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDbContext<TransactionDetailsDbContext>(option =>
    option.UseSqlite("Filename=C:\\sqlite\\database\\myworks.db")); // builder.Configuration.GetConnectionString("Default")));


builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

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
