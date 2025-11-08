using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MLM_Web_App.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        // 🧩 Computed display helpers
        public string DisplayName => $"{FullName} ({UserCode})";
        public string CreatedDateFormatted => CreatedAt.ToString("dd-MMM-yyyy");

        // 💰 Income rates per level
        private const int Level1Rate = 100;
        private const int Level2Rate = 50;
        private const int Level3Rate = 25;

        // 🧮 Convenience: Count direct referrals (Level 1)
        public int DirectReferralCount => InverseSponsor?.Count ?? 0;

        // 🧩 Get all level 1 members
        public IEnumerable<User> Level1Members => InverseSponsor ?? new List<User>();

        // 🧩 Get level 2 members (members of your direct referrals)
        public IEnumerable<User> Level2Members =>
            Level1Members.SelectMany(u => u.InverseSponsor ?? new List<User>());

        // 🧩 Get level 3 members (members of level 2)
        public IEnumerable<User> Level3Members =>
            Level2Members.SelectMany(u => u.InverseSponsor ?? new List<User>());

        // 💵 Total income calculation logic
        public decimal TotalIncome =>
            (Level1Members.Count() * Level1Rate) +
            (Level2Members.Count() * Level2Rate) +
            (Level3Members.Count() * Level3Rate);
    }

    // 🏷️ Metadata with annotations for validation and UI display
    public class UserMetadata
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "User Code")]
        [ScaffoldColumn(false)] // ✅ hide from scaffolding and ignore on validation
        public string UserCode { get; set; } = null!;

        [Required, StringLength(100), Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number starting with 6-9")]
        [Display(Name = "Mobile Number")]
        public string? Mobile { get; set; }


        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string PasswordHash { get; set; } = null!;

        [Display(Name = "Sponsor Code")]
        public string? SponsorUserCode { get; set; }

        [Display(Name = "Sponsor ID")]
        public int? SponsorId { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Created Date"), DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
