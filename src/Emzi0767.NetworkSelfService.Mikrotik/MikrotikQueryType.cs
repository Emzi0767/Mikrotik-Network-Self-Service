using System.Runtime.Serialization;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Specifies the type of query to Mikrotik API.
/// </summary>
public enum MikrotikQueryType
{
    /// <summary>
    /// Specifies an unknown query.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property.
    /// </summary>
    [EnumMember(Value = "")]
    HasProperty,
    
    /// <summary>
    /// Specifies that the query filters for objects which have no property of a given name.
    /// </summary>
    [EnumMember(Value = "-")]
    LacksProperty,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property's value equal to a given value.
    /// </summary>
    [EnumMember(Value = "")]
    Equals,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property's value being less than a given value.
    /// </summary>
    [EnumMember(Value = "<")]
    LessThan,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property's value being greater than a given value.
    /// </summary>
    [EnumMember(Value = ">")]
    GreaterThan,
    
    /// <summary>
    /// Specifies an operation on the filter result stack.
    /// </summary>
    [EnumMember(Value = "#")]
    Operation,
}