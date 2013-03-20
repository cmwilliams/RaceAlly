using System;
using System.Collections.ObjectModel;
using System.Configuration.Provider;
using System.Data.Entity;
using System.Linq;
using RaceAlly.Contracts.DataModel;
using RaceAlly.Contracts.Roles;
using RaceAlly.Models;

namespace RaceAlly.BusinessLogic.Roles
{
    public class RoleProvider : IRoleProvider
    {
        private readonly IDataContext _context;

        public RoleProvider(IDataContext context)
        {
            _context = context;
        }

        public bool IsUserInRole(string username, string roleName)
        {
            return GetUsersInRole(roleName).Any(user => user.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public string[] GetRolesForUser(string username)
        {
            return _context.Roles.Where(role => role.Users.Any(u => u.Username.Equals(username)))
                           .Select(role => role.Name).ToArray();
        }

        public void CreateRole(string roleName)
        {
            var role = new Role { Name = roleName };
            _context.Roles.Add(role);
            _context.SaveChanges();
        }

        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (GetUsersInRole(roleName).Any() && throwOnPopulatedRole)
            {
                throw new ProviderException(String.Format("Try to delete role {0}, but it is populated", roleName));
            }

            var role = _context.Roles.Include(r => r.Users).SingleOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                role.Users.Clear();
                _context.Roles.Remove(role);
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool RoleExists(string roleName)
        {
            return _context.Roles.Any(r => r.Name == roleName);
        }

        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var users = _context.Users.Where(u => usernames.Contains(u.Username)).ToList();

            foreach (var role in roleNames.Select(roleName => _context.Roles.SingleOrDefault(r => r.Name == roleName)).Where(role => role != null))
            {
                if (role.Users == null)
                {
                    role.Users = new Collection<User>();
                }
                foreach (var user in users)
                {
                    role.Users.Add(user);
                }
            }
            _context.SaveChanges();
        }

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var users = _context.Users.Where(u => usernames.Contains(u.Username)).ToList();

            foreach (var roleName in roleNames)
            {
                var role = _context.Roles.Include(r => r.Users).SingleOrDefault(r => r.Name == roleName);
                if (role != null)
                {
                    foreach (var user in users)
                    {
                        role.Users.Remove(user);
                    }
                }
            }
            _context.SaveChanges();
        }

        public string[] GetUsersInRole(string roleName)
        {
            return _context.Roles.Where(role => role.Name.Equals(roleName))
                           .SelectMany(role => role.Users).Select(user => user.Username)
                           .ToArray();
        }

        public string[] GetAllRoles()
        {
            return _context.Roles.Select(role => role.Name).ToArray();
        }
    }
}
