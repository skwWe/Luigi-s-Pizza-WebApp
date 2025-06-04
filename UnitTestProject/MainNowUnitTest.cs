using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using PizzaWebApp.Models;
using PizzaWebApp.Pages;
using PizzaWebApp.Services;
using Microsoft.AspNetCore.Components;
using PizzaWebApp.Layout;
using Microsoft.AspNetCore.Components.Rendering;
using System.Linq;

namespace PizzaWebApp.Tests.Pages
{
    

    [TestClass]
    public class MainNowPageTests
    {
        private Bunit.TestContext _testContext;
        private Mock<ISupabaseWrapper> _supabaseMock;

        [TestInitialize]
        public void Setup()
        {
            _testContext = new Bunit.TestContext();
            _supabaseMock = new Mock<ISupabaseWrapper>();

            _testContext.Services.AddSingleton(_supabaseMock.Object);
        }

        [TestMethod]
        public void ShowsNoOrdersMessage_WhenOrdersIsEmpty()
        {
            // Arrange
            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("Готовится"))
                         .ReturnsAsync(new List<OrderDetailView>());

            // Act
            var component = _testContext.RenderComponent<MainNow>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Нет текущих заказов"));
        }


        [TestMethod]
        public void ShowsNoOrdersMessage_WhenOrdersEmpty()
        {
            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("Готовится"))
                         .ReturnsAsync(new List<OrderDetailView>());

            var component = _testContext.RenderComponent<MainNow>();

            Assert.IsTrue(component.Markup.Contains("Нет текущих заказов"));
        }


        [TestMethod]
        public void ShowsOrderCard_WithAllDetails()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderTime = DateTime.UtcNow;
            var data = new List<OrderDetailView>
            {
                new OrderDetailView
                {
                    OrderId = orderId,
                    ShortId = "ABCDE",
                    OrderTime = orderTime,
                    Status = "В обработке",
                    PizzaName = "Маргарита",
                    Quantity = 2,
                    Ingredients = new List<string> { "сыр", "томат" },
                    Toppings = new List<string> { "базилик" },
                    Comment = "без лука"
                }
            };

            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("В обработке"))
                         .ReturnsAsync(data);

            // Act
            var component = _testContext.RenderComponent<MainNew>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Маргарита"));
            Assert.IsTrue(component.Markup.Contains("<b>Состав:</b> сыр, томат"));
            Assert.IsTrue(component.Markup.Contains("<b>Допы:</b> базилик"));
            Assert.IsTrue(component.Markup.Contains("<b>Комментарий:</b> <i>без лука</i>"));

        }

        [TestMethod]
        public void HidesComment_IfEmpty()
        {
            var orderId = Guid.NewGuid();
            var orderTime = DateTime.UtcNow;
            var data = new List<OrderDetailView>
        {
            new OrderDetailView
            {
                OrderId = orderId,
                ShortId = "ABCDE",
                OrderTime = orderTime,
                Status = "Готовится",
                PizzaName = "Пепперони",
                Quantity = 1,
                Ingredients = new List<string> { "пепперони", "сыр" },
                Toppings = new List<string>(),
                Comment = "" // пусто
            }
        };

            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("Готовится"))
                         .ReturnsAsync(data);

            var component = _testContext.RenderComponent<MainNow>();

            Assert.IsFalse(component.Markup.Contains("Комментарий:"));
           
        }
    }
    
}


