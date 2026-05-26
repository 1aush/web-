using System;
using System.Collections.Generic;

namespace NetTask.Models;

public partial class Department
{
    public Guid Department_Id { get; set; }

    public string Department_Code { get; set; } = null!;

    public string Department_Name { get; set; } = null!;
}
