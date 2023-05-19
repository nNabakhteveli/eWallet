using System.Data;
using EWallet.Domain.Models;
using Microsoft.Data.SqlClient;

namespace EWallet.Domain.Data;

public class SeamlessRepository : ISeamlessRepository
{
    private string connectionString;

    public SeamlessRepository(string connString)
    {
        connectionString = connString;
    }
    
    public BetResult Bet(SeamlessBetRequest req)
    {
        var result = new BetResult();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand("Bet", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Token", req.Token));
            cmd.Parameters.Add(new SqlParameter("@Amount", req.Amount));
            cmd.Parameters.Add(new SqlParameter("@PaymentType", req.PaymentType));
            cmd.Parameters.Add(new SqlParameter("@Currency", req.Currency));
            cmd.Parameters.Add(new SqlParameter("@CreateDate", req.CreateDate));
            cmd.Parameters.Add(new SqlParameter("@Status", req.Status));

            try
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result.TransactionId = (int)rdr[0];
                        result.CurrentAmount = (decimal)rdr[1];

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }

        return result;
    }

    public BetResult Win(SeamlessBetRequest req)
    {
        var result = new BetResult();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand("Win", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Token", req.Token));
            cmd.Parameters.Add(new SqlParameter("@TransactionId", req.TransactionId));
            cmd.Parameters.Add(new SqlParameter("@Amount", req.Amount));
            cmd.Parameters.Add(new SqlParameter("@PaymentType", req.PaymentType));
            cmd.Parameters.Add(new SqlParameter("@Currency", req.Currency));
            cmd.Parameters.Add(new SqlParameter("@CreateDate", req.CreateDate));
            cmd.Parameters.Add(new SqlParameter("@Status", req.Status));

            try
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result.TransactionId = (int)rdr[0];
                        result.CurrentAmount = (decimal)rdr[1];

                        return result;
                    }
                    // If there isn't any data to read, it means that duplicate transaction error occurs
                    result.IsDuplicateTransaction = true;
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }
    }

    public BetResult CancelBet(SeamlessCancelBetRequest req)
    {
        var result = new BetResult();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand("CancelBet", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Token", req.Token));
            cmd.Parameters.Add(new SqlParameter("@OldTransactionId", req.OldTransactionId));
            cmd.Parameters.Add(new SqlParameter("@TransactionId", req.TransactionId));
            cmd.Parameters.Add(new SqlParameter("@Amount", req.Amount));
            cmd.Parameters.Add(new SqlParameter("@PaymentType", req.PaymentType));
            cmd.Parameters.Add(new SqlParameter("@Currency", req.Currency));
            cmd.Parameters.Add(new SqlParameter("@CreateDate", req.CreateDate));
            cmd.Parameters.Add(new SqlParameter("@Status", req.Status));

            try
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result.TransactionId = (int)rdr[0];
                        result.CurrentAmount = (decimal)rdr[1];

                        return result;
                    }
                    // If there isn't any data to read, it means that duplicate transaction error occurs
                    result.IsDuplicateTransaction = true;
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }
    }


    public BetResult ChangeWin(SeamlessCancelBetRequest req)
    {
        var result = new BetResult();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand("ChangeWin", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Token", req.Token));
            cmd.Parameters.Add(new SqlParameter("@OldTransactionId", req.OldTransactionId));
            cmd.Parameters.Add(new SqlParameter("@Amount", req.Amount));
            cmd.Parameters.Add(new SqlParameter("@PaymentType", req.PaymentType));
            cmd.Parameters.Add(new SqlParameter("@Currency", req.Currency));
            cmd.Parameters.Add(new SqlParameter("@CreateDate", req.CreateDate));
            cmd.Parameters.Add(new SqlParameter("@Status", req.Status));

            try
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result.TransactionId = (int)rdr[0];
                        result.CurrentAmount = (decimal)rdr[1];

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }
        return result;
    }

    public BetResult GetBalance(SeamlessBetRequest req)
    {
        var result = new BetResult
        {
            TransactionId = 1
        };

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand("GetBalance", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Token", req.Token));

            try
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result.TransactionId = 0;
                        result.CurrentAmount = (decimal)rdr[0];

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }
        return result;
    }
}