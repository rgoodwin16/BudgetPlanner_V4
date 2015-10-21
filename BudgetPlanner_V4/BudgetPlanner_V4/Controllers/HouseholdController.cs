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
    [RoutePrefix("api/Household")]
    public class HouseholdController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class InviteCode
        {
            public string MakeCode()
            {
                var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var random = new Random();
                return new string(Enumerable.Range(0, 8).Select(n => chars[random.Next(chars.Length)]).ToArray());
            }
        }

        //GET: Household Details
        [HttpPost, Route("Details")]
        [ResponseType(typeof(Household))]
        public IHttpActionResult GetHousehold()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var houseHold = user.Household;

            if (houseHold == null)
            {
                return BadRequest("User is not currently a memeber of a household.");
            }

            else
            {
                var returnHouse = new HouseholdVM()
                {
                    Name = user.Household.Name,
                    Accounts = houseHold.HouseholdAccounts.Where(a => a.IsArchived == false).ToList(),
                    BudgetItems = houseHold.BudgetItems.ToList(),
                    Users = houseHold.Users.ToList()

                };

                return Ok(returnHouse);
            }


        }

        //POST: Create New Household
        [ResponseType(typeof(Household))]
        [HttpPost, Route("Create")]
        public async Task<IHttpActionResult> PostHousehold(string name)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.HouseholdId != null)
            {
                return BadRequest("You must leave your current household before you can create a new household");
            }

            else
            {
                var household = new Household()
                {
                    Name = name
                };

                user.HouseholdId = household.Id;

                string[] defaultCategories = { "New Account Created", "User Adjusted Balance", "Job", "Mortgage/Rent", "Food", "Utilites", "Misc" };

                foreach (var catName in defaultCategories)
                {
                    household.Categories.Add(new Category
                    {
                        HouseholdId = household.Id,
                        Name = catName
                    });
                }

                db.Households.Add(household);
                await db.SaveChangesAsync();

                return Ok(household.Name + " successfully created.");
            }
        }

        //POST: Create New Invitation to Household
        [ResponseType(typeof(Invitation))]
        [HttpPost, Route("Invite")]
        public async Task<IHttpActionResult> PostInvite(string inviteEmail)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var inviteExists = db.Invitations.Any(i => i.InvitedEmail == inviteEmail);

            var invite = new Invitation();

            if (inviteExists)
            {
                invite = db.Invitations.Where(i => i.InvitedEmail == inviteEmail).FirstOrDefault();
            }

            else
            {
                var code = new InviteCode();
                invite = new Invitation()
                {
                    Code = code.MakeCode(),
                    HouseholdId = user.HouseholdId,
                    InvitedEmail = inviteEmail
                };

                db.Invitations.Add(invite);

            }

            var url = "http://rgoodwin-budget.azurewebsites.net";

            var mailer = new EmailService();
            var message = new IdentityMessage()
            {
                Subject = "New Invitation from rgoodwin-budget.azurewebsites.net",
                Destination = inviteEmail,
                Body = "You have been invited to join " + user.UserName + "'s HouseHold. To join please follow this <a href=\'" + url + "\'>link </a> and use this code: " + invite.Code
            };

            await db.SaveChangesAsync();
            await mailer.SendAsync(message);

            return Ok("Invitation sent.");

        }

        //POST: Join existing Household
        [ResponseType(typeof(Household))]
        [HttpPost, Route("Join")]
        public async Task<IHttpActionResult> PostJoinHousehold(JoinHouseVM model)
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            var invite = db.Invitations.Where(i => i.Code == model.inviteCode && i.InvitedEmail == model.inviteEmail).FirstOrDefault();

            if (invite == null)
            {
                return BadRequest("No invitation found");
            }

            else
            {
                user.HouseholdId = invite.HouseholdId;
                db.Invitations.Remove(invite);
            }

            await db.SaveChangesAsync();
            return Ok(user.DisplayName + " successfully joined the " + user.Household.Name + " household.");
        }

        //POST: Leave Household
        [ResponseType(typeof(Household))]
        [HttpPost, Route("Leave")]
        public async Task<IHttpActionResult> PostLeaveHousehold()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            user.HouseholdId = null;
            await db.SaveChangesAsync();

            return Ok(user.DisplayName + " has been successfully removed.");
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
