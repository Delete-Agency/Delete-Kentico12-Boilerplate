using Kentico.Forms.Web.Mvc;
using Kentico.Forms.Web.Mvc.Widgets;
using System.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Configuration
{
    public static class KenticoFormBuilderStaticMarkupConfiguration
    {
        /// <summary>
        /// Changing the DOM for Kentico Forms
        /// </summary>
        public static void SetGlobalRenderingConfigurations()
        {
            FormWidgetRenderingConfiguration.Default = new FormWidgetRenderingConfiguration
            {
                FormHtmlAttributes = { { "class", "form kentico-form" }, { "data-kentico-form", "" } },

                SubmitButtonHtmlAttributes = { { "class", "btn" } },

                FormWrapperConfiguration = new FormWrapperRenderingConfiguration
                {
                    ElementName = "div",
                    HtmlAttributes = { { "class", "form-module" } }
                }
            };

            // FormFieldRenderingConfiguration.Widget.SuppressValidationMessages = true;

            FormFieldRenderingConfiguration.Widget.RootConfiguration =
                new ElementRenderingConfiguration
                {
                    ElementName = "div",
                    HtmlAttributes = { { "class", "form__row" } },
                    ChildConfiguration = new ElementRenderingConfiguration
                    {
                        ElementName = "div",
                        HtmlAttributes = { { "class", "form-control" } },
                        ChildConfiguration = FormFieldRenderingConfiguration.Widget.LabelWrapperConfiguration
                    }
                };

            FormFieldRenderingConfiguration.Widget.LabelWrapperConfiguration.CustomHtml =
                MvcHtmlString.Create($"{ElementRenderingConfiguration.CONTENT_PLACEHOLDER}");

            FormFieldRenderingConfiguration.Widget.EditorWrapperConfiguration.CustomHtml =
                MvcHtmlString.Create($"{ElementRenderingConfiguration.CONTENT_PLACEHOLDER}");

            FormFieldRenderingConfiguration.Widget.ComponentWrapperConfiguration.CustomHtml =
                MvcHtmlString.Create($"{ElementRenderingConfiguration.CONTENT_PLACEHOLDER}");
        }
    }
}