
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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"
                    SELECT * FROM TutorialAppSchema.Posts
            ";
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("Posts/{postId}")]
        public Post GetPost(int postId)
        {
            string sql = @"
                SELECT * FROM TutorailAppSchema.Posts
                    WHERE PostId=@PostId
            ";
            var parameters = new
            {
                PostId = postId
            };
            return _dapper.LoadDataSingle<Post>(sql, parameters);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = @"
                SELECT * FROM TutorialAppSchema.Posts
                    WHERE UserId= @UserId
            ";
            var parameters = new
            {
                UserId = userId
            };
            return _dapper.LoadData<Post>(sql, parameters);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"
                    SELECT * FROM TutorialAppSchema.Posts
                        WHERE UserId= @UserId
            ";
            var parameters = new
            {
                UserId = User.FindFirst("userId")?.Value
            };
            return _dapper.LoadData<Post>(sql, parameters);
        }
        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.Posts (
                    UserId,
                    PostTitle,
                    PostContent,
                    PostCreated,
                    PostUpdated
                ) VALUES (
                    @UserId,
                    @PostTitle,
                    @PostContent,
                    @PostCreated,
                    @PostUpdated
                )
            ";
            var parameters = new
            {
                UserId = User.FindFirst("userId"),
                PostTitle = postToAdd.PostTitle,
                PostContent = postToAdd.PostContent,
                PostCreated = DateTime.Now,
                PostUpdated = DateTime.Now

            };
            if (_dapper.ExecuteSql(sql, parameters))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post");

        }
        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sql = @"
                UPDATE TutorialAppSchema.Posts SET
                    PostTitle= @PostTitle
                    PostContent= @PostContent
                    PostUpdated= @PostUpdated
                WHERE
                    PostId= @PostId AND UserId=@UserId    
            ";
            var parameters = new
            {
                PostTitle = postToEdit.PostTitle,
                PostContent = postToEdit.PostContent,
                PostUpdated = DateTime.Now,
                PostId = postToEdit.PostId,
                UserId = User.FindFirst("userId")?.Value
            };
            if (_dapper.ExecuteSql(sql, parameters))
            {
                return Ok();
            }
            throw new Exception("Failed to Updated the post");
        }

        [HttpDelete("Post/{postId}")]

        public IActionResult DeletePost(int postId)
        {
            string sql = @"
                DELETE FROM TutorialAppSchema.Posts 
                    WHERE PostId=@PostId AND UserId= @UserId
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

        [HttpGet("SearchPosts/{searchKeyword}")]

        public IEnumerable<Post> SearchPosts(string searchKeyword)
        {
            string sql = @"
                    SELECT * 
                    FROM TutorialAppSchema.Posts
                    WHERE PostTitle LIKE '%' + @searchKeyword + '%' 
                    OR PostContent LIKE '%' + @searchKeyword + '%';
                    ";
            var parameters = new
            {
                searchKeyword = searchKeyword
            };

            return _dapper.LoadData<Post>(sql, parameters);

        }




    }
}