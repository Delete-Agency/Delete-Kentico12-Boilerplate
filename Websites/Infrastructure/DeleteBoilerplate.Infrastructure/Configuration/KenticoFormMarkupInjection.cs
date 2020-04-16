using Kentico.Forms.Web.Mvc;
using System;

namespace DeleteBoilerplate.Infrastructure.Configuration
{
    public static class KenticoFormMarkupInjection
    { 
        public static void RegisterEventHandlers()
        {
            FormFieldRenderingConfiguration.GetConfiguration.Execute += InjectMarkupIntoKenticoForm;
        }

        private static void InjectMarkupIntoKenticoForm(object sender, GetFormFieldRenderingConfigurationEventArgs e)
        {
            if (IsDefaultKenticoForm(e) == false)
            {
                return;
            }

            AddSpecificMarkupToComponents(e);
        }

        private static bool IsDefaultKenticoForm(GetFormFieldRenderingConfigurationEventArgs e)
        {
            return e.FormComponent.Definition.Identifier.StartsWith("Kentico", StringComparison.InvariantCultureIgnoreCase);
        }

        private static void AddSpecificMarkupToComponents(GetFormFieldRenderingConfigurationEventArgs e)
        {
            if (e.FormComponent is TextInputComponent || e.FormComponent is EmailInputComponent || e.FormComponent is USPhoneComponent || e.FormComponent is IntInputComponent)
            {
                e.Configuration.LabelHtmlAttributes["class"] = "form-control__label";
                e.Configuration.EditorHtmlAttributes["class"] = "input-text form__input-text";
            }

            if (e.FormComponent is TextAreaComponent)
            {
                e.Configuration.LabelHtmlAttributes["class"] = "form-control__label";
                e.Configuration.EditorHtmlAttributes["class"] = "textarea form__textarea";
            }

            if (e.FormComponent is CheckBoxComponent)
            {
                e.Configuration.EditorHtmlAttributes["class"] = "form-control__checkbox";
            }
        }
    }
}