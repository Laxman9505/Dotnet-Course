using DotnetAPI.Dtos;
using DOTNETAPI.Data;
using DOTNETAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOTNETAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            string sql = @"SELECT  [UserId]
                            , [FirstName]
                            , [LastName]
                            , [Email]
                            , [Gender]
                            , [Active]
                        FROM  TutorialAppSchema.Users";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;
        }
        [HttpGet("GetUser/{userId}")]
        public User GetUser(int userId)
        {

            string sql = @"SELECT  [UserId]
                            , [FirstName]
                            , [LastName]
                            , [Email]
                            , [Gender]
                            , [Active]
                        FROM  TutorialAppSchema.Users
                            WHERE UserId = " + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
            return user;
        }

        [HttpPut("editUser")]
        public IActionResult EditUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            string sql = $@"
            UPDATE TutorialAppSchema.Users
            SET 
                FirstName = '{user.FirstName}',
                LastName = '{user.LastName}',
                Email = '{user.Email}',
                Gender = '{user.Gender}',
                Active = {(user.Active ? 1 : 0)}
            WHERE
                UserId = '{user.UserId}'    
            ";
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to update user");
        }

        [HttpPost("/addUser")]
        public IActionResult AddUser([FromBody] UserToAddDto user)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.Users
                (FirstName, LastName, Email, Gender, Active)
                VALUES
                (@FirstName, @LastName, @Email, @Gender, @Active)
            ";

            Console.WriteLine(sql);

            var parameters = new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                user.Gender,
                user.Active
            };

            if (_dapper.ExecuteSql(sql, parameters))
            {
                return Ok();
            }
            Console.WriteLine(user.FirstName);
            throw new Exception("Failed to add user");
        }

        [HttpDelete("deleteUser/{userid}")]
        public IActionResult DeleteUser(int userid)
        {
            string sql = $@"DELETE FROM TutorialAppSchema.Users WHERE UserId = @UserId";
            var parameters = new { UserId = userid };
            if (_dapper.ExecuteSql(sql, parameters))
            {
                return Ok();
            }
            throw new Exception("Failed to delete user");
        }

    }
}
