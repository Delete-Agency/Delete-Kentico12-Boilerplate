using System.Web;
using DeleteBoilerplate.Infrastructure.Personalization.ConditionTypes;
using Kentico.PageBuilder.Web.Mvc.Personalization;
using Microsoft.Owin.Security;

[assembly: RegisterPersonalizationConditionType("DeleteBoilerplate.Personalization.IsAuthenticatedUser",
    typeof(IsAuthenticatedUserConditionType),
    "{$DeleteBoilerplate.ConditionType.IsAuthenticatedUser.Name$}",
    Description = "{$DeleteBoilerplate.ConditionType.IsAuthenticatedUser.Description$}",
    IconClass = "icon-user-checkbox",
    Hint = "{$DeleteBoilerplate.ConditionType.IsAuthenticatedUser.Hint$}")]

namespace DeleteBoilerplate.Infrastructure.Personalization.ConditionTypes
{
    public class IsAuthenticatedUserConditionType : ConditionType
    {
        private IAuthenticationManager AuthManager => HttpContext.Current.GetOwinContext().Authentication;

        public override bool Evaluate()
        {
            return this.AuthManager.User.Identity.IsAuthenticated;
        }
    }
}