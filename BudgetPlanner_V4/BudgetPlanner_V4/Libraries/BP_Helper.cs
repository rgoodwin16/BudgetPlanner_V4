using BudgetPlanner_V4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace BudgetPlanner_V4.Libraries
{
    public static class BP_Helper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());

        public static void NewAccount(HouseholdAccount model)
        {
            var account = new HouseholdAccount()
            {
                Name = model.Name,
                OriginalName = model.Name,
                Balance = model.Balance,
                ReconciledBalance = model.Balance,
                HouseholdId = (int)user.HouseholdId
            };

            db.HouseholdAccounts.Add(account);

            var transaction = new Transaction()
            {
                Description = "New Account: " + model.Name + " created.",
                Amount = model.Balance,
                Created = DateTimeOffset.Now,
                IsReconciled = true,
                HouseholdAccountId = model.HouseholdId,
                CategoryId = user.Household.Categories.FirstOrDefault(c => c.Name == "New Account Created").Id
            };

            db.Transactions.Add(transaction);
            db.SaveChangesAsync();
        }

        public static void AdjustBalance(HouseholdAccount model)
        {
            var oldAccount = db.HouseholdAccounts.FirstOrDefault(a => a.Id == model.Id);
            var adjBal = oldAccount.Balance - model.Balance;

            var transaction = new Transaction()
                {
                    Description = "User Adjusted Balance",
                    Amount = adjBal,
                    IsReconciled = true,
                    CategoryId = user.Household.Categories.FirstOrDefault(c => c.Name == "User Adjusted Balance").Id,
                    Created = DateTimeOffset.Now,
                    HouseholdAccountId = model.Id
                };

            db.Transactions.Add(transaction);

            oldAccount.Balance -= adjBal;
            oldAccount.ReconciledBalance -= adjBal;

            db.SaveChangesAsync();
        }

        public static bool IsNewCategory(string name)
        {
            var categories = user.Household.Categories;

            if (!categories.Any(c => c.Name == name))
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        public static void NewCategory(string name)
        {
            var newCategory = new Category()
            {
                Name = name,
                HouseholdId = (int)user.HouseholdId
            };

            db.Categories.Add(newCategory);
            db.SaveChangesAsync();
        }
    }
}