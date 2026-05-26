using Microsoft.AspNetCore.Mvc;
using NetTask.Utilities;

namespace NetTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginUserController : ControllerBase
    {
        private readonly NetTaskDbContext _db;

        public LoginUserController(NetTaskDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [PermissionAuthorize("用户列表")]
        public IActionResult Get()
        {
            return Ok(_db.LoginUser.ToList());
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("用户详情")]
        public IActionResult Get(Guid id)
        {
            var user = _db.LoginUser.FirstOrDefault(it => it.LoginUser_Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        [PermissionAuthorize("新增用户")]
        public IActionResult Post([FromBody] Models.LoginUser value)
        {
            if (string.IsNullOrEmpty(value.LoginUser_Account))
            {
                return BadRequest("用户账号不能为空");
            }
            if (string.IsNullOrEmpty(value.LoginUser_Password))
            {
                return BadRequest("密码不能为空");
            }
            List<string> listRole = new List<string> { "员工", "领导", "管理员" };
            if (!listRole.Contains(value.LoginUser_Role))
            {
                return BadRequest("角色不正确");
            }
            if (!_db.Department.Any(it => it.Department_Id == value.LoginUser_Department))
            {
                return BadRequest("部门不存在");
            }
            if (_db.LoginUser.Any(it => it.LoginUser_Account.ToLower().Trim() == value.LoginUser_Account.ToLower().Trim()))
            {
                return BadRequest("用户账号已存在");
            }
            value.LoginUser_Id = Guid.NewGuid();
            value.LoginUser_Salt = HashPasswordService.CreateSalt();
            value.LoginUser_Password = HashPasswordService.HashPassword(value.LoginUser_Password, value.LoginUser_Salt);
            value.LoginUser_CreateTime = DateTime.Now;
            _db.LoginUser.Add(value);
            _db.SaveChanges();
            return Ok(value);
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("修改用户")]
        public IActionResult Put(Guid id, [FromBody] Models.LoginUser value)
        {
            var user = _db.LoginUser.FirstOrDefault(it => it.LoginUser_Id == id);
            if (user == null)
            {
                return NotFound();
            }
            List<string> listRole = new List<string> { "员工", "领导", "管理员" };
            if (!listRole.Contains(value.LoginUser_Role))
            {
                return BadRequest("角色不正确");
            }
            if (!_db.Department.Any(it => it.Department_Id == value.LoginUser_Department))
            {
                return BadRequest("部门不存在");
            }
            user.LoginUser_Role = value.LoginUser_Role;
            user.LoginUser_Department = value.LoginUser_Department;
            if (!string.IsNullOrEmpty(value.LoginUser_Password))
            {
                user.LoginUser_Salt = HashPasswordService.CreateSalt();
                user.LoginUser_Password = HashPasswordService.HashPassword(value.LoginUser_Password, user.LoginUser_Salt);
            }
            _db.LoginUser.Update(user);
            _db.SaveChanges();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("删除用户")]
        public IActionResult Delete(Guid id)
        {
            var user = _db.LoginUser.FirstOrDefault(it => it.LoginUser_Id == id);
            if (user == null)
            {
                return NotFound();
            }
            _db.TaskItem.RemoveRange(_db.TaskItem.Where(it => it.TaskItem_LoginUserId == id));
            _db.LoginUser.Remove(user);
            _db.SaveChanges();
            return Ok();
        }
    }
}
