using AudioMerger___Backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AudioMerger___Backend.DataBase;
using AudioMerger___Backend.Classes;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace AudioMerger___Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataBaseModel _db;
        private readonly IConfiguration _config;

        private JasonWebToken _jwt = new();
        public UsersController(DataBaseModel db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserDbModel request)
        {
            try
            {
                bool canConnect = _db.Database.CanConnect();
                if (!canConnect)
                {
                    string errorMsg = "Cannot connect to data base";

                    LogError(request.UserName, "Users.cs", errorMsg);
                    return BadRequest(errorMsg);
                }

                dynamic hasUser = userExist(request);
                if (hasUser != null)
                    return Conflict(new { message = "Email já cadastrado" });

                _db.Usuarios.Add(request);
                _db.SaveChanges();

                hasUser = userExist(request);

                // Gerar token JWT
                var token = _jwt.GenerateJwtToken(hasUser.UserName, hasUser.UserId) ;

                return Ok(new
                {
                    token,
                    message = "Usuário cadastrado com sucesso"
                });
            }
            catch (Exception ex)
            {
                LogError(request.UserName, "Users.cs", ex.Message);
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }

        [HttpPost("Login")]
        public IActionResult Login(UserDbModel request)
        {
            try
            {
                bool canConnect = _db.Database.CanConnect();
                if (!canConnect)
                {
                    string errorMsg = "Cannot connect to data base";

                    LogError(request.UserName, "Users.cs", errorMsg);
                    return BadRequest(errorMsg);
                }

                dynamic hasUser = userExist(request);
                bool isValid = (hasUser != null) && (hasUser.UserPassword == request.UserPassword);

                if (!isValid)
                    return Conflict(new { message = "Usuário não cadastrado" });

                // Gerar token JWT
                var token = _jwt.GenerateJwtToken(hasUser.UserName, hasUser.UserId);

                return Ok(new
                {
                    token,
                    message = "Login realizado com sucesso"
                });
            }
            catch (Exception ex)
            {
                LogError(request.UserName, "Users.cs", ex.Message);
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }

        [HttpGet("profile")]
        public IActionResult GetProfile(string jwtToken) {

            //var teste = _jwt.ValidateToken(jwtToken);
            var principal = _jwt.ValidateToken(jwtToken);

            // Extract only the data you need
            var result = new
            {
                Username = principal.FindFirst(ClaimTypes.Name)?.Value,
                UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };

            return Ok(result); // Returns { "Username": "alice", "UserId": "123" }
        }

        // Função para verificar se o usuário existe
        [NonAction]
        public dynamic userExist(UserDbModel request)
        {
            return _db.Usuarios.FirstOrDefault(x => x.Email == request.Email);
        }

        // Função auxiliar para log de erros
        [NonAction]
        private void LogError(string user, string file, string message)
        {
            CreateLogs log = new CreateLogs();
            LogModel logModel = new LogModel
            {
                User = user,
                ErrorFile = file,
                ErrorMsg = message,
                Date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
            };

            log.CreateLog(logModel);
        }

    }
}
