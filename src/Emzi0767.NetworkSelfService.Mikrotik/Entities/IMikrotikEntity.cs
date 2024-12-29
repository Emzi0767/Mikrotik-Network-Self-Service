namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a Mikrotik API entity.
/// </summary>
public interface IMikrotikEntity
{
    /// <summary>
    /// Gets the API client this entity is associated with.
    /// </summary>
    public MikrotikClient Client { get; }
}