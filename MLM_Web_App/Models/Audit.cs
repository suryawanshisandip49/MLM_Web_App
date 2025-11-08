using System;
using System.Collections.Generic;

namespace MLM_Web_App.Models;

public partial class Audit
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public DateTime? CreatedAt { get; set; }
}
