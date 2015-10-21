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
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class Chart
        {
            public string startDate { get; set; }
            public string endDate { get; set; }
        }

        //POST: api/Dashboard - GET ALL BUDGET ITEMS/TRANSACTIONS FOR CHART
        [HttpPost, Route("Test")]
        public IHttpActionResult Get()
        {
            var r = new Random();

            return Ok(new dynamic[]
            {
                new
                {
                    key = "Actual",
                    color = "#51A351",
                    values = Enumerable.Range(0, 10).Select(i =>
                    new
                    {
                        x = (char)('A' + i),
                        y = r.Next(100, 5000),

                    })
                
                },
                new
                {
                    key = "Budgeted",
                    color = "#BD362F",
                    values = Enumerable.Range(0, 10).Select(i =>
                    new
                    {
                        x = (char)('A' + i),
                        y = r.Next(100, 5000),

                    })
                }
            });
        }

        [HttpPost, Route("Selected")]
        public IHttpActionResult All(Chart model)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = user.Household;

            var startDate = model.startDate;
            var endDate = model.endDate;

            var sD = Convert.ToDateTime(startDate);
            var eD = Convert.ToDateTime(endDate);

            return Ok(new dynamic[]
            {
                new
                {
                    key = "Actual",
                    color = "#FF931E",
                    values = from b in household.BudgetItems.Where(bi=> bi.IsExpense == true)
                             select new 
                             {
                                x = b.Category.Name,
                                y = b.Category.Transactions.Where(t=> t.Created >= sD && t.Created <= eD).Select(t=> t.Amount).DefaultIfEmpty().Sum() * -1,
                             }
                },

                new 
                {
                    key = "Budgeted",
                    color = "#00acac",
                    values = from b in household.BudgetItems.Where(bi=> bi.IsExpense == true)
                             select new 
                             {
                                 x = b.Category.Name,
                                 y = b.Amount //* (b.isExpense ? -1 : 1)
                             }
                }
            
            });
        }

        [HttpPost, Route("Current")]
        public IHttpActionResult All()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = user.Household;

            var date = DateTimeOffset.Now;

            return Ok(new dynamic[]
            {
                new
                {
                    key = "Actual",
                    color = "#FF931E",
                    values = from b in household.BudgetItems.Where(bi=> bi.IsExpense == true)
                             select new 
                             {
                                x = b.Category.Name,
                                y = b.Category.Transactions.Where(t=> t.Created.Month == date.Month && t.IsArchived == false).Select(t=> t.Amount).DefaultIfEmpty().Sum() * -1,
                             }
                },

                new 
                {
                    key = "Budgeted",
                    color = "#00acac",
                    values = from b in household.BudgetItems.Where(bi=> bi.IsExpense == true)
                             select new 
                             {
                                 x = b.Category.Name,
                                 y = b.Amount //* (b.isExpense ? -1 : 1)
                             }
                }
            
            });
        }

        [HttpPost, Route("Yearly")]
        public IHttpActionResult Yearly()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = user.Household;

            var now = DateTimeOffset.Now;

            return Ok(new dynamic[]
            {
                new
                {
                    key = "Total Transactions",
                    color = "#FF931E",
                    values = from month in Enumerable.Range(1,12)
                             .Select(m=> new DateTime(now.Year,m,1))

                             select new 
                             {
                                x = month.ToString("MMMM"),
                                y = Math.Abs(household.HouseholdAccounts.SelectMany(t => t.Transactions).Where(t=> t.Created.Month == month.Month && t.Created.Year == now.Year && t.IsDebit == true && t.IsArchived == false).Select(t=> t.Amount).DefaultIfEmpty().Sum() ),
                             }
                },

                new 
                {
                    key = "Budgeted Expenses",
                    color = "#00acac",
                    values = from month in Enumerable.Range(1,12)
                             .Select(m=> new DateTime(now.Year,m,1))

                             select new 
                             {
                                x = month.ToString("MMMM"),
                                y = Math.Abs(household.BudgetItems.Where(b=> b.IsExpense == true).Select(b => b.Amount).DefaultIfEmpty().Sum() ),
                             }
                }
            
            });
        }

        [HttpPost, Route("Dates")]
        public IHttpActionResult GetDates()
        {

            var date = DateTimeOffset.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var year = DateTimeOffset.Now.Year;

            var firstDayOfYear = new DateTime(year, 1, 1);
            var lastDayOfYear = new DateTime(year, 12, 31);

            var dates = new
            {
                BeginMonth = firstDayOfMonth,
                EndMonth = lastDayOfMonth,
                BeginYear = firstDayOfYear,
                EndYear = lastDayOfYear
            };

            return Ok(dates);
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
