using System;
using System.Collections.Generic;

namespace NetTask.Models;

public partial class TaskItem
{
    public Guid TaskItem_Id { get; set; }

    public string TaskItem_Title { get; set; } = null!;

    public DateTime TaskItem_CreateTime { get; set; }

    public DateTime TaskItem_FinishTime { get; set; }

    public Guid TaskItem_LoginUserId { get; set; }

    public string TaskItem_State { get; set; } = null!;
}
