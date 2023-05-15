using System.Data;
using Dapper;
using EWallet.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace EWallet.Domain.Data;

public class TransactionsRepository : ITransactionsRepository
{
    private IDbConnection db;

    public TransactionsRepository(string connectionString)
    {
        db = new SqlConnection(connectionString);
    }

    public async Task<TransactionEntity> GetTransactionByIdAsync(string id)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@TransactionId", id);

        return (await db.QueryAsync<TransactionEntity>("GetTransactionById", parameters,
            commandType: CommandType.StoredProcedure)).FirstOrDefault();
    }

    public async Task<IEnumerable<TransactionEntity>> FilterInRange(string startDate = "", string endDate = "")
    {
        if (startDate.IsNullOrEmpty() || endDate.IsNullOrEmpty())
        {
            return await GetAllAsync();
        }

        var parameters = new DynamicParameters();

        parameters.Add("@StartDate", startDate);
        parameters.Add("@EndDate", endDate);

        return await db.QueryAsync<TransactionEntity>("GetTransactionsInRange", parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<TransactionEntity>> GetAllAsync()
    {
        return await db.QueryAsync<TransactionEntity>("GetAllTransactions", commandType: CommandType.StoredProcedure);
    }

    public async Task<TransactionEntity> CreateAsync(TransactionEntity transaction)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@Id", value: transaction.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@UserId", transaction.UserId);
        parameters.Add("@PaymentType", transaction.PaymentType);
        parameters.Add("@Amount", transaction.Amount);
        parameters.Add("@Currency", transaction.Currency);
        parameters.Add("@CreateDate", transaction.CreateDate);
        parameters.Add("@Status", transaction.Status);

        var newId =
            (await db.QueryAsync<int>("CreateTransaction", parameters, commandType: CommandType.StoredProcedure))
            .FirstOrDefault();

        transaction.Id = newId;

        return transaction;
    }

    public async Task<TransactionEntity> UpdateAsync(TransactionEntity transaction)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@Id", value: transaction.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@UserId", transaction.UserId);
        parameters.Add("@PaymentType", transaction.PaymentType);
        parameters.Add("@Amount", transaction.Amount);
        parameters.Add("@Currency", transaction.Currency);
        parameters.Add("@CreateDate", transaction.CreateDate);
        parameters.Add("@Status", transaction.Status);

        await db.ExecuteAsync("UpdateTransaction", parameters, commandType: CommandType.StoredProcedure);

        return transaction;
    }
}