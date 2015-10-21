using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BudgetPlanner_V4.Models
{
    public class HouseholdAccount
    {
        public HouseholdAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public int Id { get;set; }
        public string Name { get;set; }
        public string OriginalName { get; set; }
        public decimal Balance { get;set; }
        public decimal ReconciledBalance { get; set; }
        public bool IsArchived { get; set; }

        public int HouseholdId { get; set; }
        
        public virtual ICollection<Transaction> Transactions { get;set; }
        [JsonIgnore]
        public virtual Household Household { get; set; }
    }
}