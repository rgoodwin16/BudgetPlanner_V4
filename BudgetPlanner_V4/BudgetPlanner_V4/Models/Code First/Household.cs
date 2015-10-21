using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner_V4.Models
{
    public class Household
    {
        public Household()
        {
            this.HouseholdAccounts = new HashSet<HouseholdAccount>();
            this.BudgetItems = new HashSet<BudgetItem>();
            this.Users = new HashSet<ApplicationUser>();
            this.Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<HouseholdAccount> HouseholdAccounts { get; set; }
        [JsonIgnore]
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> Users { get; set; }
        [JsonIgnore]
        public virtual ICollection<Category> Categories { get; set; }
    }
}