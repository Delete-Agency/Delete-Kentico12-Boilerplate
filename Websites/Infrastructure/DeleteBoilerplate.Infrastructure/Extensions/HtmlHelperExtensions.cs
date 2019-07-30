using System.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static partial class HtmlHelperExtensions
    {
        public static bool NoCache(this HtmlHelper helper)
        {
#if _NOCACHE
			return true;
#else
            return false;
#endif
        }
    }
}
