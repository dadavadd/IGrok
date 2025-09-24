using IGrok.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IGrok.Tests.IntegrationTests;

public class MyWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        builder.UseEnvironment("Testing")
               .ConfigureServices(services =>
               {
                   var descriptor = services.SingleOrDefault(d =>
                   d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                   if (descriptor != null)
                       services.Remove(descriptor);

                   services.AddDbContext<AppDbContext>(options =>
                   {
                       options.UseSqlite(_connection);
                   });

               });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
