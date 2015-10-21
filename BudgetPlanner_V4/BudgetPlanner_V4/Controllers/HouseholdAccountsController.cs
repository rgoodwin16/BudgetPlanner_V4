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
    [RoutePrefix("api/HouseholdAccounts")]
    public class HouseholdAccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: api/HouseHoldAccounts - LIST ALL ACCOUNTS FOR THIS USER'S HOUSEHOLD
        [HttpPost, Route("Index")]
        public IHttpActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var accounts = user.Household.HouseholdAccounts.Where(a => a.IsArchived == false).ToList();

            if (accounts == null)
            {
                return BadRequest("No Accounts Found");
            }

            else
            {
                List<object> returnAccount = new List<object>();

                foreach (var item in accounts)
                {
                    var acc = new HouseholdAccountVM()
                    {
                        Name = item.Name,
                        Balance = item.Balance,
                        ReconciledBalance = item.ReconciledBalance,
                        Transactions = item.Transactions.ToList()
                    };

                    returnAccount.Add(acc);
                }

                return Ok(returnAccount);
            }

        }

        // POST: api/HouseHoldAccounts - CREATE ACCOUNT
        [ResponseType(typeof(HouseholdAccount))]
        [HttpPost, Route("Create")]
        public async Task<IHttpActionResult> Create(HouseholdAccount model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            else
            {
                BP_Helper.NewAccount(model);
                await db.SaveChangesAsync();
                return Ok(model.Name + " created successfully.");
            }

        }

        // POST: api/HouseHoldAccounts/5 - GET ACCOUNT
        [ResponseType(typeof(HouseholdAccount))]
        [HttpPost, Route("Details")]
        public IHttpActionResult Details(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.Household.HouseholdAccounts.Where(a => a.Id == id && !a.IsArchived).FirstOrDefault();

            if (account == null)
            {
                return BadRequest("No account found");
            }

            return Ok(account);
        }

        // POST: api/HouseHoldAccounts/5 - EDIT ACCOUNT

        [HttpPost, Route("Edit")]
        public async Task<IHttpActionResult> Edit(HouseholdAccount model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldAccount = db.HouseholdAccounts.FirstOrDefault(a => a.Id == model.Id);

            if (oldAccount.Name != model.Name)
            {
                oldAccount.Name = model.Name;
                oldAccount.OriginalName = model.Name;
            }

            //check balance
            if (oldAccount.Balance != model.Balance)
            {
                BP_Helper.AdjustBalance(model);
            }
            await db.SaveChangesAsync();
            return Ok();

        }

        // POST: api/HouseHoldAccounts/5 - ARCHIVE ACCOUNT
        [ResponseType(typeof(HouseholdAccount))]
        [HttpPost, Route("Archive")]
        public async Task<IHttpActionResult> Archive(int id)
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.Household.HouseholdAccounts.FirstOrDefault(a => a.Id == id);

            if (account == null)
            {
                return BadRequest("No account found");
            }

            account.Name = account.OriginalName + "-Archived";
            account.IsArchived = true;

            foreach (var transaction in account.Transactions)
            {
                transaction.IsArchived = true;
            }

            await db.SaveChangesAsync();

            return Ok("The account: " + account.OriginalName + " has been archived.");
        }

        // POST: api/HouseHoldAccounts/5 - RECLAIM ACCOUNT
        [ResponseType(typeof(HouseholdAccount))]
        [HttpPost, Route("Reclaim")]
        public async Task<IHttpActionResult> Reclaim(int id)
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.Household.HouseholdAccounts.FirstOrDefault(a => a.Id == id);

            if (account == null)
            {
                return BadRequest("No account found");
            }

            account.Name = account.OriginalName + "-Reclaimed";
            account.IsArchived = false;

            foreach (var transaction in account.Transactions)
            {
                transaction.IsArchived = false;
            }

            await db.SaveChangesAsync();

            return Ok("The account: " + account.OriginalName + " has been reclaimed.");
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
