using Backend.Domain.User;
using MediatR;
using System.Reflection;

namespace InterviewCrusherAdmin
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCreateServices(this IServiceCollection services)
        {
 
            return services;
        }
        public static IServiceCollection RegisterGetDocumentByNamesServices(this IServiceCollection services)
        {
           
            return services;
        }

        public static IServiceCollection RegisterInsertGenericServices(this IServiceCollection services)
        {
           
            return services;
        }

        public static IServiceCollection RegisterExportServices(this IServiceCollection services)
        {
             return services;
        }

        public static Assembly[] RegisterServices(this IServiceCollection services)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name.Contains("Backend"))
                .ToList();

            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var allAssemblyFiles = Directory.GetFiles(assemblyPath, "*.dll");

            foreach (var assemblyFile in allAssemblyFiles)
            {
                var assemblyName = Path.GetFileNameWithoutExtension(assemblyFile);

                if (!loadedAssemblies.Any(a => a.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (assemblyName.Contains("Backend"))
                    {
                        try
                        {
                            var assembly = Assembly.LoadFrom(assemblyFile);
                            loadedAssemblies.Add(assembly);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not load assembly {assemblyFile}: {ex.Message}");
                        }
                    }
                }
            }

            return loadedAssemblies.ToArray();
        }

    }
}