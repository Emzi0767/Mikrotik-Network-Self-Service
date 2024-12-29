namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a Mikrotik API entity that can be modified.
/// </summary>
public interface IMikrotikModifiableEntity<T>
    where T : class, IMikrotikEntity
{
    /// <summary>
    /// Begins modifying the entity.
    /// </summary>
    /// <returns>A modifier which allows for further changes to be made, or queued changes to be executed.</returns>
    IMikrotikEntityModifier<T> Modify();
}