using AzureStorageLibrary;
using AzureStorageLibrary.Services;
using WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConnectionString.AzureStorageConnectionString = builder.Configuration.GetConnectionString("StorageConStr");

builder.Services.AddScoped(typeof(INoSqlStorage<>), typeof(TableStorage<>));
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
