namespace Discount.Grpc.Extensions
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Npgsql;
    using System.Threading;

    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating Postgres database...");

                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

                    connection.Open();

                    using var command = new NpgsqlCommand { Connection = connection };

                    command.CommandText = @"
                        DROP TABLE IF EXISTS Coupon;

                        CREATE TABLE Coupon(
		                        ID SERIAL PRIMARY KEY         NOT NULL,
		                        ProductName     VARCHAR(24) NOT NULL,
		                        Description     TEXT,
		                        Amount          INT
	                        );

                        INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150);

                        INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);";

                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated Postgres database...");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogInformation("An error occurred while migration Postgres database.");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        host.MigrateDatabase<TContext>(retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}
