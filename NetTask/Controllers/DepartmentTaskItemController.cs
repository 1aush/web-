using Microsoft.AspNetCore.Mvc;
using NetTask.Utilities;

namespace NetTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentTaskItemController : ControllerBase
    {
        private readonly NetTaskDbContext _db;
        private readonly Utilities.ITokenService _tokenService;
        private readonly Models.LoginUser _user;

        public DepartmentTaskItemController(NetTaskDbContext db, Utilities.ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
            _user = _tokenService.ReadToken();
        }

        [HttpGet]
        [PermissionAuthorize("部门任务列表")]
        public IActionResult Get()
        {
            var list = _db.TaskItem
                .Join(
                    _db.LoginUser,
                    item => item.TaskItem_LoginUserId,
                    user => user.LoginUser_Id,
                    (item, user) => new
                    {
                        item.TaskItem_Id,
                        item.TaskItem_LoginUserId,
                        item.TaskItem_State,
                        item.TaskItem_CreateTime,
                        item.TaskItem_Title,
                        user.LoginUser_Account,
                        user.LoginUser_Role,
                        user.LoginUser_Department
                    }
                )
                .Where(it => it.LoginUser_Department == _user.LoginUser_Department)
                .ToList();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("部门任务详情")]
        public ActionResult<Models.TaskItem> Get(Guid id)
        {
            var taskItem = _db.TaskItem.FirstOrDefault(it => it.TaskItem_Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }
            var taskUser = _db.LoginUser.FirstOrDefault(it => it.LoginUser_Id == taskItem.TaskItem_LoginUserId);
            if (taskUser == null)
            {
                return BadRequest("任务所属用户不存在");
            }
            if (taskUser.LoginUser_Department != _user.LoginUser_Department)
            {
                return Forbid();
            }
            return taskItem;
        }
    }
}
