using System;
using System.Collections.Generic;

namespace MLM_Web_App.Models;

public partial class VwTeamLevel
{
    public int? RootUserId { get; set; }

    public int? MemberId { get; set; }

    public int? Level { get; set; }
}
