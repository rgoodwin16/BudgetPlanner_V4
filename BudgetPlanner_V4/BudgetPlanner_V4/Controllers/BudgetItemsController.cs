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
    [RoutePrefix("api/BudgetItems")]
    public class BudgetItemsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: api/BudgetItems - GET ALL BUDGET ITEMS FOR THIS HOUSEHOLD
        [HttpPost, Route("Index")]
        public IHttpActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            try
            {
                var bItems = user.Household.BudgetItems.ToList();
                return Ok(bItems);
            }

            catch (NullReferenceException)
            {
                return BadRequest("No Budget Items found.");
            }

        }

        // POST: api/BudgetItems - CREATE BUDGET ITEM
        [ResponseType(typeof(BudgetItem))]
        [HttpPost, Route("Create")]
        public async Task<IHttpActionResult> Create(BudgetItem model)
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

            var budgetItemExits = user.Household.BudgetItems.Any(bi => bi.Name == model.Name);

            if (budgetItemExits)
            {
                return BadRequest("You already have a budget item called: " + model.Name + " . Please chose another name.");
            }

            else
            {
                model.HouseholdId = (int)user.HouseholdId;
                model.CategoryId = model.Category.Id;
                model.Category = null;

                db.BudgetItems.Add(model);
                await db.SaveChangesAsync();

                return Ok();
            }

        }

        // GET: api/BudgetItems/5 - GET BUDGET ITEM
        [ResponseType(typeof(BudgetItem))]
        [HttpPost, Route("Details")]
        public IHttpActionResult Details(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var budgetItem = user.Household.BudgetItems.FirstOrDefault(bi => bi.Id == id);

            if (budgetItem == null)
            {
                return BadRequest("No Budget Item Found");
            }

            return Ok(budgetItem);
        }

        // PUT: api/BudgetItems/5 - EDIT BUDGET ITEM
        [ResponseType(typeof(void))]
        [HttpPost, Route("Edit")]
        public async Task<IHttpActionResult> Edit(BudgetItem model)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (BP_Helper.IsNewCategory(model.Category.Name))
            {
                BP_Helper.NewCategory(model.Category.Name);
                model.CategoryId = user.Household.Categories.Where(c => c.Name == model.Category.Name).FirstOrDefault().Id;
            }

            else
            {
                model.CategoryId = model.Category.Id;
                model.Category = null;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Update<BudgetItem>(model, "Amount", "Name", "Frequency", "isExpense", "CategoryId");
            await db.SaveChangesAsync();

            return Ok();

        }

        // DELETE: api/BudgetItems/5 - DELETE BUDGET ITEM
        [ResponseType(typeof(BudgetItem))]
        [HttpPost, Route("Delete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(id);
            if (budgetItem == null)
            {
                return NotFound();
            }

            db.BudgetItems.Remove(budgetItem);
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
