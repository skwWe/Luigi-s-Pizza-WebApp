using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PizzaWebApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaWebApp.Tests.Pages
{
    [TestClass]
    public class OrderConfirmedTests
    {
        private Mock<IOrderService> _orderServiceMock;
        private TestNavigationManager _navigationManager;
        private OrderConfirmed _component;

        public class OrderConfirmed
        {
            public IOrderService OrderService { get; set; }
            public NavigationManager Navigation { get; set; }
            public List<OrderDetail> OrderDetails { get; set; } = new();
            public string OrderId { get; set; }

            public async Task OnInitializedAsync()
            {
                if (Guid.TryParse(OrderId, out var orderGuid))
                {
                    var details = await OrderService.GetOrderDetailsAsync(orderGuid);
                    if (details != null)
                    {
                        OrderDetails = details;
                    }
                }
            }

            public string FormatMoscowTime(DateTime dateTime)
            {
                return dateTime.AddHours(3).ToString("HH:mm");
            }

            public void GoToMenu()
            {
                Navigation.NavigateTo("/menu");
            }

            public void PrintOrder()
            {
                Console.WriteLine("Печать заказа...");
            }
        }

        public interface IOrderService
        {
            Task<List<OrderDetail>> GetOrderDetailsAsync(Guid orderId);
        }

        public class OrderDetail
        {
            public string PizzaName { get; set; }
            public int Quantity { get; set; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _navigationManager = new TestNavigationManager();

            _component = new OrderConfirmed
            {
                OrderService = _orderServiceMock.Object,
                Navigation = _navigationManager
            };
        }

        [TestMethod]
        public async Task OnInitializedAsync_ShouldLoadOrderDetails_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var testDetails = new List<OrderDetail>
            {
                new OrderDetail { PizzaName = "Pepperoni", Quantity = 2 }
            };

            _orderServiceMock.Setup(x => x.GetOrderDetailsAsync(orderId))
                          .ReturnsAsync(testDetails);

            _component.OrderId = orderId.ToString();

            // Act
            await _component.OnInitializedAsync();

            // Assert
            Assert.AreEqual(1, _component.OrderDetails.Count);
            Assert.AreEqual("Pepperoni", _component.OrderDetails[0].PizzaName);
        }

        [TestMethod]
        public void FormatMoscowTime_ShouldAdd3Hours()
        {
            // Arrange
            var utcTime = new DateTime(2023, 1, 1, 10, 0, 0);

            // Act
            var result = _component.FormatMoscowTime(utcTime);

            // Assert
            Assert.AreEqual("13:00", result);
        }

        [TestMethod]
        public void GoToMenu_ShouldNavigateToMenuPage()
        {
            // Act
            _component.GoToMenu();

            // Assert
            Assert.AreEqual("/menu", _navigationManager.LastNavigatedTo);
        }

        public class TestNavigationManager : NavigationManager
        {
            public string LastNavigatedTo { get; private set; }

            public TestNavigationManager()
            {
                Initialize("http://localhost/", "http://localhost/");
            }

            protected override void NavigateToCore(string uri, bool forceLoad)
            {
                LastNavigatedTo = uri;
            }
        }
    }
}