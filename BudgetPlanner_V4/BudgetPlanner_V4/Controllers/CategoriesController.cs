using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using BudgetPlanner_V4.Models;

namespace BudgetPlanner_V4.Controllers
{
    [Authorize]
    [RoutePrefix("api/Household/Categories")]
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //POST: api/HouseHold/Categories - GET ALL CATEGORIES FOR THIS HOUSEHOLD
        [HttpPost, Route("Index")]
        public IHttpActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            try
            {
                var categories = user.Household.Categories.Where(c => c.Name != "New Account Created" && c.Name != "User Adjusted Balance");
                return Ok(categories);
            }

            catch (NullReferenceException)
            {
                return BadRequest("No Categories found.");
            }

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
