using System;
using System.Collections.Generic;
using System.Linq;
using CMS;
using CMS.DataEngine;
using CMS.DocumentEngine;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Modules;

[assembly: RegisterModule(typeof(TaxonomyModule))]
namespace DeleteBoilerplate.Modules
{
    public class TaxonomyModule : Module
    {
        public TaxonomyModule() : base("DeleteBoilerplate.Modules.Taxonomy")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            DocumentEvents.Update.Before += UpdateOnBefore;
            DocumentEvents.Insert.Before += UpdateOnBefore;

            DocumentEvents.Update.After += UpdateOnAfter;
            DocumentEvents.Insert.After += UpdateOnAfter;
        }

        private void UpdateOnBefore(object sender, DocumentEventArgs e)
        {
            // Combine all taxonomy fields into one
            var columns = GetNamesBySearchFieldsSelector(e.Node.ClassName,
                x => x.FieldName.StartsWith(Constants.Taxonomy.SearchFieldNamePrefix, StringComparison.OrdinalIgnoreCase)).Values;

            var combinedTaxonomy = new List<string>();

            if (e.Node.ContainsColumn(Constants.Taxonomy.SearchFieldNamePrefix))
            {
                // Assign some of taxonomy fields from parent
                if (e.Node.Parent != null)
                {
                    var itemParentReferencedFields = GetNamesBySearchFieldsSelector(e.Node.ClassName,
                        x => x.FieldName.StartsWith(Constants.Taxonomy.ParentSearchFieldNamePrefix, StringComparison.OrdinalIgnoreCase));

                    var parentFieldsToProcess = GetNamesBySearchFieldsSelector(e.Node.Parent.ClassName,
                            x => x.FieldName.StartsWith(Constants.Taxonomy.SearchFieldNamePrefix, StringComparison.OrdinalIgnoreCase))
                        .Where(x => itemParentReferencedFields.ContainsKey($"{Constants.Taxonomy.ParentSearchFieldNamePrefix}{x.Value}"));

                    foreach (var parentField in parentFieldsToProcess)
                    {
                        var parentFieldValue = e.Node.Parent[parentField.Value];

                        var itemField = itemParentReferencedFields[$"{Constants.Taxonomy.ParentSearchFieldNamePrefix}{parentField.Value}"];

                        e.Node[itemField] = parentFieldValue;
                    }
                }
            }

            foreach (var column in columns)
            {
                combinedTaxonomy.AddRange(e.Node.GetStringValue(column, String.Empty)
                    .Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries));
            }

            var result = string.Join(" ", combinedTaxonomy);

            e.Node[Constants.Taxonomy.SearchFieldNamePrefix] = result;
        }

        private IDictionary<string, string> GetNamesBySearchFieldsSelector(string className,
            Func<SearchSettingsInfo, bool> searchFieldSelector)
        {
            var pageTypeInfo = DataClassInfoProvider.GetDataClassInfo(className);

            var result = pageTypeInfo.ClassSearchSettingsInfos.Items.TypedValues.Where(x =>
                    x.FieldName != null && searchFieldSelector(x))
                .ToDictionary(k => k.FieldName, v => v.Name, StringComparer.OrdinalIgnoreCase);

            return result;
        }

        private void UpdateOnAfter(object sender, DocumentEventArgs e)
        {
            // Fire update for child items in case 
            if (e.Node.ContainsColumn(Constants.Taxonomy.SearchFieldNamePrefix))
            {
                var classesRequiringUpdate = GetClassNamesWithTaxonomyParentReferencies();

                foreach (var child in e.Node.Children.WithAllData.Where(x => classesRequiringUpdate.Contains(x.ClassName)))
                {
                    child.Update();
                }
            }
        }

        private IEnumerable<string> GetClassNamesWithTaxonomyParentReferencies()
        {
            return DataClassInfoProvider.GetClasses()
                .Where(x => x.ClassSearchSettingsInfos.Items.TypedValues.Any(y =>
                    y.FieldName != null && y.FieldName.StartsWith(Constants.Taxonomy.ParentSearchFieldNamePrefix, StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.ClassName);
        }
    }
}