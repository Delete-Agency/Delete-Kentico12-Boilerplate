using CMS.Helpers;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;
using DeleteBoilerplate.Domain.RepositoryCaching.Keys;
using LightInject.Interception;
using System;
using System.Linq;
using System.Reflection;

namespace DeleteBoilerplate.Domain.RepositoryCaching
{
    public interface ICachingRepositoryInterceptor : IInterceptor
    { }

    public sealed class CachingRepositoryInterceptor : ICachingRepositoryInterceptor
    {
        private TimeSpan CacheItemDuration { get; } = GetCacheItemDurationFromSettings();

        private IDependencyCacheKey DependencyCacheKey { get; }

        public CachingRepositoryInterceptor(IDependencyCacheKey dependencyCacheKey)
        {
            DependencyCacheKey = dependencyCacheKey;
        }

        public object Invoke(IInvocationInfo invocation)
        {
            if (IsCacheEnabled() && invocation.TargetMethod.GetCustomAttributes<RepositoryCachingAttribute>().Any())
                return this.GetOrCreateCachedResult(invocation);

            return invocation.Proceed();
        }

        private object GetOrCreateCachedResult(IInvocationInfo invocation)
        {
            var cacheKey = CacheKeyHelper.GetCacheItemKey(new CacheKeyParams
            {
                Type = invocation.Proxy.Target.GetType().FullName,
                Method = invocation.Method.Name,
                Arguments = invocation.Arguments
            });

            if (cacheKey == null)
                return invocation.Proceed();

            object result = null;

            using (var cs = new CachedSection<object>(ref result, this.CacheItemDuration.TotalMinutes, true, cacheKey))
            {
                if (cs.LoadData)
                {
                    result = invocation.Proceed();

                    var cacheDependencies = DependencyCacheKey.GetDependencyCacheKeys(invocation, result);

                    cs.Data = result;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return result;
        }

        private static bool IsCacheEnabled()
        {
            return !Settings.PreviewEnabled;
        }

        private static TimeSpan GetCacheItemDurationFromSettings()
        {
            return TimeSpan.FromSeconds(Settings.Cache.RepositoryCacheItemDuration);
        }
    }
}
