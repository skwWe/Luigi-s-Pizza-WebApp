using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Supabase.Gotrue.Interfaces;
using System;
using System.Threading.Tasks;
using SupabaseSession = Supabase.Gotrue.Session;
using SupabaseUser = Supabase.Gotrue.User;

namespace AuthServiceTool.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private readonly Mock<ISupabaseAuthWrapper> _authWrapperMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authWrapperMock = new Mock<ISupabaseAuthWrapper>();
            _authService = new AuthService(_authWrapperMock.Object);
        }

        [TestMethod]
        public async Task RegisterAsync_Success_ReturnsSession()
        {
            // Arrange
            var expectedSession = new SupabaseSession();
            _authWrapperMock.Setup(x => x.SignUp(
                "test@example.com",
                "password123"
            )).ReturnsAsync(expectedSession);

            // Act
            var result = await _authService.RegisterAsync("test@example.com", "password123");

            // Assert
            Assert.AreEqual(expectedSession, result);
        }

        [TestMethod]
        public async Task RegisterAsync_RetryOnFailure_ReturnsSession()
        {
            // Arrange
            var expectedSession = new SupabaseSession();
            _authWrapperMock.SetupSequence(x => x.SignUp(
                "test@example.com",
                "password123"
            ))
            .ThrowsAsync(new Exception("First attempt failed"))
            .ReturnsAsync(expectedSession);

            // Act
            var result = await _authService.RegisterAsync("test@example.com", "password123");

            // Assert
            Assert.AreEqual(expectedSession, result);
            _authWrapperMock.Verify(x => x.SignUp(
                "test@example.com",
                "password123"
            ), Times.Exactly(2));
        }
        [TestMethod]
        public async Task RegisterAsync_NullEmail_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await _authService.RegisterAsync(null, "password123");
            });
        }
        [TestMethod]
        public async Task RegisterAsync_NullPassword_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await _authService.RegisterAsync("user@example.com", null);
            });
        }
        [TestMethod]
        public async Task RegisterAsync_CallsSignUp_WithCorrectParameters()
        {
            // Arrange
            string expectedEmail = "check@domain.com";
            string expectedPassword = "super-secret";

            _authWrapperMock.Setup(x => x.SignUp(expectedEmail, expectedPassword))
                .ReturnsAsync(new SupabaseSession());

            // Act
            await _authService.RegisterAsync(expectedEmail, expectedPassword);

            // Assert
            _authWrapperMock.Verify(x => x.SignUp(expectedEmail, expectedPassword), Times.Once);
        }
    }
}