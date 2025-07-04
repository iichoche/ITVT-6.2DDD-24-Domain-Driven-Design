using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using BCImplementatie.Application.Commands.StartGebruik;
using BCImplementatie.Application.Interfaces;
using BCImplementatie.Domain.Entities;
using BCImplementatie.Domain.Events;

namespace BCImplementatie.Tests.Application
{
    public class StartGebruikHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesEntityAndPublishesEvent()
        {
            // Arrange
            var mockRepo = new Mock<IGebruikRepository>();
            var mockPub = new Mock<IEventPublisher>();
            Gebruik saved = null;

            mockRepo
                .Setup(r => r.AddAsync(It.IsAny<Gebruik>(), It.IsAny<CancellationToken>()))
                .Callback<Gebruik, CancellationToken>((g, ct) => saved = g)
                .Returns(Task.CompletedTask);

            var handler = new StartGebruikHandler(mockRepo.Object, mockPub.Object);
            var cmd = new StartGebruikCommand(5, Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            // Act
            var resultId = await handler.Handle(cmd, CancellationToken.None);

            // Assert - repository
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Gebruik>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(saved.Id, resultId);

            // Assert - event published
            mockPub.Verify(p =>
                p.PublishAsync(
                    It.Is<GebruikGestartEvent>(e => e.GebruikId == saved.Id),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }
    }
}
