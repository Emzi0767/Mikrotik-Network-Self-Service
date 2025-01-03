using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Provides extension methods for <see cref="IAsyncQueryable{T}"/>.
/// </summary>
public static class AsyncQueryableExtensions
{
    /// <summary>
    /// Casts this <see cref="IQueryable{T}"/> to <see cref="IAsyncQueryable{T}"/>.
    /// </summary>
    /// <param name="source">Queryable to cast.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Queryable cast as async queryable.</returns>
    public static IAsyncQueryable<T> AsAsyncQueryable<T>(this IQueryable<T> source)
        => source as IAsyncQueryable<T>;

    /// <summary>
    /// Asynchronously flattens this async queryable to a list.
    /// </summary>
    /// <param name="source">Queryable to flatten.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Flattened list containing the results of this queryable's execution.</returns>
    public static async Task<List<T>> ToListAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default)
    {
        var list = new List<T>();
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            list.Add(item);

        return list;
    }

    /// <summary>
    /// Asynchronously flattens this async queryable to an array.
    /// </summary>
    /// <param name="source">Queryable to flatten.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Flattened array containing the results of this queryable's execution.</returns>
    public static async Task<T[]> ToArrayAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default)
    {
        var list = await source.ToListAsync(cancellationToken);
        return list.ToArray();
    }

    /// <summary>
    /// Asynchronously flattens this async queryable to a dictionary.
    /// </summary>
    /// <param name="source">Queryable to flatten.</param>
    /// <param name="keySelector">Function to convert results into dictionary keys.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the qyeryable.</typeparam>
    /// <typeparam name="TKey">Type of key in the dictionary.</typeparam>
    /// <returns>Flattened dictionary containing the results of this queryable's execution.</returns>
    public static Task<IDictionary<TKey, T>> ToDictionaryAsync<T, TKey>(
        this IAsyncQueryable<T> source,
        Func<T, TKey> keySelector,
        CancellationToken cancellationToken = default
    )
        => source.ToDictionaryAsync(keySelector, static x => x, null, cancellationToken);
    
    /// <summary>
    /// Asynchronously flattens this async queryable to a dictionary.
    /// </summary>
    /// <param name="source">Queryable to flatten.</param>
    /// <param name="keySelector">Function to convert results into dictionary keys.</param>
    /// <param name="comparer">An implementation of a comparer for dictionary's keys.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the qyeryable.</typeparam>
    /// <typeparam name="TKey">Type of key in the dictionary.</typeparam>
    /// <returns>Flattened dictionary containing the results of this queryable's execution.</returns>
    public static Task<IDictionary<TKey, T>> ToDictionaryAsync<T, TKey>(
        this IAsyncQueryable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey> comparer,
        CancellationToken cancellationToken = default
    )
        => source.ToDictionaryAsync(keySelector, static x => x, comparer, cancellationToken);
    
    /// <summary>
    /// Asynchronously flattens this async queryable to a dictionary.
    /// </summary>
    /// <param name="source">Queryable to flatten.</param>
    /// <param name="keySelector">Function to convert results into dictionary keys.</param>
    /// <param name="valueSelector">Function to convert results into dictionary values.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the qyeryable.</typeparam>
    /// <typeparam name="TKey">Type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of value in the dictionary.</typeparam>
    /// <returns>Flattened dictionary containing the results of this queryable's execution.</returns>
    public static Task<IDictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(
        this IAsyncQueryable<T> source,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector,
        CancellationToken cancellationToken = default
    )
        => source.ToDictionaryAsync(keySelector, valueSelector, null, cancellationToken);

    /// <summary>
    /// Asynchronously flattens this async queryable to a dictionary.
    /// </summary>
    /// <param name="source">Queryable to flatten.</param>
    /// <param name="keySelector">Function to convert results into dictionary keys.</param>
    /// <param name="valueSelector">Function to convert results into dictionary values.</param>
    /// <param name="comparer">An implementation of a comparer for dictionary's keys.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the qyeryable.</typeparam>
    /// <typeparam name="TKey">Type of key in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of value in the dictionary.</typeparam>
    /// <returns>Flattened dictionary containing the results of this queryable's execution.</returns>
    public static async Task<IDictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(
        this IAsyncQueryable<T> source,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector,
        IEqualityComparer<TKey> comparer,
        CancellationToken cancellationToken = default
    )
    {
        var dict = new Dictionary<TKey, TValue>(comparer);
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            dict.Add(keySelector(item), valueSelector(item));
        
        return dict;
    }
}