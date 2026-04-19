using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;

namespace AndreyTalanin0x00.EntityFrameworkCore.Transactions;

/// <summary>
/// Encapsulates a method that asynchronously creates a new <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.
/// </summary>
/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
/// <returns>
/// A task representing the asynchronous operation.
/// The task's result will be the <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction created.
/// </returns>
public delegate Task<IDbContextTransaction> DbContextTransactionAsyncFactory(CancellationToken cancellationToken);
