using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using RaceAlly.Models;

namespace RaceAlly.Contracts.DataModel
{
    public interface IDataContext
    {
        IDbSet<User> Users { get; set; }
        IDbSet<Role> Roles { get; set; }
        IDbSet<Race> Races { get; set; }
        IDbSet<OAuthAccount> OAuthAccounts { get; set; }

        DbEntityEntry Entry<T>(T entity) where T : class ;

        void SaveChanges();
        void Dispose();
    }
}
