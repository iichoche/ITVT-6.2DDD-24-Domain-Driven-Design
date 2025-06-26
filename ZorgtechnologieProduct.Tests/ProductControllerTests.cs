
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZorgtechnologieProduct.API.Controllers;
using ZorgtechnologieProduct.Application;
using ZorgtechnologieProduct.Domain;

namespace ZorgtechnologieProduct.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public void Get_ReturnsListOfProducts()
        {
            var mockService = new Mock<IZorgtechnologieZoekerService>();
            mockService.Setup(s => s.FilterOpCriteria(It.IsAny<ZorgtechnologieFilter>()))
                .Returns(new List<ZorgtechnologieProduct> { new ZorgtechnologieProduct { Id = Guid.NewGuid(), Naam = "TestProduct" } });

            var controller = new ProductController(mockService.Object);
            var result = controller.Get().Result as OkObjectResult;
            var producten = result.Value as List<ZorgtechnologieProduct>;

            Assert.NotNull(producten);
            Assert.Single(producten);
        }
    }
}
