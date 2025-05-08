using Microsoft.AspNetCore.DataProtection;

namespace EventPortal.PropertyEditors
{
    public class PasswordEncryptionService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public PasswordEncryptionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            var protector = _dataProtectionProvider.CreateProtector("UmbracoPasswordEditor");
            return protector.Protect(password);
        }

        public string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword)) return string.Empty;

            try
            {
                var protector = _dataProtectionProvider.CreateProtector("UmbracoPasswordEditor");
                return protector.Unprotect(encryptedPassword);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
