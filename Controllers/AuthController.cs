using DOTNETAPI.Data;
using DOTNETAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DOTNETAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;


        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("Register")]

        public IActionResult Regiser([FromBody] UserForRegisterationDto userForRegisteration)
        {
            if (userForRegisteration.Password == userForRegisteration.PasswordConfirm)
            {
                string sqlToCheckIfEmailExists = @"
                    SELECT Email FROM TutorialAppSchema.Auth
                        WHERE Email = @Email
                ";
                var parameters = new
                {
                    Email = userForRegisteration.Email
                };

                IEnumerable<string> existingEmails = _dapper.LoadData<string>(sqlToCheckIfEmailExists);
                if (existingEmails.Count() == 0)
                {

                    return Ok();

                }

                throw new Exception("User already exists");
            }
            throw new Exception("Password and password confirm should be matched.");
        }

        [HttpPost("Login")]

        public IActionResult Login([FromBody] UserForLoginDto userForLogin)
        {
            return Ok();

        }
    }
}