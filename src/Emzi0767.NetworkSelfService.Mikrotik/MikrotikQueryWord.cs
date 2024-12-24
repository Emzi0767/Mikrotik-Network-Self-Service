using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API query word.
/// </summary>
public sealed class MikrotikQueryWord : IMikrotikWord
{
    internal const string ValuePlaceholder = "{}";
    
    private static readonly IReadOnlyDictionary<MikrotikQueryType, string> _typeDictionary;
    private static readonly IReadOnlyDictionary<MikrotikQueryOperation, Func<String, String>> _operationDictionary;
    
    /// <summary>
    /// Gets the prefix for all attributes.
    /// </summary>
    public const string Prefix = "?";

    /// <summary>
    /// Gets the separator for name and value.
    /// </summary>
    public const string Separator = "=";

    /// <inheritdoc />
    public int Length { get; }

    /// <summary>
    /// Gets the name of this attribute.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value of this attribute.
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Gets the type of the query.
    /// </summary>
    public MikrotikQueryType Type { get; }
    
    /// <summary>
    /// Gets the operation to be performed as part of the query evaluation.
    /// </summary>
    public MikrotikQueryOperation? Operation { get; }

    private readonly string _computed;

    static MikrotikQueryWord()
    {
        _typeDictionary = typeof(MikrotikQueryType)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(x => new { v = (MikrotikQueryType)x.GetValue(null), a = x.GetCustomAttribute<EnumMemberAttribute>() })
            .Where(x => x.a is not null)
            .ToDictionary(x => x.v, x => x.a.Value);
        
        _operationDictionary = typeof(MikrotikQueryOperation)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(x => new { v = (MikrotikQueryOperation)x.GetValue(null), a = x.GetCustomAttribute<EnumMemberAttribute>() })
            .Where(x => x.a is not null)
            .ToDictionary(x => x.v, x => MakeTransformer(x.a.Value));
    }

    private MikrotikQueryWord(string name, string value, MikrotikQueryType type, MikrotikQueryOperation? operation, string computed)
    {
        this.Name = name;
        this.Value = value;
        this.Type = type;
        this.Operation = operation;
        this._computed = computed;
        this.Length = this._computed.ComputeEncodedLength();
    }

    /// <inheritdoc />
    public bool TryEncode(IMemoryBuffer<byte> destination)
        => this._computed.TryEncodeTo(destination);

    /// <summary>
    /// Creates a new query that checks whether the object has a given property.
    /// </summary>
    /// <param name="name">Name of the property to check for.</param>
    /// <returns>Constructed query word.</returns>
    public static MikrotikQueryWord HasProperty(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Property name cannot be null.");

        var type = MikrotikQueryType.HasProperty;
        var computed = CreateComputed(name, null, type, null);
        return new(name, null, type, default, computed);
    }

    /// <summary>
    /// Creates a new query that checks whether the object lacks a given property.
    /// </summary>
    /// <param name="name">Name of the property to check for.</param>
    /// <returns>Constructed query word.</returns>
    public static MikrotikQueryWord LacksProperty(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Property name cannot be null.");

        var type = MikrotikQueryType.LacksProperty;
        var computed = CreateComputed(name, null, type, null);
        return new(name, null, type, default, computed);
    }

    /// <summary>
    /// Creates a new query that checks whether the object has a property with given value.
    /// </summary>
    /// <param name="name">Name of the property to check.</param>
    /// <param name="value">Value to compare against.</param>
    /// <returns>Constructed query word.</returns>
    public static MikrotikQueryWord PropertyEquals(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Property name cannot be null.");
        
        if (string.IsNullOrWhiteSpace(value))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Value cannot be null.");

        var type = MikrotikQueryType.Equals;
        var computed = CreateComputed(name, value, type, null);
        return new(name, value, type, default, computed);
    }

    /// <summary>
    /// Creates a new query that checks whether the object has a property with a value greater than given value.
    /// </summary>
    /// <param name="name">Name of the property to check.</param>
    /// <param name="value">Value to compare against.</param>
    /// <returns>Constructed query word.</returns>
    public static MikrotikQueryWord PropertyGreaterThan(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Property name cannot be null.");
        
        if (string.IsNullOrWhiteSpace(value))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Value cannot be null.");

        var type = MikrotikQueryType.GreaterThan;
        var computed = CreateComputed(name, value, type, null);
        return new(name, value, type, default, computed);
    }

    /// <summary>
    /// Creates a new query that checks whether the object has a property with a value less than given value.
    /// </summary>
    /// <param name="name">Name of the property to check.</param>
    /// <param name="value">Value to compare against.</param>
    /// <returns>Constructed query word.</returns>
    public static MikrotikQueryWord PropertyLessThan(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Property name cannot be null.");
        
        if (string.IsNullOrWhiteSpace(value))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Value cannot be null.");

        var type = MikrotikQueryType.LessThan;
        var computed = CreateComputed(name, value, type, null);
        return new(name, value, type, default, computed);
    }

    /// <summary>
    /// Creates a new query word that performs an operation on the query result stack.
    /// </summary>
    /// <param name="operation">Operation to perform on the query result stack.</param>
    /// <param name="value">Value (index) as argument. Required only for some operations.</param>
    /// <returns>Constructed query word.</returns>
    public static MikrotikQueryWord StackOperation(MikrotikQueryOperation operation, string value = default)
    {
        if (operation == MikrotikQueryOperation.Unknown)
            MikrotikThrowHelper.Throw_Argument(nameof(operation), "Invalid operation specified.");
        
        if (operation is MikrotikQueryOperation.Replace or MikrotikQueryOperation.Copy or MikrotikQueryOperation.CopyTop && string.IsNullOrWhiteSpace(value))
            MikrotikThrowHelper.Throw_Argument(nameof(value), "Value is required for given operations.");
        
        var type = MikrotikQueryType.Operation;
        var computed = CreateComputed(null, value, type, operation);
        return new(null, value, type, operation, computed);
    }

    private static Func<String, String> MakeTransformer(string v)
    {
        if (v == ValuePlaceholder)
            return static x => x;

        var index = v.IndexOf(ValuePlaceholder, StringComparison.InvariantCultureIgnoreCase);
        if (index == -1)
            return _ => v;

        var pre = v[..index];
        var post = v[(index + ValuePlaceholder.Length)..];
        return x => pre + x + post;
    }

    private static string CreateComputed(string name, string value, MikrotikQueryType type, MikrotikQueryOperation? operation)
    {
        var sb = new StringBuilder();
        sb.Append(Prefix);
        
        var typePrefix = _typeDictionary[type];
        sb.Append(typePrefix);

        if (type != MikrotikQueryType.Operation)
        {
            sb.Append(name);

            if (value is not null)
            {
                sb.Append(Separator);
                sb.Append(value);
            }
        }
        else
        {
            var op = operation.Value;
            var opTransformer = _operationDictionary[op];
            sb.Append(opTransformer(value));
        }

        return sb.ToString();
    }
}