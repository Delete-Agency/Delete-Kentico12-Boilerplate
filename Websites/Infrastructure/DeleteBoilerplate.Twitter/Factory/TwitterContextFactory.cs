using LinqToTwitter;
using System;
using Settings = DeleteBoilerplate.Domain.Settings;

namespace DeleteBoilerplate.Twitter.Factory
{
    public class TwitterContextFactory
    {
        public static TwitterContext CreateTwitterContext()
        {
            var auth = DoApplicationOnlyAuth();
            try
            {
                auth.AuthorizeAsync().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return new TwitterContext(auth);
            }
            return new TwitterContext(auth);
        }

        static IAuthorizer DoApplicationOnlyAuth()
        {
            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Settings.Integrations.Twitter.ConsumerKey,
                    ConsumerSecret = Settings.Integrations.Twitter.ConsumerSecret
                }
            };

            return auth;
        }
    }
}