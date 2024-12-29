using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a <see cref="IQueryProvider"/> that works asynchronously.
/// </summary>
public interface IAsyncQueryProvider : IQueryProvider
{
    /// <summary>
    /// Asynchronously executes this query against the data source.
    /// </summary>
    /// <param name="expression">Expression to evaluate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TResult">Type of result to return.</typeparam>
    /// <returns>The evaluated result.</returns>
    ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default);
}