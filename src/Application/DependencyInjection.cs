using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using RuanFa.Shop.Application.Common.Behaviours;
using Serilog;

namespace RuanFa.Shop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            config.AddOpenBehavior(typeof(AuthorizationPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddMappings();

        Log.Information("Add applications.");
        return services;
    }


    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // Log before scanning
        Log.Information("Before scanning: Starting Mapster configuration");

        // Scan for registers
        IList<IRegister> registers = config.Scan(Assembly.GetExecutingAssembly());
        Log.Information($"Found {registers.Count} register(s) in assembly");

        // Log each register for debugging
        foreach (var register in registers)
        {
            Log.Information($"Applying register: {register.GetType().FullName}");
        }

        // Apply the registers
        config.Apply(registers);
        config.AllowImplicitDestinationInheritance = true;

        // Force compile all mappings to ensure they're registered in the map dictionary
        config.Compile();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // Log after configuration
        LogAllMapsterMappings(config);

        return services;
    }

    private static void LogAllMapsterMappings(TypeAdapterConfig config)
    {
        Log.Information("Attempting to log Mapster mappings...");

        try
        {
            // Get mappings using MapsterMapper's methods instead of reflection when possible
            var mappings = config.RuleMap;

            if (mappings != null)
            {
                Log.Information($"Found {mappings.Count} mappings in RuleMap.");

                foreach (var mapping in mappings)
                {
                    var sourceType = mapping.Key.Source;
                    var destType = mapping.Key.Destination;

                    Log.Information(
                        "⟹ Map: {Source} → {Dest}",
                        sourceType.FullName,
                        destType.FullName
                    );
                }
                return;
            }
        }
        catch (Exception ex)
        {
            Log.Warning($"Couldn't access mappings via RuleMap: {ex.Message}");
        }

        // Fallback to reflection approach
        try
        {
            // Try different field names that might exist in different Mapster versions
            foreach (var fieldName in new[] { "_mapDict", "Maps", "RuleMap", "_adaptations" })
            {
                var field = typeof(TypeAdapterConfig)
                    .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                if (field != null)
                {
                    Log.Information($"Found field: {fieldName}");
                    var mapDict = field.GetValue(config) as System.Collections.IDictionary;

                    if (mapDict != null && mapDict.Count > 0)
                    {
                        Log.Information($"Found {mapDict.Count} mappings via field {fieldName}.");

                        foreach (var keyObj in mapDict.Keys)
                        {
                            try
                            {
                                Log.Information($"Processing mapping key: {keyObj}");

                                // Try to extract source and destination types
                                // This approach varies based on Mapster version
                                Type? sourceType = null;
                                Type? destType = null;

                                if (keyObj.GetType().Name.Contains("Tuple"))
                                {
                                    var tuple = (dynamic)keyObj;
                                    sourceType = tuple.Source;
                                    destType = tuple.Destination;
                                }
                                else if (keyObj.GetType().Name.Contains("TypeTuple"))
                                {
                                    var propSource = keyObj.GetType().GetProperty("Source");
                                    var propDest = keyObj.GetType().GetProperty("Destination");
                                    if (propSource != null && propDest != null)
                                    {
                                        sourceType = (Type)propSource.GetValue(keyObj)!;
                                        destType = (Type)propDest.GetValue(keyObj)!;
                                    }
                                }

                                if (sourceType != null && destType != null)
                                {
                                    Log.Information(
                                        "⟹ Map: {Source} → {Dest}",
                                        sourceType.FullName,
                                        destType.FullName
                                    );
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Warning($"Error processing key {keyObj}: {ex.Message}");
                            }
                        }
                        return;
                    }
                }
            }

            Log.Warning("Could not find mapping dictionary in TypeAdapterConfig.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while accessing Mapster mappings via reflection.");
        }
    }
}
