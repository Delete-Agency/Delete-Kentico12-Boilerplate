using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using System;
using System.Web;
using System.Web.Mvc;
using DeleteBoilerplate.DynamicRouting.Helpers;
using RequestContext = System.Web.Routing.RequestContext;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class DynamicHttpHandler : IHttpHandler
    {
        public DynamicHttpHandler(RequestContext requestContext)
        {
            this.RequestContext = requestContext;
        }

        public RequestContext RequestContext { get; set; }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the current page is using a template or not.
        /// </summary>
        /// <param name="page">The Tree Node</param>
        /// <returns>If it has a template or not</returns>
        public static bool PageHasTemplate(TreeNode page)
        {
            var templateConfiguration = page.GetValue("DocumentPageTemplateConfiguration", "");

            // Check Temp Page builder widgets to detect a switch in template
            var instanceGuid = ValidationHelper.GetGuid(URLHelper.GetQueryValue(HttpContext.Current.Request.Url.AbsoluteUri, "instance"), Guid.Empty);
            if (instanceGuid != Guid.Empty)
            {
                var table = ConnectionHelper.ExecuteQuery(string.Format("select PageBuilderTemplateConfiguration from Temp_PageBuilderWidgets where PageBuilderWidgetsGuid = '{0}'", instanceGuid.ToString()), null, QueryTypeEnum.SQLQuery).Tables[0];
                if (table.Rows.Count > 0)
                {
                    templateConfiguration = ValidationHelper.GetString(table.Rows[0]["PageBuilderTemplateConfiguration"], "");
                }
            }

            return !string.IsNullOrWhiteSpace(templateConfiguration) && !templateConfiguration.ToLower().Contains("\"empty.template\"");
        }

        public void ProcessRequest(HttpContext context)
        {
            IController controller = null;
            var factory = ControllerBuilder.Current.GetControllerFactory();

            var defaultController = (RequestContext.RouteData.Values.ContainsKey("controller") ? RequestContext.RouteData.Values["controller"].ToString() : "");
            var defaultAction = (RequestContext.RouteData.Values.ContainsKey("action") ? RequestContext.RouteData.Values["action"].ToString() : "");
            var newController = "";
            var newAction = "Index";
            // Get the classname based on the URL
            var foundNode = DocumentQueryHelper.GetNodeByAliasPath(EnvironmentHelper.GetUrl(context.Request));
            var className = foundNode.ClassName;


            switch (className.ToLower())
            {

                default:
                    // 
                    if (PageHasTemplate(foundNode))
                    {
                        // Uses Page Templates, send to basic Page Template handler
                        newController = "DynamicPageTemplate";
                        newAction = "Index";
                    }
                    else
                    {
                        // Try finding a class that matches the class name
                        newController = className.Replace(".", "_");
                    }
                    break;
                // can add your own cases to do more advanced logic if you wish
                case "":
                    break;

            }

            // Controller not found, use defaults
            if (string.IsNullOrWhiteSpace(newController))
            {
                newController = defaultController;
                newAction = defaultAction;
            }

            // Setup routing with new values
            this.RequestContext.RouteData.Values["Controller"] = newController;

            // If there is an action (2nd value), change it to the CheckNotFound, and remove ID
            if (this.RequestContext.RouteData.Values.ContainsKey("Action"))
            {
                this.RequestContext.RouteData.Values["Action"] = newAction;
            }
            else
            {
                this.RequestContext.RouteData.Values.Add("Action", newAction);
            }
            if (RequestContext.RouteData.Values.ContainsKey("Id"))
            {
                RequestContext.RouteData.Values.Remove("Id");
            }
            try
            {
                controller = factory.CreateController(RequestContext, newController);
                controller.Execute(RequestContext);
            }
            catch (HttpException ex)
            {
                // Catch Controller Not implemented errors and log and go to Not Foud
                if (ex.Message.ToLower().Contains("does not implement icontroller."))
                {
                    EventLogProvider.LogException("DynamicHttpHandler", "ClassControllerNotConfigured", ex, additionalMessage: "Page found, but could not find Page Templates, nor a Controller for " + newController + ", either create Page Templates for this class or create a controller with an index view to auto handle or modify the DynamicHttpHandler");
                    RequestContext.RouteData.Values["Controller"] = "DynamicPageTemplate";
                    RequestContext.RouteData.Values["Action"] = "NotFound";
                    controller = factory.CreateController(RequestContext, "DynamicPageTemplate");
                    controller.Execute(RequestContext);
                }
                else
                {
                    // This will show for any http generated exception, like view errors
                    throw new HttpException(ex.Message, ex);
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.ToLower().Contains("page template with identifier"))
                {
                    // This often occurs when there is a page template assigned that is not defined
                    EventLogProvider.LogException("DynamicHttpHandler", "ClassControllerNotConfigured", ex, additionalMessage: "Page found, but contains a template that is not registered with this application.");
                    RequestContext.RouteData.Values["Controller"] = "DynamicPageTemplate";
                    RequestContext.RouteData.Values["Action"] = "UnregisteredTemplate";
                    controller = factory.CreateController(RequestContext, "DynamicPageTemplate");
                    controller.Execute(RequestContext);
                }
                else
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }

            }
            factory.ReleaseController(controller);
        }
    }
}