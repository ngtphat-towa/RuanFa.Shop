using System.Reflection;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.SharedKernel.Utilities;

/// <summary>
/// Utility class for discovering user-related commands and queries.
/// </summary>
public static class UserMessageDiscovery
{
    /// <summary>
    /// Retrieves all types that implement <see cref="IUserMessage"/> (i.e., user-related commands and queries).
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for types. If null, uses the calling assembly.</param>
    /// <returns>A collection of types that implement <see cref="IUserMessage"/>.</returns>
    public static IEnumerable<Type> GetUserMessageTypes(params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IUserMessage).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();
    }

    /// <summary>
    /// Filters user messages by a specific user ID.
    /// </summary>
    /// <param name="messages">The collection of user messages to filter.</param>
    /// <param name="userId">The user ID to filter by.</param>
    /// <returns>A collection of user messages matching the specified user ID.</returns>
    public static IEnumerable<IUserMessage> FilterByUserId(IEnumerable<IUserMessage> messages, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Enumerable.Empty<IUserMessage>();
        }

        return messages.Where(m => m.UserId == userId).ToList();
    }
}
