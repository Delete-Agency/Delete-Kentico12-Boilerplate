using System;
using System.Collections.Concurrent;
using CMS.DataEngine;
using CMS.DocumentEngine;

namespace DeleteBoilerplate.Domain.RepositoryCaching.Providers
{
    public interface IContentItemMetadataProvider
    {
        string GetClassNameFromPageRuntimeType(Type type);

        string GetClassNameFromPageRuntimeType<T>() where T : TreeNode, new();

        string GetObjectTypeFromInfoObjectRuntimeType(Type type);

        string GetObjectTypeFromInfoObjectRuntimeType<T>() where T : AbstractInfo<T>, new();
    }

    public sealed class ContentItemMetadataProvider : IContentItemMetadataProvider
    {
        private readonly ConcurrentDictionary<Type, string> _classNames = new ConcurrentDictionary<Type, string>();
        private readonly ConcurrentDictionary<Type, string> _objectTypes = new ConcurrentDictionary<Type, string>();

        public string GetClassNameFromPageRuntimeType(Type type)
        {
            return _classNames.GetOrAdd(type, x => ((TreeNode)Activator.CreateInstance(type)).ClassName.ToLowerInvariant());
        }

        public string GetClassNameFromPageRuntimeType<T>() where T : TreeNode, new()
        {
            return _classNames.GetOrAdd(typeof(T), x => new T().ClassName.ToLowerInvariant());
        }

        public string GetObjectTypeFromInfoObjectRuntimeType(Type type)
        {
            return _objectTypes.GetOrAdd(type, x => ((BaseInfo)Activator.CreateInstance(type)).TypeInfo.ObjectType.ToLowerInvariant());
        }

        public string GetObjectTypeFromInfoObjectRuntimeType<T>() where T : AbstractInfo<T>, new()
        {
            return _objectTypes.GetOrAdd(typeof(T), x => new T().TypeInfo.ObjectType.ToLowerInvariant());
        }
    }
}
