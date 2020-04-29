using CMS.Helpers;
using DeleteBoilerplate.Common.Extensions;

namespace DeleteBoilerplate.Infrastructure.Services
{
    public interface IHashService
    {
        string GetHash(string str);

        bool ValidateHash(string str, string hash);
    }

    public class HashService : IHashService
    {
        protected readonly string Salt;

        public HashService(string salt)
        {
            Salt = salt;
        }

        public string GetHash(string str)
        {
            return str.IsNullOrEmpty()
                ? string.Empty
                : SecurityHelper.GetSHA2Hash($"{str}{Salt}");
        }

        public bool ValidateHash(string str, string hash)
        {
            if (hash.IsNullOrEmpty() || str.IsNullOrEmpty())
                return false;

            return hash == this.GetHash(str);
        }
    }
}