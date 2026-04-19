using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Storage;

namespace AndreyTalanin0x00.EntityFrameworkCore.Transactions;

/// <summary>
/// Represents a handle for an external <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction that manages its state and provides access to the underlying <see cref="DbTransaction" /> ADO.NET transaction.
/// <para>When a handle is created, it remembers whether it owns the EntityFrameworkCore context transaction, and when the same handle is disposed, the EntityFrameworkCore context transaction is only disposed if necessary.</para>
/// </summary>
/// <remarks>Consider utilizing the <c>using</c> or <c>await using</c> statement to avoid EntityFrameworkCore context transaction lifetime mismanagement.</remarks>
public class DbTransactionHandle : DbContextTransactionHandle
{
    private readonly Lazy<DbTransaction> m_transactionLazy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbTransactionHandle" /> type using the specified <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.
    /// </summary>
    /// <param name="contextTransaction">The <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.</param>
    public DbTransactionHandle(IDbContextTransaction contextTransaction)
        : this(contextTransaction, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbTransactionHandle" /> type using the specified <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction and overridden initial state.
    /// </summary>
    /// <param name="contextTransaction">The <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.</param>
    /// <param name="contextTransactionOwnedByHandle">Specifies whether the EntityFrameworkCore context transaction is owned by the handle (and therefore should be disposed when the handle gets disposed).</param>
    [SuppressMessage("Style", "IDE0200:Remove unnecessary lambda expression", Justification = "Personal preference.")]
    public DbTransactionHandle(IDbContextTransaction contextTransaction, bool contextTransactionOwnedByHandle)
        : base(contextTransaction, contextTransactionOwnedByHandle)
    {
        m_transactionLazy = new Lazy<DbTransaction>(() => contextTransaction.GetDbTransaction());
    }

    /// <summary>
    /// Gets the underlying <see cref="DbTransaction" /> ADO.NET transaction associated with the <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction managed by the current handle.
    /// </summary>
    public DbTransaction Transaction
    {
        get
        {
            DbTransaction transaction = IsDisposed()
                ? throw new ObjectDisposedException("The ADO.NET transaction handle has already been disposed.", (Exception?)null)
                : m_transactionLazy.Value;

            return transaction;
        }
    }
}
