using System;
using Xunit;
using BCImplementatie.Domain.Entities;

namespace BCImplementatie.Tests.Domain
{
    public class GebruikTests
    {
        [Fact]
        public void Constructor_InitializesFieldsCorrectly()
        {
            // Arrange
            var clientId = 7;
            var prodId = Guid.NewGuid();

            // Act
            var g = new Gebruik(clientId, prodId);

            // Assert
            Assert.Equal(clientId, g.ClientId);
            Assert.Equal(prodId, g.ZorgtechnologieProductItemId);
            Assert.NotEqual(Guid.Empty, g.Id);
            Assert.True(g.InGebruik);
            Assert.Null(g.EndTime);
            Assert.True((DateTime.UtcNow - g.StartTime).TotalSeconds < 5);
        }

        [Fact]
        public void VoegCareNeedToe_AppendsToCollection()
        {
            var g = new Gebruik(1, Guid.NewGuid());
            var need = new CareNeed
            {
                Id = 42,
                GebruikId = g.Id,
                NeedDescription = "Test",
                NeedCategoryName = "Gezondheid"
            };

            g.VoegCareNeedToe(need);

            Assert.Contains(need, g.CareNeeds);
        }

        [Fact]
        public void VoegErvaringToe_AppendsToCollection()
        {
            var g = new Gebruik(1, Guid.NewGuid());
            var ev = new Ervaring
            {
                Id = 99,
                GebruikId = g.Id,
                Datum = DateTime.UtcNow,
                Review = "👍",
                Observatie = "Ok"
            };

            g.VoegErvaringToe(ev);

            Assert.Contains(ev, g.Ervaringen);
        }

        [Fact]
        public void StopGebruik_SetsInGebruikFalseAndEndTime()
        {
            var g = new Gebruik(2, Guid.NewGuid());

            g.StopGebruik();

            Assert.False(g.InGebruik);
            Assert.NotNull(g.EndTime);
            Assert.True(g.EndTime.Value >= g.StartTime);
        }
    }
}
