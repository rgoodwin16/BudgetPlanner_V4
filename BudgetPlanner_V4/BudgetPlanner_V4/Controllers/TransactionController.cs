using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using System.Web.Http.Description;
using BudgetPlanner_V4.Models;
using BudgetPlanner_V4.Libraries;

namespace BudgetPlanner_V4.Controllers
{
    [Authorize]
    [RoutePrefix("api/HouseholdAccounts/Transactions")]
    public class TransactionController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: Get all Transactions for this Household Account
        [HttpPost, Route("Index")]
        public IHttpActionResult Index(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            try
            {
                var transactions = db.HouseholdAccounts.Where(a => a.HouseholdId == user.HouseholdId && a.Id == id).FirstOrDefault().Transactions;
                return Ok(transactions);
            }

            catch (NullReferenceException)
            {
                return Ok("No transactions found.");
            }

        }

        //GET: Get Recent Transactions for all Household Accounts associated with this User's Household
        [HttpPost, Route("Recent")]
        public IHttpActionResult GetRecentTransactions()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var date = DateTimeOffset.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var transactions = db.Transactions.Where(t => t.HouseholdAccount.HouseholdId == user.HouseholdId).Take(10).OrderByDescending(c => c.Created <= lastDayOfMonth).ToArray();
            var models = new List<object>();

            foreach (var item in transactions)
            {
                models.Add(new
                {
                    AccountName = item.HouseholdAccount.Name,
                    TransactionCategory = item.Category.Name,
                    TransactionDesc = item.Description,
                    TransactionAmount = item.Amount,
                    TransactionCreated = item.Created
                });
            }

            return Ok(models);

        }

        //POST: Create Transaction
        [ResponseType(typeof(Transaction))]
        [HttpPost, Route("Create")]
        public async Task<IHttpActionResult> Create(Transaction model)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (BP_Helper.IsNewCategory(model.Category.Name))
            {
                BP_Helper.NewCategory(model.Category.Name);
                model.CategoryId = user.Household.Categories.Where(c => c.Name == model.Category.Name).FirstOrDefault().Id;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.IsDebit)
            {
                if (model.Amount > 0)
                {
                    model.Amount *= -1;//This makes sure that we can't have a positive debit
                }

            }
            else
                if (model.Amount < 0)
                {
                    model.Amount *= -1;//This makes sure we can't have a negative credit
                }

            var account = db.HouseholdAccounts.Find(model.HouseholdAccountId);

            account.Balance += model.Amount;

            if (model.IsReconciled)
            {
                account.ReconciledBalance += model.Amount;
            }

            model.Created = DateTimeOffset.Now;

            model.CategoryId = model.Category.Id;
            model.Category = null;

            db.Transactions.Add(model);
            await db.SaveChangesAsync();

            return Ok();
        }

        //GET: Get Details of this Transaction
        [ResponseType(typeof(Transaction))]
        [HttpPost, Route("Details")]
        public async Task<IHttpActionResult> Details(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var transaction = await db.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return BadRequest("No transaction found.");
            }

            return Ok(transaction);
        }

        //POST: Edit Transaction
        [HttpPost, Route("Edit")]
        public async Task<IHttpActionResult> Edit(Transaction model)
        {
            if (BP_Helper.IsNewCategory(model.Category.Name))
            {
                BP_Helper.NewCategory(model.Category.Name);

                var user = db.Users.Find(User.Identity.GetUserId());
                model.CategoryId = user.Household.Categories.Where(c => c.Name == model.Category.Name).FirstOrDefault().Id;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.IsDebit)
            {
                if (model.Amount > 0)
                {
                    model.Amount *= -1;//This makes sure we don't have a positive debit
                }
            }
            else
                if (model.Amount < 0)
                {
                    model.Amount *= -1;//This makes sure we don't have a negative credit
                }

            var account = db.HouseholdAccounts.FirstOrDefault(a => a.Id == model.HouseholdAccountId);
            var originalTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == model.Id);

            //check if the amount/IsDebit has changed
            if (originalTransaction.Amount != model.Amount)
            {
                account.Balance -= originalTransaction.Amount;
                account.Balance += model.Amount;
            }

            //check if transaction has been reconciled
            if (originalTransaction.IsReconciled != model.IsReconciled)
            {
                account.ReconciledBalance -= originalTransaction.Amount;
                account.ReconciledBalance += model.Amount;
            }

            model.CategoryId = model.Category.Id;
            model.Category = null;

            originalTransaction.Updated = DateTimeOffset.Now;

            db.Update<Transaction>(model, "Description", "Amount", "IsDebit", "IsReconciled", "CategoryId");
            await db.SaveChangesAsync();

            return Ok();
        }

        //POST: Delete Transaction
        [ResponseType(typeof(Transaction))]
        [HttpPost, Route("Delete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var transaction = await db.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return BadRequest("No transaction found.");
            }

            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.Household.HouseholdAccounts.FirstOrDefault(a => a.Id == transaction.HouseholdAccountId);

            account.Balance = account.Balance - transaction.Amount;
            account.ReconciledBalance = account.ReconciledBalance - transaction.Amount;

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




    }
}
