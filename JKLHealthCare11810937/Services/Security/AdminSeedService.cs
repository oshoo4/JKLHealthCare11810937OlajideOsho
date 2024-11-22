using JKLHealthCare11810937.Models.DBModels;
using JKLHealthCare11810937.Services.Repository;

namespace JKLHealthCare11810937.Services.Security
{
    public class AdminSeedService : IAdminSeedService
    {
        private readonly IRepository _repository;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IKeyVaultService _keyVaultService;
        private readonly (string Username, string Password) _adminCredentials;

        public AdminSeedService(
            IRepository repository,
            IUserAuthenticationService userAuthenticationService,
            IKeyVaultService keyVaultService
        )
        {
            _repository = repository;
            _userAuthenticationService = userAuthenticationService;
            _keyVaultService = keyVaultService;
            _adminCredentials = GetAdminCredentials();
        }

        private (string Username, string Password) GetAdminCredentials()
        {
            string username = _keyVaultService.GetSecret("AdminUsername");
            string password = _keyVaultService.GetSecret("AdminPassword");

            if (username != null && password != null)
            {
                return (Username: username, Password: password);
            }
            else
            {
                throw new InvalidOperationException("'AdminCredentials' not found");
            }
        }

        public async Task SeedAdminUserAsync()
        {
            if (await _repository.AdminExists())
            {
                return;
            }

            var adminUser = new User
            {
                Username = _adminCredentials.Username,
                Role = "administrator",
                PasswordHash = _userAuthenticationService.HashPassword(_adminCredentials.Password)
            };

            await _repository.AddUserAsync(adminUser);
        }
    }
}