using System.Data;
using Dapper;
using EWallet.Domain.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Data;

public class TransactionsRepository : ITransactionsRepository
{
    private IDbConnection db;

    public TransactionsRepository(IConfiguration configuration)
    {
        db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task<TransactionEntity> CreateAsync(TransactionEntity transaction)
    {
        var sql =
            "INSERT INTO Transactions " +
            "(UserId, RecipientId, PaymentType, Amount, Currency, CreateDate, Status)" +
            " VALUES (@UserId, @RecipientId, @PaymentType, @Amount, @Currency, @CreateDate, @Status)" +
            "SET @Id = cast(scope_identity() as int) " +
            "SELECT SCOPE_IDENTITY() AS NewId";

        var newId = (int)(await db.QueryAsync(sql, transaction)).SingleOrDefault().NewId;

        transaction.Id = newId;

        return transaction;
    }
}