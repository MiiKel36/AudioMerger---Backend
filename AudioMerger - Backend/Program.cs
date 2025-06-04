using AudioMerger___Backend.Classes;
using AudioMerger___Backend.DataBase;
using AudioMerger___Backend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. Lê a chave JWT do appsettings.json =====
string secretKey = "fad801847586af2c11e5be56dd153aeb113d1bbe89e32d6fdd6bb437936b6efaf9ba9f0be24a8f3e45899d228cf579e302e14bc49fc31b35e7d518594ba1945b123e349ad6167f71f95cdcc3988addf9e046ed043bb91942edfc69d077111caa8ab2c32c4d3241e9515c30b1f24eb3a8a8adaf0490a481992029810a8a51df613ef033263aeaafd3662b5fb0e2a5766b88eab99210948ac8d0e2edeb6e3691dee1ce42ed96cc622722ff11ef2ce970a76d79ec959e41870daf099397de552b9729243050ff8c2d0005b43554c4d954c0d8b80cea5d8153f40168d2aaa72e2391dbbc804dfc26e41dd5c69cb86ee905a1d91a11b3681438d3a57726dbc6f6fe5c";
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

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

// ===== 3. Configura autenticação JWT =====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Since GenerateJwtToken doesn't set Issuer/Audience, we skip validation
        ValidateIssuer = false,
        ValidateAudience = false,

        // Critical validations (must match generation)
        ValidateLifetime = true, // Check token expiration (exp)
        ValidateIssuerSigningKey = true, // Verify signature
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes), // Same key as generation
        ClockSkew = TimeSpan.Zero // Strict expiry validation
    };
});

// ===== 4. Outros serviços =====
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

// Ativa autenticação e autorização
app.UseAuthentication(); // <-- IMPORTANTE: antes do UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
