using Microsoft.AspNetCore.Authorization;

namespace NetTask.Utilities
{
    /// <summary>
    /// 定义自定义特性
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "Permission";
        public PermissionAuthorizeAttribute(string permissionName) => PermissionName = permissionName;
        public string PermissionName
        {
            get
            {
                return PermissionName;
            }
            set
            {
                Policy = $"{POLICY_PREFIX}{value.ToString()}";
            }
        }
    }
}
