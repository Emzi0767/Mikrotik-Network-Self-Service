// This file is part of Network Self-Service Project.
// Copyright © 2024-2025 Mateusz Brawański <Emzi0767>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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
    public static async Task<List<T>> ToListAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
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
    public static async Task<T[]> ToArrayAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
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
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
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
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
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
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
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
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
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

    /// <summary>
    /// Asynchronously retrieves the first item from the queryable. If there are none, the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static Task<T> FirstAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.FirstAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the first item that matches the predicate from the queryable. If there are none, the
    /// method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="predicate">Predicate to match the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static async Task<T> FirstAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            if (predicate(item))
                return item;

        MikrotikThrowHelper.Throw_InvalidOperation("Source does not contain matching elements.");
        return default;
    }

    /// <summary>
    /// Asynchronously retrieves the first item from the queryable. If no item is present, a default value is returned.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static Task<T> FirstOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.FirstOrDefaultAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the first item that matches the predicate from the queryable. If no matching item is
    /// present, a default value is returned.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="predicate">Predicate to match the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static async Task<T> FirstOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            if (predicate(item))
                return item;

        return default;
    }

    /// <summary>
    /// Asynchronously retrieves the single item from the queryable. If the queryable does not contain exactly one item,
    /// the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static Task<T> SingleAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.SingleAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the single item that matches the predicate from the queryable. If the queryable does
    /// not contain exactly one item, the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="predicate">Predicate to match the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static async Task<T> SingleAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var hasItem = false;
        var single = default(T);
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (predicate(item))
            {
                if (hasItem)
                    MikrotikThrowHelper.Throw_InvalidOperation("Source contains more than one matching element.");

                hasItem = true;
                single = item;
            }
        }

        if (!hasItem)
            MikrotikThrowHelper.Throw_InvalidOperation("Source does not contain matching elements.");

        return single;
    }

    /// <summary>
    /// Asynchronously retrieves the single item from the queryable. If the queryable does not contain a matching item,
    /// a default value is returned. If more than one item matches, the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static Task<T> SingleOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.SingleOrDefaultAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the single item that matches the predicate from the queryable. If the queryable does
    /// not contain a matching item, a default value is returned. If more than one item matches, the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="predicate">Predicate to match the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static async Task<T> SingleOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var hasItem = false;
        var single = default(T);
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (predicate(item))
            {
                if (hasItem)
                    MikrotikThrowHelper.Throw_InvalidOperation("Source contains more than one matching element.");

                hasItem = true;
                single = item;
            }
        }

        return single;
    }

    /// <summary>
    /// Asynchronously retrieves the last item from the queryable. If the queryable does not contain a matching item,
    /// the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static Task<T> LastAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.LastAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the last item that matches the predicate from the queryable. If the queryable does not
    /// contain a matching item, the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="predicate">Predicate to match the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static async Task<T> LastAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var hasItem = false;
        var last = default(T);
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (predicate(item))
            {
                last = item;
                hasItem = true;
            }
        }

        if (!hasItem)
            MikrotikThrowHelper.Throw_InvalidOperation("Source does not contain matching elements.");

        return last;
    }

    /// <summary>
    /// Asynchronously retrieves the last item from the queryable. If the queryable does not contain a matching item,
    /// the method returns a default value.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static Task<T> LastOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.LastOrDefaultAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the last item that matches the predicate from the queryable. If the queryable does not
    /// contain a matching item, the method returns a default value.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="predicate">Predicate to match the item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static async Task<T> LastOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var last = default(T);
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            last = item;

        return last;
    }

    /// <summary>
    /// Asynchronously retrieves the item at the specified index. If the index is out of bounds, the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="index">Index to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static Task<T> ElementAtAsync<T>(
        this IAsyncQueryable<T> source,
        int index,
        CancellationToken cancellationToken = default
    )
        => source.ElementAtAsync(new Index(index), cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the item at the specified index. If the index is out of bounds, the method fails.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="index">Index to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item.</returns>
    public static async Task<T> ElementAtAsync<T>(
        this IAsyncQueryable<T> source,
        Index index,
        CancellationToken cancellationToken = default
    )
    {
        var (hasItem, item) = await (index.IsFromEnd
            ? source.ElementAtStartAsync(index, cancellationToken)
            : source.ElementAtEndAsync(index, cancellationToken));

        if (!hasItem)
            MikrotikThrowHelper.Throw_OutOfRange(nameof(index), "Specified index is out of bounds.");

        return item;
    }

    /// <summary>
    /// Asynchronously retrieves the item at the specified index. If the index is out of bounds, the returns a default
    /// value.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="index">Index to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static Task<T> ElementAtOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        int index,
        CancellationToken cancellationToken = default
    )
        => source.ElementAtOrDefaultAsync(new Index(index), cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the item at the specified index. If the index is out of bounds, the returns a default
    /// value.
    /// </summary>
    /// <param name="source">Queryable to retrieve the item from.</param>
    /// <param name="index">Index to retrieve the item from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The retrieved item or default value.</returns>
    public static async Task<T> ElementAtOrDefaultAsync<T>(
        this IAsyncQueryable<T> source,
        Index index,
        CancellationToken cancellationToken = default
    )
    {
        var (_, item) = await (index.IsFromEnd
            ? source.ElementAtStartAsync(index, cancellationToken)
            : source.ElementAtEndAsync(index, cancellationToken));

        return item;
    }

    private static async Task<(bool hasItem, T item)> ElementAtStartAsync<T>(
        this IAsyncQueryable<T> source,
        Index index,
        CancellationToken cancellationToken = default
    )
    {
        var element = default(T);
        var idx = index.Value;
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            if (idx-- == 0)
                element = item;

        return (idx == 0, element);
    }

    private static async Task<(bool hasItem, T item)> ElementAtEndAsync<T>(
        this IAsyncQueryable<T> source,
        Index index,
        CancellationToken cancellationToken = default
    )
    {
        var items = await source.ToListAsync(cancellationToken);
        var idx = index.GetOffset(items.Count);
        var hasItem = idx >= 0;
        return (hasItem, hasItem ? items[idx] : default);
    }

    /// <summary>
    /// Asynchronously checks whether there is any item in the queryable.
    /// </summary>
    /// <param name="source">Queryable to check.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Whether the queryable contains any items.</returns>
    public static Task<bool> AnyAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.AnyAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously checks whether there is any item that matches the predicate in the queryable.
    /// </summary>
    /// <param name="source">Queryable to check.</param>
    /// <param name="predicate">Predicate to match items.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Whether the queryable contains any items that match the predicate.</returns>
    public static async Task<bool> AnyAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            if (predicate(item))
                return true;

        return false;
    }

    /// <summary>
    /// Asynchronously checks whether all items in the queryable satisfy a condition.
    /// </summary>
    /// <param name="source">Queryable to check.</param>
    /// <param name="predicate">Predicate to match items.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Whether all items in the queryable satisfy the predicate.</returns>
    public static async Task<bool> AllAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            if (!predicate(item))
                return false;

        return true;
    }

    /// <summary>
    /// Asynchronously counts all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to count items in.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The total number of items in the queryable.</returns>
    public static Task<int> CountAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.CountAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously counts all items that satisfy a predicate in the queryable.
    /// </summary>
    /// <param name="source">Queryable to count items in.</param>
    /// <param name="predicate">Predicate to match items.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The total number of items that satisfy the predicate in the queryable.</returns>
    public static async Task<int> CountAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var count = 0;
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            if (predicate(item))
                ++count;

        return count;
    }

    /// <summary>
    /// Asynchronously counts all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to count items in.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The total number of items in the queryable.</returns>
    public static Task<long> LongCountAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        => source.LongCountAsync(static x => true, cancellationToken);

    /// <summary>
    /// Asynchronously counts all items that satisfy a predicate in the queryable.
    /// </summary>
    /// <param name="source">Queryable to count items in.</param>
    /// <param name="predicate">Predicate to match items.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>The total number of items that satisfy the predicate in the queryable.</returns>
    public static async Task<long> LongCountAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var count = 0L;
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            if (predicate(item))
                ++count;

        return count;
    }

    /// <summary>
    /// Asynchronously computes the sum of all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to sum items in.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Computed sum of items in the queryable.</returns>
    public static Task<T> SumAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        where T : IAdditionOperators<T, T, T>
        => source.SumAsync(static x => x, cancellationToken);

    /// <summary>
    /// Asynchronously computes the sum of all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to sum items in.</param>
    /// <param name="selector">Function that transforms the input items into an arithmetic type.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TItem">Type of item in the queryable.</typeparam>
    /// <typeparam name="TNum">Type of result.</typeparam>
    /// <returns>Computed sum of items in the queryable.</returns>
    public static async Task<TNum> SumAsync<TItem, TNum>(
        this IAsyncQueryable<TItem> source,
        Func<TItem, TNum> selector,
        CancellationToken cancellationToken = default
    )
        where TNum : IAdditionOperators<TNum, TNum, TNum>
    {
        var sum = default(TNum);
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            sum += selector(item);

        return sum;
    }

    /// <summary>
    /// Asynchronously computes the average of all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to average items in.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Computed average of items in the queryable.</returns>
    public static Task<T> AverageAsync<T>(
        this IAsyncQueryable<T> source,
        CancellationToken cancellationToken = default
    )
        where T : IAdditionOperators<T, T, T>, IDivisionOperators<T, T, T>, IConvertible
        => source.AverageAsync(static x => x, cancellationToken);

    /// <summary>
    /// Asynchronously computes the average of all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to average items in.</param>
    /// <param name="selector">Function that transforms the input items into an arithmetic type.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TItem">Type of item in the queryable.</typeparam>
    /// <typeparam name="TNum">Type of result.</typeparam>
    /// <returns>Computed average of items in the queryable.</returns>
    public static async Task<TNum> AverageAsync<TItem, TNum>(
        this IAsyncQueryable<TItem> source,
        Func<TItem, TNum> selector,
        CancellationToken cancellationToken = default
    )
        where TNum : IAdditionOperators<TNum, TNum, TNum>, IDivisionOperators<TNum, TNum, TNum>, IConvertible
    {
        var sum = default(TNum);
        var count = 0L;
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            sum += selector(item);
            ++count;
        }

        return sum / (TNum)(count as IConvertible).ToType(typeof(TNum), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Aggregates all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to aggregate items in.</param>
    /// <param name="accumulator">Function that accumulates a result based on item and state.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">Type of item in the queryable.</typeparam>
    /// <returns>Aggregated value of the items in the queryable.</returns>
    public static Task<T> AggregateAsync<T>(
        this IAsyncQueryable<T> source,
        Func<T, T, T> accumulator,
        CancellationToken cancellationToken = default
    )
        => source.AggregateAsync(default, accumulator, static x => x, cancellationToken);

    /// <summary>
    /// Aggregates all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to aggregate items in.</param>
    /// <param name="seed">Initial value for the operation.</param>
    /// <param name="accumulator">Function that accumulates a result based on item and state.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TItem">Type of item in the queryable.</typeparam>
    /// <typeparam name="TAccumulate">Type of accumulated value.</typeparam>
    /// <returns>Aggregated value of the items in the queryable.</returns>
    public static Task<TAccumulate> AggregateAsync<TItem, TAccumulate>(
        this IAsyncQueryable<TItem> source,
        TAccumulate seed,
        Func<TAccumulate, TItem, TAccumulate> accumulator,
        CancellationToken cancellationToken = default
    )
        => source.AggregateAsync(seed, accumulator, static x => x, cancellationToken);

    /// <summary>
    /// Aggregates all items in the queryable.
    /// </summary>
    /// <param name="source">Queryable to aggregate items in.</param>
    /// <param name="seed">Initial value for the operation.</param>
    /// <param name="accumulator">Function that accumulates a result based on item and state.</param>
    /// <param name="selector">Function that transforms the accumulated value into the result type.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TItem">Type of item in the queryable.</typeparam>
    /// <typeparam name="TAccumulate">Type of accumulated value.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    /// <returns>Aggregated value of the items in the queryable.</returns>
    public static async Task<TResult> AggregateAsync<TItem, TAccumulate, TResult>(
        this IAsyncQueryable<TItem> source,
        TAccumulate seed,
        Func<TAccumulate, TItem, TAccumulate> accumulator,
        Func<TAccumulate, TResult> selector,
        CancellationToken cancellationToken = default
    )
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            seed = accumulator(seed, item);

        return selector(seed);
    }
}
