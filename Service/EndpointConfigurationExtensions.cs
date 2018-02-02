using System;
using System.Data.SqlClient;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;

namespace Service
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration Configure(this EndpointConfiguration endpointConfiguration, string connectionString)
        {
            LogManager.Use<DefaultFactory>();

            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

            endpointConfiguration.UseTransport<MsmqTransport>();

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.ConnectionBuilder(() => new SqlConnection(connectionString));
            persistence.SqlDialect<SqlDialect.MsSqlServer>();

            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();

            return endpointConfiguration;
        }
    }
}
