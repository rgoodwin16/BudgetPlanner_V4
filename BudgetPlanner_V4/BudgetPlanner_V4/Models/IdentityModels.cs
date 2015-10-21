using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace BudgetPlanner_V4.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public string DisplayName { get; set; }
        public int? HouseholdId { get; set; }

        public virtual Household Household { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            var existingToken = await RefreshTokens.SingleOrDefaultAsync(r => r.Subject == token.Subject);

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            RefreshTokens.Add(token);

            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                RefreshTokens.Remove(refreshToken);
                return await SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            RefreshTokens.Remove(refreshToken);
            return await SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return RefreshTokens.ToList();
        }

        public System.Data.Entity.DbSet<BudgetPlanner_V4.Models.BudgetItem> BudgetItems { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_V4.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_V4.Models.Household> Households { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_V4.Models.HouseholdAccount> HouseholdAccounts { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_V4.Models.Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_V4.Models.Invitation> Invitations { get; set; }
    }
}