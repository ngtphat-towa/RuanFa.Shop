using System.Net.Mail;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Notifications;
using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Application.Common.Security.Authorization.Services;
using RuanFa.Shop.Application.Common.Security.Tokens;
using RuanFa.Shop.Application.Common.Services;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using RuanFa.Shop.Infrastructure.Accounts.Services;
using RuanFa.Shop.Infrastructure.Data;
using RuanFa.Shop.Infrastructure.Data.Interceptors;
using RuanFa.Shop.Infrastructure.Data.Seeds;
using RuanFa.Shop.Infrastructure.Notifications;
using RuanFa.Shop.Infrastructure.Notifications.Emails;
using RuanFa.Shop.Infrastructure.Notifications.Sms;
using RuanFa.Shop.Infrastructure.Security.Authentication;
using RuanFa.Shop.Infrastructure.Security.Authorization;
using RuanFa.Shop.Infrastructure.Security.Tokens;
using RuanFa.Shop.Infrastructure.Services;
using RuanFa.Shop.Infrastructure.Settings;
using Serilog;

namespace RuanFa.Shop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        services
            .AddSettingConfiguration(configuration)
            .AddServices()
            .AddDatabase(configuration)
            .AddIdentityServices()
            .AddAuthenticationInternal(configuration)
            .AddHttpClientWithClientId()
            .AddSeeders();

        return services;
    }

    private static IServiceCollection AddSettingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StripePaymentSettings>(configuration.GetSection(StripePaymentSettings.Section));
        services.Configure<ClientSettings>(configuration.GetSection(ClientSettings.Section));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.Section));
        services.Configure<SmsSettings>(configuration.GetSection(SmsSettings.Section));
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.Section));

        var emailSettings = configuration.GetSection(EmailSettings.Section).Get<EmailSettings>();
        Guard.Against.Null(emailSettings, message: "EmailSettings not found in configuration.");
        if (emailSettings.EnableEmailNotifications)
        {
            services.AddFluentEmail(emailSettings.FromEmail)
            .AddSmtpSender(new SmtpClient(emailSettings.SmtpSettings.Server)
            {
                Port = emailSettings.SmtpSettings.Port, // Use PaperCut's default SMTP port
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false,  // Disable SSL if not required
                UseDefaultCredentials = true
            });
        }

        var smsSettings = configuration.GetSection(SmsSettings.Section).Get<SmsSettings>();
        Guard.Against.Null(smsSettings, message: "SmsSettings not found in configuration.");

        Log.Information("Init infrastructure layer: Setting Done");

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Systems
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
        // Identity
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddSingleton<IPolicyEnforcer, PolicyEnforcer>();
        // Notification
        services.AddSingleton<IEmailSenderService, EmailSenderService>();
        services.AddSingleton<ISmsSenderService, SmsSenderService>();
        services.AddSingleton<INotificationService, NotificationService>();
        // Storages
        services.AddScoped<IStorageService, FluentStorageService>();
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection string
        var connectionString = configuration.GetConnectionString(DbConnectionSetting.Postgres);
        Guard.Against.Null(connectionString, message: $"Connection string '{DbConnectionSetting.Postgres}' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseNpgsql(connectionString)
            options.UseInMemoryDatabase(connectionString)
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging() // <-- Show parameter values
                .EnableDetailedErrors();      // <-- More detailed error messages;
        });

        // Database Service
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Caches
        services.AddDistributedMemoryCache();

        return services;
    }
    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Configure password policy
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Configure lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Configure user settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;

        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
    {
        // Add configs
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        services.Configure<SocialLoginSettings>(configuration.GetSection(SocialLoginSettings.Section));

        // Validate config
        var scpialLoginSettings = configuration.GetSection(SocialLoginSettings.Section).Get<SocialLoginSettings>();
        var jwtSettings = configuration.GetSection(JwtSettings.Section).Get<JwtSettings>();

        Guard.Against.Null(
            jwtSettings, 
            message: $"{JwtSettings.Section} not found in configuration.");
        Guard.Against.Null(
            scpialLoginSettings, 
            message: $"{SocialLoginSettings.Section} not found in configuration.");
        Guard.Against.Null(
            scpialLoginSettings.Google, 
            message: $"{nameof(SocialLoginSettings.Google)} not found in configuration.");

        Guard.Against.NullOrEmpty(
            scpialLoginSettings.Google.ClientId, 
            message: $"{nameof(SocialLoginSettings.Google.ClientId)} not found in configuration.");

        if (jwtSettings == null
            || string.IsNullOrEmpty(jwtSettings.Issuer)
            || string.IsNullOrEmpty(jwtSettings.Audience)
            || string.IsNullOrEmpty(jwtSettings.Secret))
        {
            throw new InvalidOperationException("JWT settings are not properly configured.");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<ITokenProvider, TokenProvider>();


        return services;
    }
    private static IServiceCollection AddHttpClientWithClientId(this IServiceCollection services)
    {
        services.AddHttpClient();
        // Register Facebook-specific HTTP client
        services.AddHttpClient("FacebookAuth", client =>
        {
            client.BaseAddress = new Uri("https://graph.facebook.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
    private static IServiceCollection AddSeeders(this IServiceCollection services)
    {
        // Register seeders
        services.AddTransient<IDataSeeder, IdentitySeedProvider>();

        // Register orchestrator as hosted service
        services.AddHostedService<SeedOrchestrator>();

        return services;
    }
}
