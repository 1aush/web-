using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NetTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly NetTaskDbContext _db;
        private readonly Utilities.ITokenService _tokenService;

        public LoginController(NetTaskDbContext db, Utilities.ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get(string account, string password)
        {
            var user = _db.LoginUser.FirstOrDefault(it => it.LoginUser_Account.ToLower().Trim() == account.ToLower().Trim());
            if (user == null)
            {
                return NotFound("用户不存在");
            }
            if (!Utilities.HashPasswordService.Validate(password, user.LoginUser_Salt, user.LoginUser_Password))
            {
                return NotFound("密码错误");
            }
            return Ok(new
            {
                Id = user.LoginUser_Id,
                Role = user.LoginUser_Role,
                Token = _tokenService.CreateToken(user.LoginUser_Id, user.LoginUser_Account, user.LoginUser_Role, user.LoginUser_Department)
            });
        }
    }
}
