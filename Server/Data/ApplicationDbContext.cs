using BlazorApp1.Server.Models;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlazorApp1.Server.Data
{
    public class ApplicationDbContext : ApplicationApiAuthorizationDbContext<ApplicationUser, ApplicationRole>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().HasMany(p => p.Roles).WithOne().HasForeignKey(p => p.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(e => e.Claims).WithOne().HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            if (Database.ProviderName == "FileBaseContext")
            {
                //https://github.com/dotnet/aspnetcore/issues/21945
                builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(p => p.Id).HasValueGenerator<DummyIdValueGenerator>());
                builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(p => p.Id).HasValueGenerator<DummyIdValueGenerator>());
            }
        }
    }

    //https://stackoverflow.com/a/71321924
    public class ApplicationApiAuthorizationDbContext<TUser, TRole> : IdentityDbContext<TUser, TRole, string>, IPersistedGrantDbContext, IDisposable where TUser : ApplicationUser where TRole : ApplicationRole
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

        public DbSet<PersistedGrant> PersistedGrants
        {
            get;
            set;
        }

        public DbSet<DeviceFlowCodes> DeviceFlowCodes
        {
            get;
            set;
        }

        public DbSet<Key> Keys
        {
            get;
            set;
        }

        public ApplicationApiAuthorizationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
        }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);
        }
    }

    internal class DummyIdValueGenerator : ValueGenerator<int>
    {
        public override bool GeneratesTemporaryValues => false;
        public override int Next(EntityEntry entry) => new Random().Next(1, Int32.MaxValue);
    }
}