// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using UI.ConsoleManagers;
// using UI.Interfaces;

// namespace UI;

// public class DependencyRegistration
// {
    // public static IServiceProvider Register()
    // {
    //     var services = new ServiceCollection();
    //
    //     services.AddScoped<AppManager>();
    //     services.AddScoped<UserConsoleManager>();
    //     services.AddScoped<ProjectConsoleManager>();
    //     services.AddScoped<ProjectTaskConsoleManager>();
    //     services.AddScoped<DeveloperConsoleManager>();
    //     services.AddScoped<TesterConsoleManager>();
    //     services.AddScoped<StakeHolderConsoleManager>();
    //     services.AddScoped<InitialConsoleManager>();
    //
    //     foreach (Type type in typeof(IConsoleManager<>).Assembly.GetTypes()
    //                  .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
    //                      .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsoleManager<>))))
    //     {
    //         Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsoleManager<>));
    //         services.AddScoped(interfaceType, type);
    //     }
    //     var configuration = new ConfigurationBuilder()
    //         .SetBasePath(Directory.GetCurrentDirectory())
    //         .AddJsonFile("appsettings.json")
    //         .Build();
    //
    //     string connectionString = configuration.GetConnectionString("DefaultConnection");
    //
    //     BLL.DependencyRegistration.RegisterServices(services, connectionString);
    //
    //     return services.BuildServiceProvider();
    // }
// }