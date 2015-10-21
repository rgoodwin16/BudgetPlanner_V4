using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner_V4.Models
{
    public class HouseholdVM
    {
        public string Name { get; set; }
        public List<HouseholdAccount> Accounts { get; set; }
        public List<BudgetItem> BudgetItems { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }

    public class HouseholdAccountVM
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal ReconciledBalance { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

    public class JoinHouseVM
    {
        public string inviteEmail { get; set; }
        public string inviteCode { get; set; }
    }

    public class TransactionVM
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public Nullable<System.DateTimeOffset> Updated { get; set; }
        public bool IsDebit { get; set; }
        public bool IsArchived { get; set; }
        public bool IsReconciled { get; set; }
    }
}