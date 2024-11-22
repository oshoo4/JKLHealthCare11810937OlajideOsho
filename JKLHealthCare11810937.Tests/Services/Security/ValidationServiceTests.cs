using JKLHealthCare11810937.Services.Security;
using Microsoft.Extensions.DependencyInjection;

namespace JKLHealthCare11810937.Tests.Services.Security
{
    public class ValidationServiceTests : IClassFixture<TestStartup>
    {
        private readonly IValidationService _validationService;

        public ValidationServiceTests(TestStartup startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _validationService = serviceProvider.GetRequiredService<IValidationService>();
        }

        [Theory]
        [InlineData("Test1!", false)]
        [InlineData("Test1234", false)]
        [InlineData("test!123", false)]
        [InlineData("TEST!123", false)]
        [InlineData("TestTest!", false)]
        [InlineData("Test!1234", true)]
        public void IsPasswordComplex_ShouldReturnExpectedResult(string password, bool expectedResult)
        {
            bool result = _validationService.IsPasswordComplex(password);

            Assert.Equal(expectedResult, result);
        }
    }
}