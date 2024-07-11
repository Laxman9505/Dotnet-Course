using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DOTNETAPI.Data;
using DOTNETAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOTNETAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {

        IMapper _mapper;
        IUserRepository _userRepository;

        public UserEFController(IConfiguration config, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<UserToAddDto, User>()));
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {

            IEnumerable<User> users = _userRepository.GetUsers();
            return users;
        }
        [HttpGet("GetUser/{userId}")]
        public User GetUser(int userId)
        {
            return _userRepository.GetUser(userId);

        }

        [HttpPut("editUser")]
        public IActionResult EditUser([FromBody] User user)
        {
            User? toBeEditedUser = _userRepository.GetUser(user.UserId);
            if (toBeEditedUser != null)
            {
                toBeEditedUser.Active = user.Active;
                toBeEditedUser.FirstName = user.FirstName;
                toBeEditedUser.LastName = user.LastName;
                toBeEditedUser.Gender = user.Gender;
                toBeEditedUser.Email = user.Email;
            }
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to get user");
        }

        [HttpPost("/addUserEf")]
        public IActionResult AddUser([FromBody] UserToAddDto user)
        {
            User toBeAddedUser = _mapper.Map<User>(user);
            _userRepository.AddEntity<User>(toBeAddedUser);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to add user");
        }

        [HttpDelete("DeleteUser/{userId}")]

        public ActionResult DeleteUser(int userId)
        {
            User? user = _userRepository.GetUser(userId);
            if (user != null)
            {
                _userRepository.RemoveEntity<User>(user);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
            }
            throw new Exception("Failed to delete user");
        }
    }
}
