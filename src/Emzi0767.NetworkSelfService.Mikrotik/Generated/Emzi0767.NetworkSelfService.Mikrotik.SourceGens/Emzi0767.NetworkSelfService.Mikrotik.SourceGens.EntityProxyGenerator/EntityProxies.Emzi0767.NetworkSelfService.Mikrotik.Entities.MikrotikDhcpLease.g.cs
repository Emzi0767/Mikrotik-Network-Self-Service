using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Emzi0767.NetworkSelfService.Mikrotik;
internal static partial class EntityProxies
{
    private static readonly IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease>> _proxiesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease = new Dictionary<string, IMikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease>>()
    {
        [".id"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, string>(static x => x.Id, static (x, v) => x.Id = v),
        ["address"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, System.Net.IPAddress>(static x => x.Address, static (x, v) => x.Address = v),
        ["address-list"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, System.Collections.Generic.IEnumerable<string>>(static x => x.AddressLists, static (x, v) => x.AddressLists = v),
        ["allow-dual-stack-queue"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, bool>(static x => x.AllowsDualStackQueue, static (x, v) => x.AllowsDualStackQueue = v),
        ["always-broadcast"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, bool>(static x => x.AlwaysBroadcasts, static (x, v) => x.AlwaysBroadcasts = v),
        ["block-access"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, bool>(static x => x.AccessBlocked, static (x, v) => x.AccessBlocked = v),
        ["client-id"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, string>(static x => x.ClientId, static (x, v) => x.ClientId = v),
        ["dhcp-option"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, System.Collections.Generic.IEnumerable<string>>(static x => x.DhcpOptions, static (x, v) => x.DhcpOptions = v),
        ["dhcp-option-set"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, System.Collections.Generic.IEnumerable<string>>(static x => x.DhcpOptionSets, static (x, v) => x.DhcpOptionSets = v),
        ["insert-queue-before"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, string>(static x => x.InsertQueueBefore, static (x, v) => x.InsertQueueBefore = v),
        ["lease-time"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, System.TimeSpan>(static x => x.LeaseTime, static (x, v) => x.LeaseTime = v),
        ["mac-address"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, Emzi0767.NetworkSelfService.Mikrotik.Types.MacAddress>(static x => x.MacAddress, static (x, v) => x.MacAddress = v),
        ["parent-queue"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, string>(static x => x.ParentQueue, static (x, v) => x.ParentQueue = v),
        ["queue-type"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, Emzi0767.NetworkSelfService.Mikrotik.Types.MikrotikDhcpQueueType>(static x => x.QueueType, static (x, v) => x.QueueType = v),
        ["rate-limit"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, Emzi0767.NetworkSelfService.Mikrotik.Types.MikrotikDhcpRateLimit>(static x => x.RateLimit, static (x, v) => x.RateLimit = v),
        ["routes"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, System.Collections.Generic.IEnumerable<Emzi0767.NetworkSelfService.Mikrotik.Types.MikrotikDhcpRoute>>(static x => x.Routes, static (x, v) => x.Routes = v),
        ["server"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, string>(static x => x.Server, static (x, v) => x.Server = v),
        ["use-src-mac"] = new MikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, bool>(static x => x.UseSourceMac, static (x, v) => x.UseSourceMac = v),
    };
    private static readonly IReadOnlyDictionary<string, string> _unmapEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease = new Dictionary<string, string>()
    {
        [".id"] = "Id",
        ["address"] = "Address",
        ["address-list"] = "AddressLists",
        ["allow-dual-stack-queue"] = "AllowsDualStackQueue",
        ["always-broadcast"] = "AlwaysBroadcasts",
        ["block-access"] = "AccessBlocked",
        ["client-id"] = "ClientId",
        ["dhcp-option"] = "DhcpOptions",
        ["dhcp-option-set"] = "DhcpOptionSets",
        ["insert-queue-before"] = "InsertQueueBefore",
        ["lease-time"] = "LeaseTime",
        ["mac-address"] = "MacAddress",
        ["parent-queue"] = "ParentQueue",
        ["queue-type"] = "QueueType",
        ["rate-limit"] = "RateLimit",
        ["routes"] = "Routes",
        ["server"] = "Server",
        ["use-src-mac"] = "UseSourceMac",
    };
    private static readonly IReadOnlyDictionary<string, string> _mapEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease = new Dictionary<string, string>()
    {
        ["Id"] = ".id",
        ["Address"] = "address",
        ["AddressLists"] = "address-list",
        ["AllowsDualStackQueue"] = "allow-dual-stack-queue",
        ["AlwaysBroadcasts"] = "always-broadcast",
        ["AccessBlocked"] = "block-access",
        ["ClientId"] = "client-id",
        ["DhcpOptions"] = "dhcp-option",
        ["DhcpOptionSets"] = "dhcp-option-set",
        ["InsertQueueBefore"] = "insert-queue-before",
        ["LeaseTime"] = "lease-time",
        ["MacAddress"] = "mac-address",
        ["ParentQueue"] = "parent-queue",
        ["QueueType"] = "queue-type",
        ["RateLimit"] = "rate-limit",
        ["Routes"] = "routes",
        ["Server"] = "server",
        ["UseSourceMac"] = "use-src-mac",
    };
    private static readonly IReadOnlyDictionary<string, Type> _typesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease = new Dictionary<string, Type>()
    {
        ["Id"] = typeof(string),
        ["Address"] = typeof(System.Net.IPAddress),
        ["AddressLists"] = typeof(System.Collections.Generic.IEnumerable<string>),
        ["AllowsDualStackQueue"] = typeof(bool),
        ["AlwaysBroadcasts"] = typeof(bool),
        ["AccessBlocked"] = typeof(bool),
        ["ClientId"] = typeof(string),
        ["DhcpOptions"] = typeof(System.Collections.Generic.IEnumerable<string>),
        ["DhcpOptionSets"] = typeof(System.Collections.Generic.IEnumerable<string>),
        ["InsertQueueBefore"] = typeof(string),
        ["LeaseTime"] = typeof(System.TimeSpan),
        ["MacAddress"] = typeof(Emzi0767.NetworkSelfService.Mikrotik.Types.MacAddress),
        ["ParentQueue"] = typeof(string),
        ["QueueType"] = typeof(Emzi0767.NetworkSelfService.Mikrotik.Types.MikrotikDhcpQueueType),
        ["RateLimit"] = typeof(Emzi0767.NetworkSelfService.Mikrotik.Types.MikrotikDhcpRateLimit),
        ["Routes"] = typeof(System.Collections.Generic.IEnumerable<Emzi0767.NetworkSelfService.Mikrotik.Types.MikrotikDhcpRoute>),
        ["Server"] = typeof(string),
        ["UseSourceMac"] = typeof(bool),
    };
    private static readonly string[] _pathEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease = ["ip", "dhcp-server", "lease", ];
    private static readonly string[] _propertiesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease = ["Id", "Address", "AddressLists", "AllowsDualStackQueue", "AlwaysBroadcasts", "AccessBlocked", "ClientId", "DhcpOptions", "DhcpOptionSets", "InsertQueueBefore", "LeaseTime", "MacAddress", "ParentQueue", "QueueType", "RateLimit", "Routes", "Server", "UseSourceMac", ];
    public static IMikrotikEntityProxy GetProxy(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity) => new MikrotikEntityProxy<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease>(entity, _proxiesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease);
    public static string MapToSerialized<T>(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity, Expression<Func<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease, T>> prop) => prop.Body is MemberExpression member ? entity.MapToSerialized(member) : null;
    public static string MapToSerialized(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity, MemberExpression member) => member is { Member: PropertyInfo property } ? (_mapEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease.TryGetValue(property.Name, out var serialized) ? serialized : null) : null;
    public static string MapFromSerialized(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity, string serialized) => _unmapEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease.TryGetValue(serialized, out var name) ? name : null;
    public static Type GetPropertyType(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity, string name) => _typesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease.TryGetValue(name, out var type) ? type : null;
    public static IEnumerable<string> GetPath(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity) => _pathEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease;
    public static IEnumerable<string> GetProperties(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity) => _propertiesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease;
}