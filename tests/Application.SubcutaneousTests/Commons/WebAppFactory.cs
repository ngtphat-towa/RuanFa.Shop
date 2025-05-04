using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using RuanFa.Shop.Infrastructure.Data;
using RuanFa.Shop.Tests.Shared.Security;
using RuanFa.Shop.Web.Api;

namespace RuanFa.Shop.Application.SubcutaneousTests.Commons;

public class WebAppFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    public TestUserContext TestUserContext { get; private set; } = new();
    public TestCurrentUserProvider TestCurrentUserProvider { get; private set; } = new();
    public PostgresTestDatabase TestDatabase { get; set; } = null!;
    public UserManager<ApplicationUser> UserManager = null!;
    public RoleManager<ApplicationRole> RoleManager = null!;
    public ApplicationDbContext DbContext = null!;
    public IServiceProvider? ServiceProvider { get; set; }

    public IMediator CreateMediator()
    {
        IServiceScope serviceScope = Services.CreateScope();
        ServiceProvider = serviceScope.ServiceProvider;

        TestDatabase.ResetDatabase();

        UserManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        RoleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        DbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new Task DisposeAsync()
    {
        TestDatabase.Dispose();
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Replace SQLite with PostgreSQL
        TestDatabase = PostgresTestDatabase.CreateAndInitialize(
            "Server=localhost;Database=quingfa.eshop;Username=postgres;Password=123456789");

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUserContext>()
                .AddScoped<IUserContext>(_ => TestUserContext);

            services
                .RemoveAll<ICurrentUserProvider>()
                .AddScoped<ICurrentUserProvider>(_ => TestCurrentUserProvider);

            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.UseNpgsql(TestDatabase.Connection);
                    options.UseSnakeCaseNamingConvention();
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                });

            services.AddMemoryCache();
        });

        builder.ConfigureAppConfiguration((context, conf) => conf.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "EmailSettings:EnableEmailNotifications", "false" },
        }));
    }
}
