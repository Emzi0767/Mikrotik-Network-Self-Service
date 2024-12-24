using System.Runtime.Serialization;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Specifies the type of query operation to perform on the filter stack.
/// </summary>
public enum MikrotikQueryOperation
{
    /// <summary>
    /// Specifies unknown operation.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Specifies an operation that copies the value at a specified index. 
    /// </summary>
    [EnumMember(Value = $"{MikrotikQueryWord.ValuePlaceholder}+")]
    Copy,
    
    /// <summary>
    /// Specifies an operation that replaces all values with the value at a specified index.
    /// </summary>
    [EnumMember(Value = MikrotikQueryWord.ValuePlaceholder)]
    Replace,
    
    /// <summary>
    /// Specifies an operation that negates the top value.
    /// </summary>
    [EnumMember(Value = "!")]
    Not,
    
    /// <summary>
    /// Specifies an operation that pops top 2 values, performs logical AND, and pushes the result.
    /// </summary>
    [EnumMember(Value = "&")]
    And,
    
    /// <summary>
    /// Specifies an operation that pops top 2 values, performs logical OR, and pushes the result.
    /// </summary>
    [EnumMember(Value = "|")]
    Or,
    
    /// <summary>
    /// Specifies an operation that pushes a copy of the top value.
    /// </summary>
    [EnumMember(Value = ".")]
    CopyTop,
}