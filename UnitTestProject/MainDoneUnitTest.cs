using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bunit;
using Moq;
using PizzaWebApp.Pages; // пространство имен страницы main-done
using PizzaWebApp.Models;
using PizzaWebApp.Services;

using Microsoft.Extensions.DependencyInjection;

namespace PizzaWebApp.Tests.Pages
{
   

    [TestClass]
    public class MainDonePageTests : Bunit.TestContext
    {
        private Mock<ISupabaseWrapper> _supabaseMock;

        [TestInitialize]
        public void Setup()
        {
            _supabaseMock = new Mock<ISupabaseWrapper>();

            // Регистрируем сервис в DI контейнере bUnit
            Services.AddSingleton(_supabaseMock.Object);
        }

        [TestMethod]
        public void ShowsLoadingMessage_WhenOrdersAreNull()
        {
            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("Готово"))
        .ReturnsAsync(new List<OrderDetailView>()); // <--- Пустой список, не null!

            // Act
            var component = RenderComponent<MainDone>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Нет выполненных заказов"));
        }

        [TestMethod]
        public void ShowsNoOrdersMessage_WhenOrdersIsEmpty()
        {
            // Arrange
            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("Готово"))
                .ReturnsAsync(new List<OrderDetailView>());

            // Act
            var component = RenderComponent<MainDone>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Нет выполненных заказов"));
            Assert.IsTrue(component.Markup.Contains("images/bowser-sad.png"));
        }

        [TestMethod]
        public async Task ShowsOrderCard_WithAllDetails()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var orderDetails = new List<OrderDetailView>
    {
        new OrderDetailView
        {
            OrderId = orderId,
            ShortId = "A1B2",
            OrderTime = new System.DateTime(2025, 6, 4, 14, 30, 0),
            Status = "Готово",
            PizzaName = "Маргарита",
            Ingredients = new List<string> { "сыр", "томат" },
            Toppings = new List<string> { "базилик" },
            Comment = "без лука",
            Quantity = 2
        }
    };

            _supabaseMock.Setup(x => x.GetOrdersWithStatusAsync("Готово"))
                .ReturnsAsync(orderDetails);

            // Act
            var component = RenderComponent<MainDone>();

            // Чтобы дождаться OnInitializedAsync и перерендера
            await Task.Delay(100);

            // Assert
            var markup = component.Markup;

            Assert.IsTrue(markup.Contains("14:30"));
            Assert.IsTrue(markup.Contains("Заказ A1B2"));
            Assert.IsTrue(markup.Contains("Маргарита x(2)"));
            Assert.IsTrue(markup.Contains("<b>Состав:</b> сыр, томат"));
            Assert.IsTrue(markup.Contains("<b>Допы:</b> базилик"));
            Assert.IsTrue(markup.Contains("<b>Комментарий:</b>"));
            Assert.IsTrue(markup.Contains("<i>без лука</i>"));
        }
    }

}
