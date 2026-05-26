using Microsoft.AspNetCore.Mvc;
using NetTask.Utilities;

namespace NetTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly NetTaskDbContext _db;

        public DepartmentController(NetTaskDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [PermissionAuthorize("部门列表")]
        public IActionResult Get()
        {
            return Ok(_db.Department.ToList());
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("部门详情")]
        public IActionResult Get(Guid id)
        {
            var department = _db.Department.FirstOrDefault(it => it.Department_Id == id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }

        [HttpPost]
        [PermissionAuthorize("新增部门")]
        public IActionResult Post([FromBody] Models.Department value)
        {
            if (string.IsNullOrEmpty(value.Department_Code))
            {
                return BadRequest("部门代码不能为空");
            }
            if (string.IsNullOrEmpty(value.Department_Name))
            {
                return BadRequest("部门名称不能为空");
            }
            if (_db.Department.Any(it => it.Department_Code.ToLower().Trim() == value.Department_Code.ToLower().Trim()))
            {
                return BadRequest("部门代码已存在");
            }
            value.Department_Id = Guid.NewGuid();
            _db.Department.Add(value);
            _db.SaveChanges();
            return Ok(value);
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("修改部门")]
        public IActionResult Put(Guid id, [FromBody] Models.Department value)
        {
            var department = _db.Department.FirstOrDefault(it => it.Department_Id == id);
            if (department == null)
            {
                return NotFound();
            }
            department.Department_Name = value.Department_Name;
            _db.Department.Update(department);
            _db.SaveChanges();
            return Ok(department);
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("删除部门")]
        public IActionResult Delete(Guid id)
        {
            var department = _db.Department.FirstOrDefault(it => it.Department_Id == id);
            if (department == null)
            {
                return NotFound();
            }
            if (_db.LoginUser.Any(it => it.LoginUser_Department == id))
            {
                return BadRequest("当前部门下还有人员，无法删除");
            }
            _db.Department.Remove(department);
            _db.SaveChanges();
            return Ok();
        }
    }
}
