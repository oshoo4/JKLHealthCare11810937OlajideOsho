using JKLHealthCare11810937.Services.Security;
using Microsoft.Extensions.DependencyInjection;

namespace JKLHealthCare11810937.Tests.Services.Security
{
    public class UserAuthenticationServiceTests : IClassFixture<TestStartup>
    {
        private readonly IUserAuthenticationService _userAuthenticationService;

        public UserAuthenticationServiceTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _userAuthenticationService = serviceProvider.GetRequiredService<IUserAuthenticationService>();
        }

        [Fact]
        public void HashPassword_ShouldGenerateDifferentHashesForSamePassword()
        {
            string password = "Password123!";

            string hash1 = _userAuthenticationService.HashPassword(password);
            string hash2 = _userAuthenticationService.HashPassword(password);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
        {
            string password = "Password123!";
            string hash = _userAuthenticationService.HashPassword(password);

            bool result = _userAuthenticationService.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
        {
            string password = "Password123!";
            string hash = _userAuthenticationService.HashPassword(password);
            string incorrectPassword = "WrongPass";

            bool result = _userAuthenticationService.VerifyPassword(incorrectPassword, hash);

            Assert.False(result);
        }
    }
}