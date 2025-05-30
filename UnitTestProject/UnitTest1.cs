using AuthServiceTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supabase.Gotrue;
using System.Threading.Tasks;

[TestClass]
public class AuthServiceTests
{

    public class FakeAuthClient : IAuthClient
    {
        private readonly bool _shouldFail;

        public FakeAuthClient(bool shouldFail = false)
        {
            _shouldFail = shouldFail;
        }

        public Task<Session> SignUp(string email, string password)
        {
            if (_shouldFail)
                throw new Exception("Test error");

            return Task.FromResult(new Session { AccessToken = "fake-token" });
        }
    }

    [TestMethod]
    public async Task RegisterAsync_ReturnsSession_WhenSuccess()
    {
        // Arrange
        var fakeClient = new FakeAuthClient();
        var service = new AuthService(fakeClient);

        // Act
        var result = await service.RegisterAsync("user@example.com", "password123");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("fake-token", result.AccessToken);
    }

    [TestMethod]
    public async Task RegisterAsync_ReturnsNull_WhenFails()
    {
        // Arrange
        var fakeClient = new FakeAuthClient(shouldFail: true);
        var service = new AuthService(fakeClient);

        // Act
        var result = await service.RegisterAsync("user@example.com", "password123");

        // Assert
        Assert.IsNull(result);
    }
}
