using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Data.Entities;
using FarmCraft.Users.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmCraft.Users.Data.Context
{
    public class UserContext : DbContext, IFarmCraftContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<FarmCraftLog> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("UserContext");

            modelBuilder.Entity<User>()
                .ToContainer("Users")
                .HasNoDiscriminator()
                .HasPartitionKey(u => u.AzureId);

            modelBuilder.Entity<Organization>()
                .ToContainer("Organizations")
                .HasNoDiscriminator()
                .HasPartitionKey(o => o.OrganizationId);

            modelBuilder.Entity<Invitation>()
                .ToContainer("Invitations")
                .HasNoDiscriminator()
                .HasPartitionKey(i => i.InvitationId);

            modelBuilder.Entity<FarmCraftLog>()
                .ToContainer("Logs")
                .HasNoDiscriminator()
                .HasPartitionKey(l => l.LogId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
