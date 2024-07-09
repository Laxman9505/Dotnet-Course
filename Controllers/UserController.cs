using Microsoft.AspNetCore.Mvc;

namespace DOTNETAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController()
    {

    }

    [HttpGet("GetUsers/{testValue}")]

    public string[] GetUsers(string testValue)
    {
        return new string[] { "User1", "User2", testValue };

    }
}

