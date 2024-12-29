using System.Collections.Generic;
using System.Linq;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a <see cref="IQueryable{T}"/> that can be executed asynchronously.
/// </summary>
/// <typeparam name="T">Type of item in the data source.</typeparam>
public interface IAsyncQueryable<out T> : IQueryable<T>, IAsyncEnumerable<T>
{ }