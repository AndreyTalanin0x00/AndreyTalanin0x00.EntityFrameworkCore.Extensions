using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

// Disable the IDE0001 (Simplify name) notification to preserve explicit object types.
#pragma warning disable IDE0001

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace AndreyTalanin0x00.EntityFrameworkCore.Transactions;

/// <summary>
/// Provides a set of extension methods for the <see cref="DatabaseFacade" /> class.
/// </summary>
public static class DatabaseFacadeExtensions
{
    /// <summary>
    /// Creates a handle that creates or provides access to an existing <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction for a given <see cref="DbContext" /> context.
    /// </summary>
    /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> object of the given <see cref="DbContext" /> context.</param>
    /// <param name="contextTransactionFactory">A factory that creates a new <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction is one does not exists.</param>
    /// <returns>The <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction handle.</returns>
    public static DbContextTransactionHandle GetDbContextTransactionHandle(this DatabaseFacade databaseFacade, DbContextTransactionFactory contextTransactionFactory)
    {
        DbContextTransactionHandle contextTransactionHandle =
            CreateDbContextTransactionHandle<DbContextTransactionHandle>(databaseFacade, contextTransactionFactory, CreateDbContextTransactionHandle);

        return contextTransactionHandle;
    }

    /// <summary>
    /// Asynchronously creates a handle that creates or provides access to an existing <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction for a given <see cref="DbContext" /> context.
    /// </summary>
    /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> object of the given <see cref="DbContext" /> context.</param>
    /// <param name="contextTransactionAsyncFactory">A factory that asynchronously creates a new <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction is one does not exists.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task's result will be the <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction handle.
    /// </returns>
    public static async Task<DbContextTransactionHandle> GetDbContextTransactionHandleAsync(this DatabaseFacade databaseFacade, DbContextTransactionAsyncFactory contextTransactionAsyncFactory, CancellationToken cancellationToken = default)
    {
        DbContextTransactionHandle contextTransactionHandle =
            await CreateDbContextTransactionHandleAsync<DbContextTransactionHandle>(databaseFacade, contextTransactionAsyncFactory, CreateDbContextTransactionHandle, cancellationToken);

        return contextTransactionHandle;
    }

    /// <summary>
    /// Creates a handle that creates or uses an existing <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction for a given <see cref="DbContext" /> context and provides access to the underlying <see cref="DbTransaction" /> ADO.NET transaction.
    /// </summary>
    /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> object of the given <see cref="DbContext" /> context.</param>
    /// <param name="contextTransactionFactory">A factory that creates a new <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction is one does not exists.</param>
    /// <returns>The <see cref="DbTransaction" /> ADO.NET transaction handle.</returns>
    public static DbTransactionHandle GetDbTransactionHandle(this DatabaseFacade databaseFacade, DbContextTransactionFactory contextTransactionFactory)
    {
        DbTransactionHandle transactionHandle =
            CreateDbContextTransactionHandle<DbTransactionHandle>(databaseFacade, contextTransactionFactory, CreateDbTransactionHandle);

        return transactionHandle;
    }

    /// <summary>
    /// Asynchronously creates a handle that creates or uses an existing <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction for a given <see cref="DbContext" /> context and provides access to the underlying <see cref="DbTransaction" /> ADO.NET transaction.
    /// </summary>
    /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> object of the given <see cref="DbContext" /> context.</param>
    /// <param name="contextTransactionAsyncFactory">A factory that asynchronously creates a new <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction is one does not exists.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task's result will be the <see cref="DbTransaction" /> ADO.NET transaction handle.
    /// </returns>
    public static async Task<DbTransactionHandle> GetDbTransactionHandleAsync(this DatabaseFacade databaseFacade, DbContextTransactionAsyncFactory contextTransactionAsyncFactory, CancellationToken cancellationToken = default)
    {
        DbTransactionHandle transactionHandle =
            await CreateDbContextTransactionHandleAsync<DbTransactionHandle>(databaseFacade, contextTransactionAsyncFactory, CreateDbTransactionHandle, cancellationToken);

        return transactionHandle;
    }

    private static TDbContextTransactionHandle CreateDbContextTransactionHandle<TDbContextTransactionHandle>(this DatabaseFacade databaseFacade, DbContextTransactionFactory contextTransactionFactory, DbContextTransactionHandleFactory<TDbContextTransactionHandle> contextTransactionHandleFactory)
        where TDbContextTransactionHandle : DbContextTransactionHandle
    {
        (IDbContextTransaction? contextTransaction, bool contextTransactionOwnedByHandle) = (databaseFacade.CurrentTransaction, false);

        try
        {
            if (contextTransaction is null)
                (contextTransaction, contextTransactionOwnedByHandle) = (contextTransactionFactory(), true);

            TDbContextTransactionHandle contextTransactionHandle = contextTransactionHandleFactory(contextTransaction, contextTransactionOwnedByHandle);

            return contextTransactionHandle;
        }
        catch
        {
            if (contextTransactionOwnedByHandle && contextTransaction is not null)
                contextTransaction.Dispose();

            throw;
        }
    }

    private static async Task<TDbContextTransactionHandle> CreateDbContextTransactionHandleAsync<TDbContextTransactionHandle>(this DatabaseFacade databaseFacade, DbContextTransactionAsyncFactory contextTransactionAsyncFactory, DbContextTransactionHandleFactory<TDbContextTransactionHandle> contextTransactionHandleFactory, CancellationToken cancellationToken = default)
        where TDbContextTransactionHandle : DbContextTransactionHandle
    {
        (IDbContextTransaction? contextTransaction, bool contextTransactionOwnedByHandle) = (databaseFacade.CurrentTransaction, false);

        try
        {
            if (contextTransaction is null)
                (contextTransaction, contextTransactionOwnedByHandle) = (await contextTransactionAsyncFactory(cancellationToken), true);

            TDbContextTransactionHandle contextTransactionHandle = contextTransactionHandleFactory(contextTransaction, contextTransactionOwnedByHandle);

            return contextTransactionHandle;
        }
        catch
        {
            if (contextTransactionOwnedByHandle && contextTransaction is not null)
                await contextTransaction.DisposeAsync();

            throw;
        }
    }

    private static DbContextTransactionHandle CreateDbContextTransactionHandle(IDbContextTransaction contextTransaction, bool contextTransactionOwnedByHandle)
    {
        return new(contextTransaction, contextTransactionOwnedByHandle);
    }

    private static DbTransactionHandle CreateDbTransactionHandle(IDbContextTransaction contextTransaction, bool contextTransactionOwnedByHandle)
    {
        return new(contextTransaction, contextTransactionOwnedByHandle);
    }

    private delegate TDbContextTransactionHandle DbContextTransactionHandleFactory<TDbContextTransactionHandle>(IDbContextTransaction contextTransaction, bool contextTransactionOwnedByHandle)
        where TDbContextTransactionHandle : DbContextTransactionHandle;
}
