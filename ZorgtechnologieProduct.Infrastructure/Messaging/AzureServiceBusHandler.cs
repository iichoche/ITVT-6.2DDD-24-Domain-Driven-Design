using Microsoft.Extensions.Logging;

namespace ZorgtechnologieProduct.Infrastructure.Messaging
{
    public interface IMessageHandler
    {
        void Send(string message);
    }

    public class AzureServiceBusHandler : IMessageHandler
    {
        private readonly ILogger<AzureServiceBusHandler> _logger;
        public AzureServiceBusHandler(ILogger<AzureServiceBusHandler> logger)
        {
            _logger = logger;
        }

        public void Send(string message)
        {
            _logger.LogInformation($"Bericht verstuurd naar Service Bus: {message}");
        }
    }
}
