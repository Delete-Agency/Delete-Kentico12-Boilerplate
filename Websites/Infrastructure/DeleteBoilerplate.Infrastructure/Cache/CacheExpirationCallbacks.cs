using System;
using System.Collections.Concurrent;

namespace DeleteBoilerplate.Infrastructure
{
    public static class CacheExpirationCallbacks
    {
        private static readonly ConcurrentDictionary<string, Action> Callbacks =
            new ConcurrentDictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

        public static void Register(string key, Action callback)
        {
            Callbacks.AddOrUpdate(key, callback, (k, oldCallback) => callback);
        }

        public static void Call(string key)
        {
            if (Callbacks.TryGetValue(key, out var callback))
            {
                callback();
            }
        }
    }
}
