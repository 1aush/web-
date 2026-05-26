using Microsoft.AspNetCore.Authorization;

namespace NetTask.Utilities
{
    /// <summary>
    /// 定义权限策略
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }

        public PermissionRequirement(string permissionName)
        {
            this.PermissionName = permissionName;
        }
    }
}
