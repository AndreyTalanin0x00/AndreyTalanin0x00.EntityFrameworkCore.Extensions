using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AndreyTalanin0x00.EntityFrameworkCore.Connections;

/// <summary>
/// Provides a set of static methods implementing async support for the <see cref="DbConnectionHandle" /> class.
/// </summary>
public static class DbConnectionHandleAsyncHelpers
{
    /// <summary>
    /// Asynchronously creates a new <see cref="DbConnectionHandle" /> connection handle using the specified <see cref="DbConnection" /> connection.
    /// </summary>
    /// <param name="connection">The <see cref="DbConnection" /> connection.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task's result will be the <see cref="DbConnectionHandle" /> connection handle created.
    /// </returns>
    public static async Task<DbConnectionHandle> CreateDbConnectionHandleAsync(DbConnection connection, CancellationToken cancellationToken = default)
    {
        bool connectionOwnedByHandle = false;

        try
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
                connectionOwnedByHandle = true;
            }

            DbConnectionHandle connectionHandle = new(connection, connectionOwnedByHandle);

            return connectionHandle;
        }
        catch
        {
            if (connectionOwnedByHandle)
                await connection.CloseAsync();

            throw;
        }
    }
}
