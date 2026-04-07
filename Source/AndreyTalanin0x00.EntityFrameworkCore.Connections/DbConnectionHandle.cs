using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AndreyTalanin0x00.EntityFrameworkCore.Connections;

/// <summary>
/// Represents a handle for an external <see cref="DbConnection" /> connection that manages its state.
/// <para>When a handle is created, it opens the provided connection if necessary, and when the same handle is disposed, the connection is only closed if necessary.</para>
/// </summary>
/// <remarks>Consider utilizing the <c>using</c> or <c>await using</c> statement to avoid connection lifetime mismanagement.</remarks>
public class DbConnectionHandle : IDisposable, IAsyncDisposable
{
    private readonly DbConnection m_connection;
    private readonly bool m_connectionOwnedByHandle;
    private bool m_disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionHandle" /> type using the specified <see cref="DbConnection" /> connection.
    /// </summary>
    /// <param name="connection">The <see cref="DbConnection" /> connection.</param>
    public DbConnectionHandle(DbConnection connection)
        : this(connection, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionHandle" /> type using the specified <see cref="DbConnection" /> connection and overridden initial state.
    /// </summary>
    /// <param name="connection">The <see cref="DbConnection" /> connection.</param>
    /// <param name="connectionOwnedByHandle">Specifies whether the connection is owned by the handle (and therefore should be closed when the handle gets disposed).</param>
    public DbConnectionHandle(DbConnection connection, bool connectionOwnedByHandle)
    {
        m_connection = connection;
        m_connectionOwnedByHandle = connectionOwnedByHandle;
        m_disposed = false;

        if (connection.State == ConnectionState.Closed)
        {
            m_connection.Open();
            m_connectionOwnedByHandle = true;
        }
    }

    /// <summary>
    /// Gets the <see cref="DbConnection" /> connection managed by the current handle.
    /// </summary>
    public DbConnection Connection
    {
        get
        {
            DbConnection connection = m_disposed
                ? throw new ObjectDisposedException("The connection handle has already been disposed.", (Exception?)null)
                : m_connection;

            return connection;
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
            if (dispose && m_connectionOwnedByHandle)
                m_connection.Close();

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
            if (dispose && m_connectionOwnedByHandle)
                await m_connection.CloseAsync();

            m_disposed = true;
        }
    }
}
