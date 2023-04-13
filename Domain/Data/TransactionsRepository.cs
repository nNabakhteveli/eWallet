using System.Data;
using Dapper;
using EWallet.Domain.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Domain.Data;

public class TransactionsRepository : ITransactionsRepository
{
    private IDbConnection db;

    public TransactionsRepository(string connectionString)
    {
        db = new SqlConnection(connectionString);
    }

    public async Task<IEnumerable<Deposit>> FilterInRange(string startDate, string endDate)
    {
        return (await db.QueryAsync<Deposit>(
            "SELECT * FROM Transactions WHERE CreateDate BETWEEN @StartDate AND @EndDate", new { startDate, endDate })).ToList();
    }

    public async Task<IEnumerable<Deposit>> GetAllAsync()
    {
        return (await db.QueryAsync<Deposit>("SELECT * FROM Transactions")).ToList();
    }

    public async Task<TransactionEntity> CreateAsync(TransactionEntity transaction)
    {
        var sql =
            "INSERT INTO Transactions " +
            "(UserId, Amount, PaymentType, Currency, CreateDate, Status)" +
            " VALUES (@UserId, @Amount, @PaymentType, @Currency, @CreateDate, @Status)" +
            "SET @Id = cast(scope_identity() as int) " +
            "SELECT SCOPE_IDENTITY() AS NewId";

        var newId = (int)(await db.QueryAsync(sql, transaction)).SingleOrDefault().NewId;

        transaction.Id = newId;

        return transaction;
    }

    public async Task<Deposit> CreateAsync(Deposit transaction)
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

    public async Task<TransactionEntity> UpdateAsync(TransactionEntity transaction)
    {
        var sql =
            "UPDATE Transactions " +
            "SET UserId = @UserId, " +
            "PaymentType = @PaymentType, " +
            "Amount = @Amount, " +
            "Currency = @Currency, " +
            "CreateDate = @CreateDate, " +
            "Status = @Status " +
            "WHERE Id = @Id";

        await db.ExecuteAsync(sql, transaction);

        return transaction;
    }

    public async Task<Deposit> UpdateAsync(Deposit transaction)
    {
        var sql =
            "UPDATE Transactions " +
            "SET UserId = @UserId, " +
            "PaymentType = @PaymentType, " +
            "Amount = @Amount, " +
            "Currency = @Currency, " +
            "CreateDate = @CreateDate, " +
            "Status = @Status " +
            "WHERE Id = @Id";

        await db.ExecuteAsync(sql, transaction);

        return transaction;
    }
}