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
        where T : class, IMikrotikEntity => GetPath(typeof(T));
    public static IEnumerable<string> GetPath(Type t) => _pathRegistry.TryGetValue(t, out var path) ? path : null;
    public static string MapToSerialized<TEntity, TProp>(Expression<Func<TEntity, TProp>> prop)
        where TEntity : class, IMikrotikEntity => prop.Body is MemberExpression member ? MapToSerialized<TEntity>(member) : null;
    public static string MapToSerialized<TEntity>(MemberExpression member)
        where TEntity : class, IMikrotikEntity => MapToSerialized(typeof(TEntity), member);
    public static string MapToSerialized<TEntity>(MemberExpression member, string name)
        where TEntity : class, IMikrotikEntity => MapToSerialized(typeof(TEntity), name);
    public static string MapToSerialized(Type tEntity, MemberExpression member) => member is { Member: PropertyInfo property } ? MapToSerialized(tEntity, property.Name) : null;
    public static string MapToSerialized(Type tEntity, string name)
    {
        var dict = tEntity.FullName switch
        {
            "Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease" => _mapEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease,
            _ => null,
        };
        return dict is not null && dict.TryGetValue(name, out var serialized) ? serialized : null;
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

    public static IEnumerable<string> GetProperties<T>()
        where T : class, IMikrotikEntity => GetProperties(typeof(T));
    public static IEnumerable<string> GetProperties(Type t) => t.FullName switch
    {
        "Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease" => _propertiesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease,
        _ => null,
    };
    public static IMikrotikEntityProxy GetProxy(Type t, object entity) => t.FullName switch
    {
        "Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease" => (entity as Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease).GetProxy(),
        _ => null,
    };
}