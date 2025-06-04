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
    public class MainNewPageTests
    {
        private Bunit.TestContext _testContext;
        private Mock<ISupabaseWrapper> _supabaseMock;

        [TestInitialize]
        public void Setup()
        {
            _testContext = new Bunit.TestContext();
            _supabaseMock = new Mock<ISupabaseWrapper>();

            _testContext.Services.AddSingleton(new Supabase.Client(
                "https://fake.supabase.co",
                "fake-key"
            ));

            _testContext.Services.AddSingleton<ISupabaseWrapper>(_supabaseMock.Object);


            _testContext.ComponentFactories.Add<ProfileSidebar>(() => new FakeProfileSidebar());

        }

        [TestCleanup]
        public void Cleanup() => _testContext.Dispose();

        [TestMethod]
        public void ShowsEmptyMessage_WhenNoOrders()
        {
            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("В обработке"))
                         .ReturnsAsync(new List<OrderDetailView>());

            var component = _testContext.RenderComponent<MainNew>();

            Assert.IsTrue(component.Markup.Contains("Нет заказов"));
        }


        [TestMethod]
        public void ShowsNoOrdersMessage_WhenOrdersEmpty()
        {
            // Arrange
            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("В обработке"))
                         .ReturnsAsync(new List<OrderDetailView>());

            // Act
            var component = _testContext.RenderComponent<MainNew>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Нет новых заказов"));
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
                    PizzaName = "Пепперони",
                    Quantity = 1,
                    Ingredients = new List<string> { "пепперони", "сыр" },
                    Toppings = new List<string>(),
                    Comment = "" // пусто
                }
            };

            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("В обработке"))
                         .ReturnsAsync(data);

            // Act
            var component = _testContext.RenderComponent<MainNew>();

            // Assert
            Assert.IsFalse(component.FindAll(".order-comment").Any());
            
        }

        [TestMethod]
        public async Task ClickingStartButton_UpdatesStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var data = new List<OrderDetailView>
            {
                new OrderDetailView
                {
                    OrderId = orderId,
                    ShortId = "ABCDE",
                    OrderTime = DateTime.UtcNow,
                    Status = "В обработке",
                    PizzaName = "4 сыра",
                    Quantity = 1,
                    Ingredients = new List<string> { "сыр 1", "сыр 2", "сыр 3", "сыр 4" },
                    Toppings = new List<string>()
                }
            };

            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("В обработке"))
                         .ReturnsAsync(data);

            _supabaseMock.Setup(x => x.UpdateOrderStatusAsync(orderId, "Готовится"))
                         .Returns(Task.CompletedTask)
                         .Verifiable();

            // Act
            var component = _testContext.RenderComponent<MainNew>();
            var button = component.Find("button.ready-btn");
            button.Click();

            // Assert
            _supabaseMock.Verify(x => x.UpdateOrderStatusAsync(orderId, "Готовится"), Times.Once);
        }
    }

    // ВНЕ тестового класса (внизу файла):
    public class FakeProfileSidebar : ProfileSidebar
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            
        }
    }
    

}
