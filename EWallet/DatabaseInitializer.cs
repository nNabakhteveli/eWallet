using Dapper;
using Microsoft.Data.SqlClient;

namespace BetsolutionsApi;

public class DatabaseInitializer
{
    public static void AddStoredProcedures(string connectionStr)
    {
        var db = new SqlConnection(connectionStr);

        // Transactions
        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[GetAllTransactions]
                AS 
                BEGIN
                     SELECT [Id],
                    [UserId],
                    [PaymentType], 
                    [Amount], 
                    [Currency],
					[CreateDate],
					[Status] 
                    FROM [dbo].[Transactions];
                END"
            );
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
            // Console.WriteLine();
        }

        try
        {
	        db.Query(
		        @"CREATE PROCEDURE [dbo].[GetTransactionsInRange]
				@StartDate varchar(50), 
				@EndDate varchar(50)
                AS 
                BEGIN
                     SELECT [Id], [UserId], [PaymentType], [Amount], [Currency], [CreateDate], [Status] 
                    FROM [dbo].[Transactions] WHERE CreateDate BETWEEN @StartDate AND @EndDate;
                END"
	        );
        }
        catch (Exception e)
        {
	        // Console.WriteLine(e);
	        // Console.WriteLine();
        }

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[CreateTransaction]
	            @Id	int output, 
	            @UserId	varchar(100), 
	            @PaymentType varchar(10), 	
	            @Amount	DECIMAL(10, 2), 
	            @Currency	varchar(10), 
				@CreateDate smalldatetime,  
				@Status int
                AS
                BEGIN
	                INSERT INTO [dbo].[Transactions]
		                ([UserId] 
						,[PaymentType] 
		                ,[Amount] 
		                ,[Currency] 
						,[CreateDate] 
						,[Status])
					VALUES
		                (@UserId,
		                @PaymentType, 
		                @Amount,
		                @Currency,
						@CreateDate,
						@Status);
	                SET @Id = cast(scope_identity() as int)
					SELECT @Id
                END;"
            );
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
        }

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[UpdateTransaction]
        		@Id	int output, 
	            @UserId	varchar(100), 
	            @PaymentType varchar(10),  	
	            @Amount	DECIMAL(10, 2), 
	            @Currency	varchar(10), 
				@CreateDate smalldatetime,   
				@Status int 
			    AS
			      BEGIN
        			UPDATE	Transactions
        			SET		
						UserId = @UserId,
        				PaymentType = @PaymentType,
        				Amount = @Amount,
        				Currency = @Currency,
						CreateDate = @CreateDate,
						Status = @Status
        			WHERE Id = @Id
				END;"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


        // Wallets
        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[CreateWallet]
	            @Id	int output, 
	            @CurrentBalance DECIMAL(10, 2) 
                AS
                BEGIN
	                INSERT INTO [dbo].[Wallets]
		                ([CurrentBalance])
					VALUES
		                (@CurrentBalance);
	                SET @Id = cast(scope_identity() as int)
					SELECT @Id
                END;"
            );
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
        }

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[GetWalletById]
                    @Id int output
                    AS 
                    BEGIN
                        SELECT Id, UserId, CurrentBalance
                        FROM [dbo].[Wallets] WHERE Id = @Id;
                    END"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[GetWalletByUserId]
                    @UserId varchar(200)
                    AS 
                    BEGIN
                        SELECT Id, UserId, CurrentBalance
                        FROM [dbo].[Wallets] WHERE UserId = @UserId;
                    END"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[UpdateWallet]
        		@Id	int output, 
	            @CurrentBalance DECIMAL(10, 2) 
			    AS
			      BEGIN
        			UPDATE	Wallets
        			SET		
						CurrentBalance = @CurrentBalance
        			WHERE Id = @Id
				END;"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        // Tokens

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[CreateToken]
	            @Id	int output, 
	            @UserId varchar(200), 
				@PublicToken varchar(200), 
				@PublicTokenStatus varchar(100), 
				@PrivateToken varchar(200), 
				@PrivateTokenStatus varchar(100)
                AS
                BEGIN
	                INSERT INTO [dbo].[Tokens]
		                ([UserId], [PublicToken], [PublicTokenStatus], [PrivateToken], [PrivateTokenStatus])
					VALUES
		                (@UserId, @PublicToken, @PublicTokenStatus, @PrivateToken, @PrivateTokenStatus);
	                SET @Id = cast(scope_identity() as int)
					SELECT @Id
                END;"
            );
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
        }

        try
        {
	        db.Query(
		        @"CREATE PROCEDURE [dbo].[GetTokenByPrivateToken]
					@PrivateToken varchar(200)
                    AS 
                    BEGIN
                        SELECT UserId, PublicToken, PublicTokenStatus, PrivateToken, PrivateTokenStatus
                        FROM [dbo].[Tokens] WHERE PrivateToken = @PrivateToken;
                    END"
	        );
        }
        catch (Exception e)
        {
	        Console.WriteLine(e);
        }
        
        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[GetTokenByUserId]
					@UserId varchar(200)
                    AS 
                    BEGIN
                        SELECT UserId, PublicToken, PublicTokenStatus, PrivateToken, PrivateTokenStatus
                        FROM [dbo].[Tokens] WHERE UserId = @UserId;
                    END"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[DeleteTokenByUserId]
             	@UserId varchar(200)
             	AS
             	BEGIN
             		DELETE FROM Tokens WHERE UserId = @UserId;
             	END;"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[DeleteTokenByPrivateToken]
             	@PrivateToken varchar(200)
             	AS
             	BEGIN
             		DELETE FROM Tokens WHERE PrivateToken = @PrivateToken;
             	END;"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}