using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL.Services;

public class AppContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = GetDbContextOptionsBuilder();
        return new ApplicationContext(optionsBuilder.Options);
    }

    private DbContextOptionsBuilder<ApplicationContext> GetDbContextOptionsBuilder()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("/Users/dayanagavrylenko/Desktop/Web/dotNet/WebTechnicalTask/ProjectManager/UI/bin/Debug/net6.0/appsettings.json");
        var config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return optionsBuilder;
    }
}