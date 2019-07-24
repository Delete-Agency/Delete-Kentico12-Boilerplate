using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate("Empty.Template", "No Template", customViewName: "PageTemplates/_EmptyTemplate", Description = "No Template (Use standard Dynamic Routing)", IconClass = "icon-modal-close")]

[assembly: RegisterPageTemplate("Blank.Widget", "Blank Widget Page", customViewName: "PageTemplates/_BlankWidgetTemplate", Description = "Blank page with a widget zone")]