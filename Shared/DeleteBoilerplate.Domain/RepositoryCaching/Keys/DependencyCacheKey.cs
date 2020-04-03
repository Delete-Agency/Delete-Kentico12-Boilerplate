using Castle.Core.Internal;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;
using DeleteBoilerplate.Domain.RepositoryCaching.Providers;
using LightInject.Interception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeleteBoilerplate.Domain.RepositoryCaching.Keys
{
    public interface IDependencyCacheKey
    {
        List<string> GetDependencyCacheKeys(IInvocationInfo invocation, object invocationResult);
    }

    public class DependencyCacheKey : IDependencyCacheKey
    {
        private IContentItemMetadataProvider ContentItemMetadataProvider { get; }

        public DependencyCacheKey(IContentItemMetadataProvider contentItemMetadataProvider)
        {
            ContentItemMetadataProvider = contentItemMetadataProvider;
        }

        public List<string> GetDependencyCacheKeys(IInvocationInfo invocation, object invocationResult)
        {
            var dependencyCacheKeys = new List<string>();

            if (invocationResult is TreeNode treeNode)
            {
                dependencyCacheKeys.Add(GetDependencyCacheKeyForPage(treeNode));
            }

            if (invocationResult is IEnumerable<TreeNode>)
            {
                dependencyCacheKeys.Add(GetDependencyCacheKeyForPages(invocationResult.GetType().GenericTypeArguments[0]));
            }

            if (invocationResult is BaseInfo baseInfo && !(invocationResult is TreeNode))
            {
                dependencyCacheKeys.Add(GetDependencyCacheKeyForObject(baseInfo));
            }

            if (invocationResult is IEnumerable<BaseInfo> && !(invocationResult is IEnumerable<TreeNode>))
            {
                dependencyCacheKeys.Add(GetDependencyCacheKeyForObjects(invocationResult.GetType().GenericTypeArguments[0]));
            }

            var cacheDependencyAttributes = invocation.TargetMethod.GetCustomAttributes<CacheDependencyAttribute>().ToList();

            if (cacheDependencyAttributes.Count > 0)
            {
                dependencyCacheKeys.Add(GetDependencyCacheKeyFromAttributes(cacheDependencyAttributes, invocation.Arguments));
            }

            return dependencyCacheKeys
                .Where(x => !x.IsNullOrEmpty())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string GetDependencyCacheKeyForPage(TreeNode node)
        {
            return string.Format(DependencyCacheKeysFormats.SpecificPage, SiteContext.CurrentSiteName.ToLowerInvariant(), node.NodeGUID);
        }

        private string GetDependencyCacheKeyForPages(Type type)
        {
            return string.Format(DependencyCacheKeysFormats.PageType, SiteContext.CurrentSiteName.ToLowerInvariant(), ContentItemMetadataProvider.GetClassNameFromPageRuntimeType(type));
        }

        private string GetDependencyCacheKeyForObject(BaseInfo info)
        {
            var id = info.GetIntegerValue(info.TypeInfo.IDColumn, 0);

            return id == 0 ? null : string.Format(DependencyCacheKeysFormats.SpecificObject, ContentItemMetadataProvider.GetObjectTypeFromInfoObjectRuntimeType(info.GetType()), id);
        }

        private string GetDependencyCacheKeyForObjects(Type type)
        {
            return string.Format(DependencyCacheKeysFormats.ObjectType, ContentItemMetadataProvider.GetObjectTypeFromInfoObjectRuntimeType(type));
        }

        private static string GetDependencyCacheKeyFromAttributes(List<CacheDependencyAttribute> attributes, object[] methodArguments)
        {
            return attributes.Select(attribute => attribute.ResolveKey(SiteContext.CurrentSiteName.ToLowerInvariant(), methodArguments)).Join(TextHelper.NewLine);
        }
    }
}
