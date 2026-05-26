using Microsoft.AspNetCore.Authorization;

namespace NetTask.Utilities
{
    /// <summary>
    /// 定义策略处理类
    /// </summary>
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly NetTaskDbContext _db;
        public PermissionRequirementHandler(NetTaskDbContext db)
        {
            _db = db;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var role = context.User.FindFirst(c => c.Type == "UserRole");
            if (role != null)
            {
                if (_db.RolePermission.Any(
                    it => it.RolePermission_Permission == requirement.PermissionName &&
                          it.RolePermission_Role == role.Value
                ))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
