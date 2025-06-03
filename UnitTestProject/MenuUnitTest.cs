using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PizzaWebApp.Models;
using PizzaWebApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaWebApp.Tests.Services
{
    [TestClass]
    public class MenuServiceTests
    {
        private readonly Mock<ISupabaseMenuWrapper> _wrapperMock;
        private readonly MenuService _menuService;

        public MenuServiceTests()
        {
            _wrapperMock = new Mock<ISupabaseMenuWrapper>();
            _menuService = new MenuService(_wrapperMock.Object);
        }

        [TestMethod]
        public async Task GetBurgersAsync_ReturnsMenuItems_WhenSuccess()
        {
            // Arrange
            var expectedItems = new List<MenuItem>
        {
            new MenuItem { Id = "1", Name = "Pizza 1", Price = 10.99m },
            new MenuItem { Id = "2", Name = "Pizza 2", Price = 12.99m }
        };

            _wrapperMock.Setup(x => x.GetMenuItems())
                      .ReturnsAsync(expectedItems);

            // Act
            var result = await _menuService.GetBurgersAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod] // <- Обязательно должен быть этот атрибут
        public async Task GetBurgersAsync_ReturnsEmptyList_WhenException()
        {
            // Arrange
            _wrapperMock.Setup(x => x.GetMenuItems())
                      .ThrowsAsync(new Exception("Test"));

            // Act
            var result = await _menuService.GetBurgersAsync();

            // Assert
            Assert.AreEqual(0, result.Count);
        }


       
        [TestMethod]
        public async Task GetImageUrl_ReturnsOriginalUrl_ForHttpPath()
        {
            // Arrange
            var httpUrl = "https://example.com/image.jpg";
            _wrapperMock.Setup(x => x.GetImageUrl(httpUrl))
                       .ReturnsAsync(httpUrl);

            // Act
            var result = await _menuService.GetImageUrl(httpUrl);

            // Assert
            Assert.AreEqual(httpUrl, result);
        }

        [TestMethod]
        public async Task GetImageUrl_GeneratesSignedUrl_ForValidPath()
        {
            // Arrange
            var imagePath = "images/burger1.jpg";
            var expectedUrl = "https://storage.example.com/signed-url";

            _wrapperMock.Setup(x => x.GetImageUrl(imagePath))
                      .ReturnsAsync(expectedUrl);

            // Act
            var result = await _menuService.GetImageUrl(imagePath);

            // Assert
            Assert.AreEqual(expectedUrl, result);
        }
    }
}