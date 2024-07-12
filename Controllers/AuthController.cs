using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DOTNETAPI.Data;
using DOTNETAPI.Dtos;
using DOTNETAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DOTNETAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;

        private readonly AuthHelper authHelper;


        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
            authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]

        public IActionResult Regiser([FromBody] UserForRegisterationDto userForRegisteration)
        {
            if (userForRegisteration.Password == userForRegisteration.PasswordConfirm)
            {
                string sqlToCheckIfEmailExists = @"
                    SELECT Email FROM TutorialAppSchema.Auth
                        WHERE Email = @Email
                ";
                var parameterToCheckIfEmailExists = new
                {
                    Email = userForRegisteration.Email
                };

                IEnumerable<string> existingEmails = _dapper.LoadData<string>(sqlToCheckIfEmailExists, parameterToCheckIfEmailExists);
                if (existingEmails.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = authHelper.GetPasswordHash(userForRegisteration.Password, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth
                            ([Email], [PasswordHash], [PasswordSalt])
                            VALUES
                                (@Email, @PasswordHash, @PasswordSalt)
                    ";
                    var parametersForPasswordHash = new
                    {
                        Email = userForRegisteration.Email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };
                    if (_dapper.ExecuteSql(sqlAddAuth, parametersForPasswordHash))
                    {
                        string sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users
                                (FirstName, LastName, Email, Gender, Active)
                                VALUES
                                    (@FirstName, @LastName, @Email, @Gender, @Active)
                        ";
                        var parametersForAddUser = new
                        {
                            FirstName = userForRegisteration.FirstName,
                            LastName = userForRegisteration.LastName,
                            Email = userForRegisteration.Email,
                            Gender = userForRegisteration.Gender,
                            Active = 1
                        };
                        if (_dapper.ExecuteSql(sqlAddUser, parametersForAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Error Saving User Details !");

                    }
                    throw new Exception("Failed to register user");

                }

                throw new Exception("User already exists");
            }
            throw new Exception("Password and password confirm should be matched.");
        }


        [AllowAnonymous]
        [HttpPost("Login")]

        public IActionResult Login([FromBody] UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"
                SELECT [PasswordHash], [PasswordSalt]
                    FROM TutorialAppSchema.Auth WHERE Email= @Email
            ";
            var parameterForHashAndSalt = new
            {
                Email = userForLogin.Email
            };

            UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt, parameterForHashAndSalt);
            byte[] passwordHash = authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
                {
                    return StatusCode(200, "Incorrect Password !");
                }
            }
            string getUserIdSql = @"
                    SELECT UserId From TutorialAppSchema.Users
                        WHERE Email= @Email
             ";
            var parametsForUserId = new
            {
                Email = userForLogin.Email
            };

            int userId = _dapper.LoadDataSingle<int>(getUserIdSql, parametsForUserId);
            Dictionary<string, string> loginResponse = new Dictionary<string, string> {
                {
                    "token", authHelper.CreateToken(userId)
                }
             };

            return Ok(loginResponse);

        }

        [HttpGet("RefreshToken")]

        public string RefreshToken()
        {
            string userIdSql = @"
                    SELECT UserId FROM TutorialAppSchema.Users 
                        WHERE UserId= @UserId
            ";
            var parameters = new
            {
                UserId = User.FindFirst("userId")?.Value
            };

            int userId = _dapper.LoadDataSingle<int>(userIdSql, parameters);
            return authHelper.CreateToken(userId);
        }

    }
}