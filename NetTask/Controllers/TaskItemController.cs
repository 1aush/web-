using Microsoft.AspNetCore.Mvc;
using NetTask.Utilities;

namespace NetTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private readonly NetTaskDbContext _db;
        private readonly Utilities.ITokenService _tokenService;
        private readonly Models.LoginUser _user;

        public TaskItemController(NetTaskDbContext db, Utilities.ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
            _user = _tokenService.ReadToken();
        }

        [HttpGet]
        [PermissionAuthorize("个人任务列表")]
        public IActionResult Get()
        {
            var list = _db.TaskItem.Where(it => it.TaskItem_LoginUserId == _user.LoginUser_Id).ToList();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("个人任务详情")]
        public IActionResult Get(Guid id)
        {
            var taskItem = _db.TaskItem.FirstOrDefault(it => it.TaskItem_Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }
            if (taskItem.TaskItem_LoginUserId != _user.LoginUser_Id)
            {
                return Forbid();
            }
            return Ok(taskItem);
        }

        [HttpPost]
        [PermissionAuthorize("新增个人任务")]
        public IActionResult Post([FromBody] Models.TaskItem value)
        {
            if (string.IsNullOrEmpty(value.TaskItem_Title))
            {
                return BadRequest("标题必填");
            }
            value.TaskItem_Id = Guid.NewGuid();
            value.TaskItem_LoginUserId = _user.LoginUser_Id;
            value.TaskItem_CreateTime = DateTime.Now;
            value.TaskItem_FinishTime = DateTime.Now;
            value.TaskItem_State = "未完成";
            _db.TaskItem.Add(value);
            _db.SaveChanges();
            return Ok(value);
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("修改个人任务")]
        public IActionResult Put(Guid id, [FromBody] Models.TaskItem value)
        {
            if (string.IsNullOrEmpty(value.TaskItem_Title))
            {
                return BadRequest("标题必填");
            }
            var taskItem = _db.TaskItem.FirstOrDefault(it => it.TaskItem_Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }
            if (taskItem.TaskItem_LoginUserId != _user.LoginUser_Id)
            {
                return Forbid();
            }
            taskItem.TaskItem_State = value.TaskItem_State;
            taskItem.TaskItem_Title = value.TaskItem_Title;
            taskItem.TaskItem_FinishTime = DateTime.Now;
            _db.TaskItem.Update(taskItem);
            _db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("删除个人任务")]
        public IActionResult Delete(Guid id)
        {
            var taskItem = _db.TaskItem.FirstOrDefault(it => it.TaskItem_Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }
            if (taskItem.TaskItem_LoginUserId != _user.LoginUser_Id)
            {
                return Forbid();
            }
            _db.TaskItem.Remove(taskItem);
            _db.SaveChanges();
            return Ok();
        }
    }
}
