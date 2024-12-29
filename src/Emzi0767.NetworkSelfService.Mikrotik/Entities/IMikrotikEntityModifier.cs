using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a set of modifications to make to an entity.
/// </summary>
public interface IMikrotikEntityModifier<T>
    where T : class, IMikrotikEntity
{
    /// <summary>
    /// Begins modifying the entity.
    /// </summary>
    /// <param name="propertySelector">Selector of the property to modify.</param>
    /// <param name="newValue">New value for the property.</param>
    /// <typeparam name="TProp">Type of the selected property.</typeparam>
    /// <returns>A modifier which allows for further changes to be made, or queued changes to be executed.</returns>
    IMikrotikEntityModifier<T> Set<TProp>(Expression<Func<T, TProp>> propertySelector, TProp newValue);
    
    /// <summary>
    /// Asynchronously executes scheduled changes to the entity. 
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}