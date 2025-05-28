using AudioMerger___Backend.Classes;
using AudioMerger___Backend.DataBase;
using AudioMerger___Backend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);


// Configurar MySQL com Pomelo
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

try
{
    builder.Services.AddDbContext<DataBaseModel>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))); // Alterado para UseMySql

} catch (Exception ex)
{
    //Logs objects
    CreateLogs log = new CreateLogs();
    LogModel logModel = new LogModel
    {
        User = "- building problem -",
        ErrorFile = "Program.cs",
        ErrorMsg = ex.Message,
        Date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
    };

    log.CreateLog(logModel);
}


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
