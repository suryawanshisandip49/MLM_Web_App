using System;
using System.Collections.Generic;

namespace MLM_Web_App.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Mobile { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? SponsorUserCode { get; set; }

    public int? SponsorId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<User> InverseSponsor { get; set; } = new List<User>();

    public virtual User? Sponsor { get; set; }
}
