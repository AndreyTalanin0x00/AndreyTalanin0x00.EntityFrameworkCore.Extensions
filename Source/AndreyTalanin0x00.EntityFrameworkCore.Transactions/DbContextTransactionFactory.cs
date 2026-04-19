using Microsoft.EntityFrameworkCore.Storage;

namespace AndreyTalanin0x00.EntityFrameworkCore.Transactions;

/// <summary>
/// Encapsulates a method that creates a new <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction.
/// </summary>
/// <returns>The <see cref="IDbContextTransaction" /> EntityFrameworkCore context transaction created.</returns>
public delegate IDbContextTransaction DbContextTransactionFactory();
