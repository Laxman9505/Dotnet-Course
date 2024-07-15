using Dapper;
using DotnetAPI.Dtos;
using DOTNETAPI.Data;
using DOTNETAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOTNETAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsers")]
        public IEnumerable<UserComplete> GetUsers()
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
            return users;
        }
        [HttpGet("GetUser/{userId}")]
        public UserComplete GetUser(int userId)
        {

            string sql = @"EXEC TutorialAppSchema.spUsers_Get @UserId=" + userId.ToString();
            UserComplete user = _dapper.LoadDataSingle<UserComplete>(sql);
            return user;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser([FromBody] UserComplete user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            string sql = $@"EXEC TutorialAppSchema.sp_User_Upsert
                                @UserId=@UserId,
                                @FirstName=@FirstName,
                                @LastName=@LastName,
                                @Gender=@Gender,
                                @Active=@Active,
                                @JobTitle=@JobTitle,
                                @Department=@Department,
                                @Email=@Email
                                ";
            var parameters = new Dictionary<string, object> {
                {"UserId",user.UserId},
                {"FirstName",user.FirstName},
                {"@LastName",user.LastName},
                {"@Gender", user.Gender},
                {"@Active",user.Active},
                {"@JobTitle",user.JobTitle},
                {"@Department", user.Department},
                {"@Email", user.Email}
            };
            if (_dapper.ExecuteSql(sql, new DynamicParameters(parameters)))
            {
                return Ok();
            }
            throw new Exception("Failed to update user");
        }



        [HttpDelete("deleteUser/{userid}")]
        public IActionResult DeleteUser(int userid)
        {
            string sql = @"EXEC TutorialAppSchema.sp_User_DELETE @UserId = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userid);

            if (_dapper.ExecuteSql(sql, parameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete user");
        }


    }
}
