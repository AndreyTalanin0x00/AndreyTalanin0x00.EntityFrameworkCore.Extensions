using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace AndreyTalanin0x00.EntityFrameworkCore.Connections;

/// <summary>
/// Provides a set of extension methods for the <see cref="DatabaseFacade" /> class.
/// </summary>
public static class DatabaseFacadeExtensions
{
    /// <summary>
    /// Creates a handle that manages the state of an underlying ADO.NET <see cref="DbConnection" /> connection for a given <see cref="DbContext" /> context.
    /// </summary>
    /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> object of the given <see cref="DbContext" /> context.</param>
    /// <returns>The <see cref="DbConnectionHandle" /> connection handle created.</returns>
    public static DbConnectionHandle GetDbConnectionHandle(this DatabaseFacade databaseFacade)
    {
        DbConnection connection = databaseFacade.GetDbConnection();

        DbConnectionHandle connectionHandle = new(connection);

        return connectionHandle;
    }

    /// <summary>
    /// Asynchronously creates a handle that manages the state of an underlying ADO.NET <see cref="DbConnection" /> connection for a given <see cref="DbContext" /> context.
    /// </summary>
    /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> object of the given <see cref="DbContext" /> context.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the <see cref="DbConnectionHandle" /> connection handle created.
    /// </returns>
    public static async Task<DbConnectionHandle> GetDbConnectionHandleAsync(this DatabaseFacade databaseFacade, CancellationToken cancellationToken = default)
    {
        DbConnection connection = databaseFacade.GetDbConnection();

        DbConnectionHandle connectionHandle = await DbConnectionHandleAsyncHelpers.CreateDbConnectionHandleAsync(connection, cancellationToken);

        return connectionHandle;
    }
}
