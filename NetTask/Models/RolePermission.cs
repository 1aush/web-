using System;
using System.Collections.Generic;

namespace NetTask.Models;

public partial class RolePermission
{
    public string RolePermission_Role { get; set; } = null!;

    public string RolePermission_Permission { get; set; } = null!;
}
