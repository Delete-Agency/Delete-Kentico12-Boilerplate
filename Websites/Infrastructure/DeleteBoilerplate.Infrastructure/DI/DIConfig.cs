using System;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using DeleteBoilerplate.Infrastructure.Helpers;
using LightInject;

namespace DeleteBoilerplate.Infrastructure
{
    public class DIConfig
    {
        public static void Bootstrap()
        {
            try
            {
                //The code that causes the error goes here.
                var container = new ServiceContainer();

                var assemblies = AssemblyHelper.GetDiscoverableAssemblyAssemblies();
                foreach (var assembly in assemblies)
                {
                    container.RegisterAssembly(assembly);
                }
                container.RegisterControllers(assemblies);

                container.EnableMvc();
                container.EnableAnnotatedPropertyInjection();

                container.RegisterInstance<MapperConfiguration>(AutoMapperConfig.BuildMapperConfiguration(container));
                container.Register<IMapper>(c => new Mapper(c.GetInstance<MapperConfiguration>(), c.GetInstance));
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                //Display or log the error based on your application.
            }
        }

    }
}