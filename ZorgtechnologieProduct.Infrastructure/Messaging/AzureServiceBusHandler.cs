using Microsoft.Extensions.Logging;
using ZorgtechnologieProduct.Domain;

namespace ZorgtechnologieProduct.Infrastructure.Messaging
{
    // Interface voor het versturen en ontvangen van producten via de Service Bus
    public interface IMessageHandler
    {
        void Send(ZorgProduct product);    // Verstuur een product
        void Receive(ZorgProduct product); // Ontvang een product
    }

    // Implementatie van IMessageHandler voor Azure Service Bus
    public class AzureServiceBusHandler : IMessageHandler
    {
        private readonly ILogger<AzureServiceBusHandler> _logger;

        // Constructor met logger
        public AzureServiceBusHandler(ILogger<AzureServiceBusHandler> logger)
        {
            _logger = logger;
        }

        // Simuleert het versturen van een product naar de Service Bus
        public void Send(ZorgProduct product)
        {
            _logger.LogInformation(
                $"Product verstuurd naar Service Bus: Id={product.Id}, Naam={product.Naam}, Type={product.Type}, Kosten={product.Kosten}, Omschrijving={product.Omschrijving}");
        }

        // Simuleert het ontvangen van een product van de Service Bus
        public void Receive(ZorgProduct product)
        {
            _logger.LogInformation(
                $"Product ontvangen van Service Bus: Id={product.Id}, Naam={product.Naam}, Type={product.Type}, Kosten={product.Kosten}, Omschrijving={product.Omschrijving}");
        }
    }
}
