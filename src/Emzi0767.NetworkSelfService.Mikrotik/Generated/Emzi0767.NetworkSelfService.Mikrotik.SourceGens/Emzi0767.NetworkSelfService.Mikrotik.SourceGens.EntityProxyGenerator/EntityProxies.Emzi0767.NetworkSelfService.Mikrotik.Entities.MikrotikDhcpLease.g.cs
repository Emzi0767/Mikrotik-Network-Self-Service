using System;
using System.Collections.Generic;

namespace Emzi0767.NetworkSelfService.Mikrotik;
internal static partial class EntityProxies
{
    private static readonly IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease>> _propertiesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease = new Dictionary<string, IMikrotikEntityProxyGetterSetter<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease>>()
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
    public static IMikrotikEntityProxy GetProxy(this Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease entity) => new MikrotikEntityProxy<Emzi0767.NetworkSelfService.Mikrotik.Entities.MikrotikDhcpLease>(entity, _propertiesEmzi0767NetworkSelfServiceMikrotikEntitiesMikrotikDhcpLease);
}