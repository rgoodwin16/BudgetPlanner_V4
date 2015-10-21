using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner_V4.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool IsExpense { get; set; }
        public int Frequency { get; set; }

        public int HouseholdId { get; set; }
        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Household Household { get; set; }
        public virtual Category Category { get; set; }
    }
}