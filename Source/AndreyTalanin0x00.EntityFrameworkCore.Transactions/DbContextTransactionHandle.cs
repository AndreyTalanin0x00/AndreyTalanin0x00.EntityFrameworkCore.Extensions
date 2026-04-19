using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;

namespace AndreyTalanin0x00.EntityFrameworkCore.Transactions;

/// <summary>
/// Represents a handle for an external <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction that manages its state.
/// <para>When a handle is created, it remembers whether it owns the EntityFrameworkCore context transaction, and when the same handle is disposed, the EntityFrameworkCore context transaction is only disposed if necessary.</para>
/// </summary>
/// <remarks>Consider utilizing the <c>using</c> or <c>await using</c> statement to avoid EntityFrameworkCore context transaction lifetime mismanagement.</remarks>
public class DbContextTransactionHandle : IDisposable, IAsyncDisposable
{
    private readonly IDbContextTransaction m_contextTransaction;
    private readonly bool m_contextTransactionOwnedByHandle;
    private bool m_disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextTransactionHandle" /> type using the specified <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.
    /// </summary>
    /// <param name="contextTransaction">The <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.</param>
    public DbContextTransactionHandle(IDbContextTransaction contextTransaction)
        : this(contextTransaction, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextTransactionHandle" /> type using the specified <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction and overridden initial state.
    /// </summary>
    /// <param name="contextTransaction">The <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.</param>
    /// <param name="contextTransactionOwnedByHandle">Specifies whether the EntityFrameworkCore context transaction is owned by the handle (and therefore should be disposed when the handle gets disposed).</param>
    public DbContextTransactionHandle(IDbContextTransaction contextTransaction, bool contextTransactionOwnedByHandle)
    {
        m_contextTransaction = contextTransaction;
        m_contextTransactionOwnedByHandle = contextTransactionOwnedByHandle;
        m_disposed = false;
    }

    /// <summary>
    /// Gets the <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction managed by the current handle.
    /// </summary>
    public IDbContextTransaction ContextTransaction
    {
        get
        {
            IDbContextTransaction contextTransaction = IsDisposed()
                ? throw new ObjectDisposedException("The EntityFrameworkCore context transaction handle has already been disposed.", (Exception?)null)
                : m_contextTransaction;

            return contextTransaction;
        }
    }

    /// <summary>
    /// Gets a boolean value indicating whether the <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction is owned by the current handle.
    /// </summary>
    public bool ContextTransactionOwnedByHandle
    {
        get
        {
            return m_contextTransactionOwnedByHandle;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(dispose: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(dispose: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="dispose">Specifies whether the <see cref="Dispose(bool)" /> method was called as part of controlled resource releasing and not garbage collection.</param>
    protected virtual void Dispose(bool dispose)
    {
        if (!m_disposed)
        {
            if (dispose && m_contextTransactionOwnedByHandle)
                m_contextTransaction.Dispose();

            m_disposed = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <param name="dispose">Specifies whether the <see cref="Dispose(bool)" /> method was called as part of controlled resource releasing and not garbage collection.</param>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    protected virtual async ValueTask DisposeAsync(bool dispose)
    {
        if (!m_disposed)
        {
            if (dispose && m_contextTransactionOwnedByHandle)
                await m_contextTransaction.DisposeAsync();

            m_disposed = true;
        }
    }

    /// <summary>
    /// Gets a boolean value indicating whether the current <see cref="DbContextTransactionHandle" /> instance is disposed.
    /// </summary>
    /// <returns>The boolean value indicating whether the current <see cref="DbContextTransactionHandle" /> instance is disposed.</returns>
    protected bool IsDisposed()
    {
        return m_disposed;
    }
}
