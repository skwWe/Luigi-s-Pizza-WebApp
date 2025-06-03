using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PizzaWebApp.Models;
using PizzaWebApp.Services;
using Supabase.Postgrest.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace PizzaWebApp.Tests.Services
{
    [TestClass]
    public class CartServiceTests
    {
        private readonly Mock<ISupabaseCartWrapper> _wrapperMock;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _wrapperMock = new Mock<ISupabaseCartWrapper>();
            _cartService = new CartService(_wrapperMock.Object);
        }

        [TestMethod]
        public void AddItem_ShouldAddNewItem_WhenItemNotInCart()
        {
            // Arrange
            var item = new MenuItem { Id = "1", Name = "Margherita" };

            // Act
            _cartService.AddItem(item);

            // Assert
            Assert.AreEqual(1, _cartService.Items.Count);
            Assert.AreEqual("Margherita", _cartService.Items[0].Item.Name);
            Assert.AreEqual(1, _cartService.Items[0].Quantity);
        }

        [TestMethod]
        public void AddItem_ShouldIncreaseQuantity_WhenItemAlreadyInCart()
        {
            // Arrange
            var item = new MenuItem { Id = "1" };
            _cartService.AddItem(item);

            // Act
            _cartService.AddItem(item);

            // Assert
            Assert.AreEqual(1, _cartService.Items.Count);
            Assert.AreEqual(2, _cartService.Items[0].Quantity);
        }

        [TestMethod]
        public void RemoveItem_ShouldRemoveItem_WhenQuantityIsOne()
        {
            // Arrange
            var item = new MenuItem { Id = "1" };
            _cartService.AddItem(item);

            // Act
            _cartService.RemoveItem(item);

            // Assert
            Assert.AreEqual(0, _cartService.Items.Count);
        }

        [TestMethod]
        public void RemoveItem_ShouldDecreaseQuantity_WhenQuantityMoreThanOne()
        {
            // Arrange
            var item = new MenuItem { Id = "1" };
            _cartService.AddItem(item);
            _cartService.AddItem(item);

            // Act
            _cartService.RemoveItem(item);

            // Assert
            Assert.AreEqual(1, _cartService.Items.Count);
            Assert.AreEqual(1, _cartService.Items[0].Quantity);
        }

        [TestMethod]
        public void AddOrUpdateItem_ShouldAddItem_WithSpecificQuantity()
        {
            // Arrange
            var item = new MenuItem { Id = "1" };

            // Act
            _cartService.AddOrUpdateItem(item, 3);

            // Assert
            Assert.AreEqual(1, _cartService.Items.Count);
            Assert.AreEqual(3, _cartService.Items[0].Quantity);
        }

        [TestMethod]
        public void AddOrUpdateItem_ShouldRemoveItem_WhenQuantityBecomesZero()
        {
            // Arrange
            var item = new MenuItem { Id = "1" };
            _cartService.AddItem(item);

            // Act
            _cartService.AddOrUpdateItem(item, -1);

            // Assert
            Assert.AreEqual(0, _cartService.Items.Count);
        }

        [TestMethod]
        public void Clear_ShouldRemoveAllItems()
        {
            // Arrange
            _cartService.AddItem(new MenuItem { Id = "1" });
            _cartService.AddItem(new MenuItem { Id = "2" });

            // Act
            _cartService.Clear();

            // Assert
            Assert.AreEqual(0, _cartService.Items.Count);
        }

        [TestMethod]
        public async Task AddItemWithImage_ShouldUsePlaceholder_WhenImageUrlIsEmpty()
        {
            // Arrange
            var item = new MenuItem { Id = "1", ImageUrl = "" };
            _wrapperMock.Setup(x => x.GetImageUrl(It.IsAny<string>()))
                      .ReturnsAsync("/images/pizza-placeholder.png");

            // Act
            await _cartService.AddItemWithImage(item);

            // Assert
            Assert.AreEqual("/images/pizza-placeholder.png", _cartService.Items[0].Item.ImageUrl);
        }

       

        [TestMethod]
        public async Task Checkout_ShouldReturnFailure_WhenOrderCreationFails()
        {
            // Arrange
            _wrapperMock.Setup(x => x.CreateOrder(It.IsAny<Order>()))
                      .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _cartService.Checkout("Test", "123", "Address");

            // Assert
            Assert.IsFalse(result.success);
            Assert.AreEqual(Guid.Empty, result.orderId);
        }

       

        [TestMethod]
        public void NotifyStateChanged_ShouldInvokeOnChange()
        {
            // Arrange
            bool eventFired = false;
            _cartService.OnChange += () => eventFired = true;

            // Act
            _cartService.AddItem(new MenuItem { Id = "1" });

            // Assert
            Assert.IsTrue(eventFired);
        }

        
    }
}