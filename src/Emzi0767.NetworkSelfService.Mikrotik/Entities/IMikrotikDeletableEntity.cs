using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a Mikrotik API entity that can be deleted.
/// </summary>
public interface IMikrotikDeletableEntity : IMikrotikEntity
{
    /// <summary>
    /// Asynchronously requests the deletion of this entity.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteAsync(CancellationToken cancellationToken = default);
}