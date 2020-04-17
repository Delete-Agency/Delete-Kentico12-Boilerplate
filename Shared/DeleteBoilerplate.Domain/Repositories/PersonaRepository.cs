using System.Collections.Generic;
using CMS.Personas;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;

namespace DeleteBoilerplate.Domain.Repositories
{
    /// <summary>
    /// Provides operations for personas.
    /// </summary>
    public interface IPersonaRepository : IRepository<PersonaInfo>
    {
        /// <summary>
        /// Returns an enumerable collection of all personas.
        /// </summary>
        IEnumerable<PersonaInfo> GetAll();
    }

    /// <summary>
    /// Provides operations for personas.
    /// </summary>
    public class KenticoPersonaRepository : IPersonaRepository
    {
        public PersonaInfo GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerable collection of all personas.
        /// </summary>
        [CacheDependency("personas.persona|all")]
        public IEnumerable<PersonaInfo> GetAll()
        {
            return PersonaInfoProvider.GetPersonas();
        }
    }
}