using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Models;
using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest;
using System.Collections.Generic;
using System.Linq;
using PizzaWebApp.Services;

namespace WebsiteTests
{
    [TestClass]
    public class RegisterComponentTests
    {
        private Mock<AuthService> _authServiceMock;
        private Mock<Client> _supabaseClientMock;
        private Mock<NavigationManager> _navManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _authServiceMock = new Mock<AuthService>();
            _supabaseClientMock = new Mock<Client>();
            _navManagerMock = new Mock<NavigationManager>();
        }

        private RegisterModel GetValidModel() => new RegisterModel
        {
            Email = "test@example.com",
            Password = "securePass",
            ConfirmPassword = "securePass",
            FirstName = "Test",
            LastName = "User",
            Age = 30
        };
    }
    [TestMethod]
        public async Task HandleRegistration_SuccessfulRegistration_NavigatesToLogin()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            _authServiceMock
                .Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Session { User = new User { Id = userId } });

            var tableMock = new Mock<Table<UserProfile>>();
            tableMock
                .Setup(t => t.Insert(It.IsAny<UserProfile>(), null, false, null, default))
                .ReturnsAsync(new Postgrest.Responses.ModeledResponse<UserProfile>
                {
                    Models = new List<UserProfile> { new() }
                });

            _supabaseClientMock.Setup(s => s.From<UserProfile>()).Returns(tableMock.Object);

            var component = new FakeRegisterComponent(
                _authServiceMock.Object,
                _navManagerMock.Object,
                _supabaseClientMock.Object)
            {
                registerModel = GetValidModel()
            };

            // Act
            await component.HandleRegistration();

            // Assert
            Assert.IsNull(component.errorMessage);
            _navManagerMock.Verify(n => n.NavigateTo("/login"), Times.Once);
        }
    }