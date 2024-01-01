using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Repository;

//You create a new ASP.NET Core Web API project with the command: dotnet new webapi -o PROJECT_NAME
//Command in order to run and see the Swagger UI with the endpoints: dotnet watch run
/*We were able to add through UI the EF Core and other packages thanks to the extension Nuget Gallery, what it does is to make able in VS Code the Nuget Package Manager integrated en Visual Studio*/
var builder = WebApplication.CreateBuilder(args);

//Adding the controllers we have code before
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registrating our ApplicationDbContext service, we must declare the services before the builder.Build in order to make work it

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    options.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IStockRepository, StockRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Mapping the controllers so the Https Redirection error dont show up
app.MapControllers();

app.Run();


