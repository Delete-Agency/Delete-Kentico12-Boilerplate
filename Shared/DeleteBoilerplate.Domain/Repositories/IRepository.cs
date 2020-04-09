using CMS.Base;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IRepository<TInfo> where TInfo : ICMSObject
    {
        /// <summary>
        /// Resolve a context item
        /// </summary>
        /// <param name="id">ContextItemDocumentId</param>
        /// <returns>ContextItem</returns>
        TInfo GetById(int id);
    }
}