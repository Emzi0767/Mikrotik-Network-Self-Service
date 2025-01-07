using System;
using System.Collections.Generic;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;

namespace Emzi0767.NetworkSelfService.Mikrotik;
internal interface IMikrotikEntityProxyGetterSetter<T>
    where T : IMikrotikEntity
{
    object Get(T @this);
    void Set(T @this, object value);
}

internal readonly struct MikrotikEntityProxyGetterSetter<T, TProp> : IMikrotikEntityProxyGetterSetter<T> where T : IMikrotikEntity
{
    public Func<T, TProp> Getter { get; }
    public Action<T, TProp> Setter { get; }

    public MikrotikEntityProxyGetterSetter(Func<T, TProp> getter, Action<T, TProp> setter)
    {
        this.Getter = getter;
        this.Setter = setter;
    }

    object IMikrotikEntityProxyGetterSetter<T>.Get(T @this) => this.Getter(@this);
    void IMikrotikEntityProxyGetterSetter<T>.Set(T @this, object value) => this.Setter(@this, (TProp)value);
}

internal interface IMikrotikEntityProxy
{
    object Get(string name);
    void Set(string name, object value);
}

internal readonly struct MikrotikEntityProxy<T> : IMikrotikEntityProxy where T : IMikrotikEntity
{
    private T This { get; }
    private IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<T>> PropertyProxies { get; }

    public MikrotikEntityProxy(T @this, IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<T>> propertyProxies)
    {
        this.This = @this;
        this.PropertyProxies = propertyProxies;
    }

    public object GetProperty(string name)
    {
        return this.PropertyProxies.TryGetValue(name, out var proxy) ? proxy.Get(this.This) : null;
    }

    public void SetProperty(string name, object value)
    {
        if (this.PropertyProxies.TryGetValue(name, out var proxy))
            proxy.Set(this.This, value);
    }

    object IMikrotikEntityProxy.Get(string name) => this.GetProperty(name);
    void IMikrotikEntityProxy.Set(string name, object value) => this.SetProperty(name, value);
}