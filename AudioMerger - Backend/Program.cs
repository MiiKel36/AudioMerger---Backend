using AudioMerger___Backend.Classes;
using AudioMerger___Backend.DataBase;
using AudioMerger___Backend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. L� a chave JWT do appsettings.json =====
var jwtKey = builder.Configuration.GetConnectionString("key"); // Supondo que esteja em "ConnectionStrings:key"
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

// ===== 2. Configura o banco de dados MySQL =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

try
{
    builder.Services.AddDbContext<DataBaseModel>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

}
catch (Exception ex)
{
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

// ===== 3. Configura autentica��o JWT =====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

// ===== 4. Outros servi�os =====
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== 5. Middlewares =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ativa autentica��o e autoriza��o
app.UseAuthentication(); // <-- IMPORTANTE: antes do UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
