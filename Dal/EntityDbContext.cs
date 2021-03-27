using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Models.Models;
using static Dal.Utilities.ConnectionStringUtility;

namespace Dal
{
    public sealed class EntityDbContext: IdentityDbContext<User, IdentityRole<int>, int>, IDesignTimeDbContextFactory<EntityDbContext>
    {
        public EntityDbContext() { }

        /// <inheritdoc />
        /// <summary>
        /// Constructor that will be called by startup.cs
        /// </summary>
        /// <param name="optionsBuilderOptions"></param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public EntityDbContext(DbContextOptions<EntityDbContext> optionsBuilderOptions) : base(optionsBuilderOptions)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        ///     This is used for DB migration locally
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public EntityDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var options = new DbContextOptionsBuilder<EntityDbContext>()
                .UseNpgsql(ConnectionStringUrlToPgResource(configuration.GetValue<string>("DATABASE_URL")))
                .Options;

            return new EntityDbContext(options);
        }
    }
}