using System;
using System.Collections.Generic;
using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Search.Azure;
using DeleteBoilerplate.Modules;

[assembly: RegisterModule(typeof(AzureSearchIndexFieldTypesModule))]

namespace DeleteBoilerplate.Modules
{
    //based on https://devnet.kentico.com/articles/customizing-azure-search-fields
    public class AzureSearchIndexFieldTypesModule : Module
    {
        private const string TaxonomyFieldName = "taxonomy";

        public AzureSearchIndexFieldTypesModule() : base("DeleteBoilerplate.Modules.AzureSearchIndexFields")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();
            DocumentFieldCreator.Instance.CreatingField.After += CreatingField_After;
            DocumentCreator.Instance.AddingDocumentValue.Execute += AddingDocumentValue_Execute;
            //DataMapper.Instance.RegisterMapping(typeof(GeographyPoint), Microsoft.Azure.Search.Models.DataType.GeographyPoint);
            DataMapper.Instance.RegisterMapping(typeof(IEnumerable<string>), Microsoft.Azure.Search.Models.DataType.Collection(Microsoft.Azure.Search.Models.DataType.String));
        }

        protected void CreatingField_After(object sender, CreateFieldEventArgs e)
        {
            if (string.Equals(e.SearchField.FieldName, TaxonomyFieldName, StringComparison.OrdinalIgnoreCase))
            {
                e.Field.Type = Microsoft.Azure.Search.Models.DataType.Collection(Microsoft.Azure.Search.Models.DataType.String);
            }
        }

        protected void AddingDocumentValue_Execute(object sender, AddDocumentValueEventArgs e)
        {
            if (string.Equals(e.AzureName, TaxonomyFieldName, StringComparison.OrdinalIgnoreCase))
            {
                if (!DataHelper.IsEmpty(e.Value))
                {
                    e.Value = e.Value.ToString().Split(' ');
                }
            }
        }
    }
}