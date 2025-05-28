using AudioMerger___Backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AudioMerger___Backend.DataBase;
using AudioMerger___Backend.Classes;
using System.Net;

namespace AudioMerger___Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataBaseModel _db;

        public UsersController(DataBaseModel db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Register(UserDbModel request)
        {
            try
            {
                bool canConnect = _db.Database.CanConnect();
                if (!canConnect)
                {
                    string errorMsg = "Cannot connect to data base";

                    //Logs objects
                    CreateLogs log = new CreateLogs();
                    LogModel logModel = new LogModel
                    {
                        User = request.UserName,
                        ErrorFile = "Users.cs",
                        ErrorMsg = errorMsg,
                        Date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
                    };

                    log.CreateLog(logModel);

                    return BadRequest(errorMsg);
                }

                dynamic hasUser = userExist(request);
                if (hasUser != null) return Conflict(new { message = $"Email ja cadastrado" });

                _db.Usuarios.Add(request);
                _db.SaveChanges();

                return Ok("Usuario cadastrado no banco de dados");
            }
            catch (Exception ex)
            {
                //Logs objects
                CreateLogs log = new CreateLogs();
                LogModel logModel = new LogModel
                {
                    User = request.UserName,
                    ErrorFile = "Users.cs",
                    ErrorMsg = ex.Message,
                    Date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
                };

                log.CreateLog(logModel);

                return BadRequest("Somethin went wrong " + ex.Message);
            }

        }

        [HttpGet]
        public IActionResult Login(UserDbModel request)
        {
            try 
            {
                bool canConnect = _db.Database.CanConnect();
                if (!canConnect)
                {
                    string errorMsg = "Cannot connect to data base";

                    //Logs objects
                    CreateLogs log = new CreateLogs();
                    LogModel logModel = new LogModel
                    {
                        User = request.UserName,
                        ErrorFile = "Users.cs",
                        ErrorMsg = errorMsg,
                        Date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
                    };

                    log.CreateLog(logModel);

                    return BadRequest(errorMsg);
                }

                dynamic hasUser = userExist(request);
                bool isValid = (hasUser != null) || (hasUser.UserPassword == request.UserPassword);

                if (!isValid) return Conflict(new { message = $"Usuario não cadastrado cadastrado" });
                
                return Ok("User exist on database");
            }
            catch (Exception ex)
            {
                //Logs objects
                CreateLogs log = new CreateLogs();
                LogModel logModel = new LogModel
                {
                    User = request.UserName,
                    ErrorFile = "Users.cs",
                    ErrorMsg = ex.Message,
                    Date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
                };

                log.CreateLog(logModel);

                return BadRequest("Somethin went wrong " + ex.Message);
            }

        }



        //Funcitions
        [NonAction]
        public dynamic userExist(UserDbModel request)
        {
            return _db.Usuarios.FirstOrDefault(x => x.Email == request.Email);        
        }
    }
}