using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;

namespace Emzi0767.NetworkSelfService.Mikrotik;
internal static partial class EntityProxies
{
    internal static readonly IReadOnlyDictionary<Type, IEnumerable<string>> _pathRegistry = new Dictionary<Type, IEnumerable<string>>()
    {
        [typeof(Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease)] = _pathEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease,
    };
    public static IEnumerable<string> GetPath<T>()
        where T : class, IMikrotikEntityProxy => _pathRegistry.TryGetValue(typeof(T), out var path) ? path : null;
    public static string MapToSerialized<TEntity, TProp>(Expression<Func<TEntity, TProp>> prop)
        where TEntity : class, IMikrotikEntity => prop.Body is MemberExpression member ? MapToSerialized<TEntity>(member) : null;
    public static string MapToSerialized<TEntity>(MemberExpression member)
        where TEntity : class, IMikrotikEntity => MapToSerialized(typeof(TEntity), member);
    public static string MapToSerialized(Type tEntity, MemberExpression member)
    {
        if (member is not { Member: PropertyInfo property })
            return null;
        var dict = tEntity.FullName switch
        {
            "Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease" => _mapEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease,
            _ => null,
        };
        return dict is not null && dict.TryGetValue(property.Name, out var serialized) ? serialized : null;
    }

    public static string MapFromSerialized<TEntity>(string serialized)
        where TEntity : class, IMikrotikEntity => MapFromSerialized(typeof(TEntity), serialized);
    public static string MapFromSerialized(Type tEntity, string serialized)
    {
        var dict = tEntity.FullName switch
        {
            "Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease" => _unmapEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease,
            _ => null,
        };
        return dict is not null && dict.TryGetValue(serialized, out var name) ? name : null;
    }
}