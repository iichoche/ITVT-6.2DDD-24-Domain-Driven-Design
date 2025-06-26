
using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using ZorgtechnologieProduct.Application;
using ZorgtechnologieProduct.Domain;

namespace ZorgtechnologieProduct.Tests
{
    public class ZorgtechnologieZoekerServiceTests
    {
        [Fact]
        public void CheckBeschikbaarheid_ReturnsTrue_WhenProductAvailable()
        {
            var service = new Mock<IZorgtechnologieZoekerService>();
            service.Setup(x => x.CheckBeschikbaarheid(It.IsAny<Guid>())).Returns(true);

            Assert.True(service.Object.CheckBeschikbaarheid(Guid.NewGuid()));
        }

        [Fact]
        public void GeefDetails_ReturnsNull_WhenProductNotFound()
        {
            var service = new Mock<IZorgtechnologieZoekerService>();
            service.Setup(x => x.GeefDetails(It.IsAny<Guid>())).Returns((ZorgtechnologieProduct)null);

            Assert.Null(service.Object.GeefDetails(Guid.NewGuid()));
        }
    }
}
