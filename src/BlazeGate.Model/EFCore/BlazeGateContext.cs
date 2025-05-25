using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.EFCore
{
    public class BlazeGateContext : DbContext
    {
        public BlazeGateContext(DbContextOptions<BlazeGateContext> options) : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }

        public DbSet<ServiceConfig> ServiceConfigs { get; set; }

        public DbSet<Destination> Destinations { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RolePage> RolePages { get; set; }

        public DbSet<AuthWhiteList> AuthWhiteLists { get; set; }

        public DbSet<DistributedCache> DistributedCaches { get; set; }

        public DbSet<AuthRsaKey> AuthRsaKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}