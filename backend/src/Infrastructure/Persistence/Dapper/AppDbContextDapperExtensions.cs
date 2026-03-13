using System.Data.Common;
using Dapper;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace backend.src.Infrastructure.Persistence.Dapper;

internal static class AppDbContextDapperExtensions
{
    public static DbConnection GetConnection(this AppDbContext context)
    {
        return context.Database.GetDbConnection();
    }

    public static CommandDefinition CreateCommand(this AppDbContext context, string sql, object? parameters = null, CancellationToken cancellationToken = default)
    {
        return new CommandDefinition(
            sql,
            parameters,
            context.Database.CurrentTransaction?.GetDbTransaction(),
            cancellationToken: cancellationToken);
    }
}
