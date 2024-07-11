using DOTNETAPI.Data;
using DOTNETAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOTNETAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserJobInfoController : ControllerBase
    {
        DataContextDapper _dapper;


        public UserJobInfoController(IConfiguration config)
        {

            _dapper = new DataContextDapper(config);

        }

        [HttpGet("GetUserJobInfos")]
        public IEnumerable<UserJobInfo> GetUserJobInfos()
        {
            string sql = @"
                SELECT * FROM TutorialAppSchema.UserJobInfo
            ";
            IEnumerable<UserJobInfo> userJobInfos = _dapper.LoadData<UserJobInfo>(sql);
            return userJobInfos;
        }

        [HttpGet("GetUserJobInfo/{userId}")]
        public UserJobInfo GetUserJobInfo(int userId)
        {
            string sql = @"
                SELECT * FROM TutorialAppSchema.UserJobInfo
                        WHERE UserId = @UserId
            ";
            var parameters = new
            {
                UserId = userId
            };

            UserJobInfo userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql, parameters);
            return userJobInfo;
        }

        [HttpPut("EditUserJobInfo")]

        public IActionResult EditUserJobInfo([FromBody] UserJobInfo userJobInfo)
        {
            if (userJobInfo == null)
            {
                return BadRequest("User job info is required");
            }
            string sql = @"
                UPDATE TutorialAppSchema.UserJobInfo
                SET
                    JobTitle = @JobTitle,
                    Department = @Department
                WHERE
                    UserId = @UserId    
            ";
            var parameters = new
            {
                JobTitle = userJobInfo.JobTitle,
                Department = userJobInfo.Department,
                UserId = userJobInfo.UserId
            };
            bool isOperationSuccedd = _dapper.ExecuteSql(sql, parameters);
            if (isOperationSuccedd)
            {
                return Ok();
            }
            throw new Exception("Failed to update Job info");

        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo([FromBody] UserJobInfo userJobInfo)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.UserJobInfo
                (JobTitle, Department)
                VALUES
                    (@JobTitle, @Department)
            ";
            var parameters = new
            {
                JobTitle = userJobInfo.JobTitle,
                Department = userJobInfo.Department
            };

            bool isOperationSuccedd = _dapper.ExecuteSql(sql, parameters);
            if (isOperationSuccedd)
            {
                return Ok();
            }
            throw new Exception("Failed to add user job info");

        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo
                WHERE
                    UserId =@UserId
            ";
            var parameters = new
            {
                UserId = userId.ToString()
            };
            bool isOperationSuccedd = _dapper.ExecuteSql(sql, parameters);
            if (isOperationSuccedd)
            {
                return Ok();
            }
            throw new Exception("Failed to delete user job info");

        }

    }
}