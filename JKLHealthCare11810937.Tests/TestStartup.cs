using JKLHealthCare11810937.Services.Security;
using Microsoft.Extensions.DependencyInjection;
using JKLHealthCare11810937.Services.Repository;
using Microsoft.Extensions.Configuration;
using JKLHealthCare11810937.Services.Data;
using JKLHealthCare11810937.Tests.Mocks;
using JKLHealthCare11810937.Tests.Services.Security;

namespace JKLHealthCare11810937.Tests
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            services.AddScoped<IRepository, MockRepository>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IKeyVaultService, MockKeyVaultService>();
        }
    }
}