using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the database context to the service collection and configures it to use PostgreSQL.
        /// </summary>
        /// <param name="services">The service collection to add the database context to.</param>
        /// <param name="configuration">The application configuration containing the connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure the DbContext to use PostgreSQL with the connection string from the configuration
            services.AddDbContextFactory<VRDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnectionStrings")));

            return services;
        }
    }
}
