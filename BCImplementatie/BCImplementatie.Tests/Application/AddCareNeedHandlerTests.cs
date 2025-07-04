using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using BCImplementatie.Application.Commands.CareNeeds;
using BCImplementatie.Application.Interfaces;
using BCImplementatie.Domain.Entities;

namespace BCImplementatie.Tests.Application
{
    public class AddCareNeedHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCalled_AddsCareNeedToGebruikAndUpdatesRepo()
        {
            // Arrange
            var existing = new Gebruik(clientId: 1, zorgtechnologieProductItemId: Guid.NewGuid());
            var gebruikId = existing.Id;  // vang de constructor‐generated Id

            var mockRepo = new Mock<IGebruikRepository>();
            mockRepo
                .Setup(r => r.GetByIdAsync(gebruikId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            mockRepo
                .Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var handler = new AddCareNeedHandler(mockRepo.Object);

            var cmd = new AddCareNeedCommand(
                GebruikId: gebruikId,
                NeedDescription: "Test beschrijving",
                NeedCategoryName: "Gezondheid",
                AdviesProductId: null
            );

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert: eigenschappen komen overeen
            Assert.Equal(cmd.GebruikId, result.GebruikId);
            Assert.Equal(cmd.NeedDescription, result.NeedDescription);
            Assert.Equal(cmd.NeedCategoryName, result.NeedCategoryName);
            Assert.Equal(cmd.AdviesProductId, result.AdviesZorgtechnologieProductId);

            // Assert: is toegevoegd aan de collectie
            Assert.Contains(result, existing.CareNeeds);

            // Assert: UpdateAsync is aangeroepen
            mockRepo.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
