
using System.Text;
using Dapper;
using DOTNETAPI.Data;
using DOTNETAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOTNETAPI.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {

        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId, int userId, string searchParam = "None")
        {
            var sql = new StringBuilder("EXEC TutorialAppSchema.sp_Posts_Get");
            var parameters = new DynamicParameters();

            bool firstCondition = true;

            if (postId != 0)
            {
                if (!firstCondition) sql.Append(",");
                sql.Append(" @PostId = @PostId");
                parameters.Add("PostId", postId);
                firstCondition = false;
            }

            if (userId != 0)
            {
                if (!firstCondition) sql.Append(",");
                sql.Append(" @UserId = @UserId");
                parameters.Add("UserId", userId);
                firstCondition = false;
            }

            if (searchParam != "None")
            {
                if (!firstCondition) sql.Append(",");
                sql.Append(" @SearchValue = @SearchValue");
                parameters.Add("SearchValue", searchParam);
            }

            return _dapper.LoadData<Post>(sql.ToString(), parameters);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.sp_Posts_Upsert @UserId=@UserId";
            var parameters = new
            {
                UserId = User.FindFirst("userId")?.Value
            };
            return _dapper.LoadData<Post>(sql, parameters);
        }
        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post post)
        {
            StringBuilder sql = new StringBuilder(@"EXEC TutorialAppSchema.sp_Posts_Upsert ");
            var parameters = new DynamicParameters();
            bool isFirstParameterInserted = false;

            if ((User.FindFirst("userId")?.Value ?? "").Length > 0)
            {
                if (isFirstParameterInserted)
                {
                    sql.Append(",");

                }
                sql.Append("@UserId=@UserId");
                parameters.Add("UserId", User.FindFirst("userId")?.Value);
                isFirstParameterInserted = true;
            }
            if (post.PostId > 0)
            {
                if (isFirstParameterInserted)
                {
                    sql.Append(",");
                }
                sql.Append("@PostId=@PostId");
                parameters.Add("PostId", post.PostId);
                isFirstParameterInserted = true;

            }
            if (post.PostTitle.Length > 0)
            {
                if (isFirstParameterInserted)
                {
                    sql.Append(",");
                    isFirstParameterInserted = true;
                }
                sql.Append("@PostTitle=@PostTitle");
                parameters.Add("PostTitle", post.PostTitle);
                isFirstParameterInserted = true;

            }
            if (post.PostContent.Length > 0)
            {
                if (isFirstParameterInserted)
                {
                    sql.Append(",");
                }
                sql.Append("@PostContent=@PostContent");
                parameters.Add("PostContent", post.PostContent);
                isFirstParameterInserted = true;

            }
            Console.WriteLine(sql.ToString());
            if (_dapper.ExecuteSql(sql.ToString(), parameters))
            {
                return Ok();
            }
            throw new Exception("Failed to upser post");

        }

        [HttpDelete("Post/{postId}")]

        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.sp_Posts_Delete @PostId=@PostId, @UserId= @UserId
            ";
            var parameters = new
            {
                PostId = postId.ToString(),
                UserId = User.FindFirst("userId")?.Value
            };

            if (_dapper.ExecuteSql(sql, parameters))
            {
                return Ok();
            }
            throw new Exception("Failed to Delete the post");
        }




    }
}