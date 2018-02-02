using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace SagaService
{
    public class Host
    {
        private static readonly ILog Log = LogManager.GetLogger<Host>();
        private readonly string _connectionString;
        private IEndpointInstance _endpoint;

        public static string EndpointName => "SagaServer";

        public Host(string connectionString) => _connectionString = connectionString;

        public async Task<IEndpointInstance> Start()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .Configure(_connectionString);

                _endpoint = await Endpoint.Start(endpointConfiguration);
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }

            return _endpoint;
        }

        public async Task Stop()
        {
            try
            {
                await _endpoint?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        private static void FailFast(string message, Exception exception)
        {
            try
            {
                Log.Fatal(message, exception);
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
