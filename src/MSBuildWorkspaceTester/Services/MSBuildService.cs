using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Logging;

namespace MSBuildWorkspaceTester.Services
{
    internal class MSBuildService : BaseService
    {
        public MSBuildService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Initialize()
        {
            var buildPath = @"C:\Program Files (x86)\dotnet\sdk\5.0.100-preview.7.20366.6";
            MSBuildLocator.RegisterMSBuildPath(buildPath);

            // THis helps with a list NuGet package that are not found easily
            // See https://github.com/microsoft/MSBuildLocator/issues/86 and https://github.com/microsoft/MSBuildLocator/pull/93
            AssemblyLoadContext.Default.Resolving += (assemblyLoadContext, assemblyName) =>
            {
                var path = Path.Combine(buildPath, assemblyName.Name + ".dll");
                if (File.Exists(path))
                {
                    return assemblyLoadContext.LoadFromAssemblyPath(path);
                }

                return null;
            };

            return;

            //var instances = GetVisualStudioInstances();
            //if (instances.Length == 0)
            //{
            //    return;
            //}

            ////var instance = instances[0];
            //var instance = instances.Last();
            //RegisterVisualStudioInstance(instance);
        }

        private VisualStudioInstance[] GetVisualStudioInstances()
        {
            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            if (instances.Length == 0)
            {
                Logger.LogError("No MSBuild instances found.");
                return Array.Empty<VisualStudioInstance>();
            }

            Logger.LogInformation("The following MSBuild instances have benen discovered:");
            Logger.LogInformation(string.Empty);

            for (int i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                Logger.LogInformation($"    {i + 1}. {instance.Name} ({instance.Version})");
            }

            Logger.LogInformation(string.Empty);

            return instances;
        }

        private void RegisterVisualStudioInstance(VisualStudioInstance instance)
        {
            MSBuildLocator.RegisterInstance(instance);

            Logger.LogInformation("Registered first MSBuild instance:");
            Logger.LogInformation(string.Empty);
            Logger.LogInformation($"    Name: {instance.Name}");
            Logger.LogInformation($"    Version: {instance.Version}");
            Logger.LogInformation($"    VisualStudioRootPath: {instance.VisualStudioRootPath}");
            Logger.LogInformation($"    MSBuildPath: {instance.MSBuildPath}");
            Logger.LogInformation(string.Empty);
        }
    }
}
