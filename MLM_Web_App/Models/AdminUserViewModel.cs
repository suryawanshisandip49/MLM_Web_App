namespace MLM_Web_App.Models
{
    public class AdminUserViewModel
    {
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public int DirectReferrals { get; set; }
        public int TotalTeam { get; set; }
        public decimal TotalIncome { get; set; }
        public bool IsActive { get; set; }
    }
}
