using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using RaceAlly.Contracts.DataModel;
using RaceAlly.Models;

namespace RaceAlly.BusinessLogic.DataModel
{
    public class RaceAllyDb : DbContext, IDataContext
    {  
        public RaceAllyDb(): base("RaceAlly")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OAuthAccount>().HasKey(a => new { a.Provider, a.ProviderUserId });
            base.OnModelCreating(modelBuilder);
        }

        public IDbSet<User> Users { get; set; }
        public IDbSet<Role> Roles { get; set; }
        public IDbSet<Race> Races { get; set; }
        public IDbSet<OAuthAccount> OAuthAccounts { get; set; }

        public new DbEntityEntry Entry<T>(T entity) where T : class 
        {
            return base.Entry(entity);
        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
