using Dapper;
using Microsoft.Data.SqlClient;

namespace BetsolutionsApi;

public class DatabaseInitializer
{
    public static void AddStoredProcedures(string connectionStr)
    {
        var db = new SqlConnection(connectionStr);

        try
        {
            db.Query(
                @"
					CREATE PROCEDURE dbo.Bet
					@Token VARCHAR(200),  
					@PaymentType varchar(10), 	
					@Amount	DECIMAL(10, 2), 
					@Currency	varchar(10), 
					@CreateDate smalldatetime,  
					@Status int 
					AS 
					BEGIN
						DECLARE @UserId VARCHAR(200)
						DECLARE @NewTransactionId int;
						DECLARE @NewCurrentBalance DECIMAL(10, 2)
					   
						SELECT @UserId = UserId FROM Tokens WHERE PrivateToken = @Token
						
						INSERT INTO dbo.Transactions
					    ([UserId], [PaymentType], [Amount], [Currency], [CreateDate], [Status])
						VALUES
					        (@UserId, 
					        @PaymentType, 
					        @Amount,
					        @Currency,
							@CreateDate,
							@Status)
						SET @NewTransactionId = SCOPE_IDENTITY()

						UPDATE dbo.Wallets
						SET CurrentBalance = CurrentBalance - @Amount
						WHERE UserId = @UserId						
						
						SELECT @NewCurrentBalance = CurrentBalance From Wallets WHERE UserId = @UserId
						
						SELECT @NewTransactionId, @NewCurrentBalance;
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
                @"
					CREATE PROCEDURE dbo.Win
					@Token VARCHAR(200),  
					@PaymentType varchar(10), 	
					@Amount	DECIMAL(10, 2), 
					@Currency	varchar(10), 
					@CreateDate smalldatetime, 
					@TransactionId int, 
					@Status int 
					AS 
					BEGIN
						DECLARE @UserId VARCHAR(200)
						DECLARE @NewTransactionId int;
						DECLARE @NewCurrentBalance DECIMAL(10, 2)
						DECLARE @IsAlreadyProcessed int
						
						SET @IsAlreadyProcessed = 0;

						SELECT @IsAlreadyProcessed = Id from Transactions WHERE Id = @TransactionId
						
						IF @IsAlreadyProcessed = 0
						BEGIN
						   
							SELECT @UserId = UserId FROM Tokens WHERE PrivateToken = @Token
							
							SET IDENTITY_INSERT Transactions ON
							
							INSERT INTO dbo.Transactions
						    ([Id], [UserId], [PaymentType], [Amount], [Currency], [CreateDate], [Status])
							VALUES
						        (@TransactionId,
						        @UserId, 
						        @PaymentType, 
						        @Amount,
						        @Currency,
								@CreateDate,
								@Status)
							SET @NewTransactionId = SCOPE_IDENTITY()
						
						   
							UPDATE dbo.Wallets
							SET CurrentBalance = CurrentBalance + @Amount
							WHERE UserId = @UserId
							
							
							SELECT @NewCurrentBalance = CurrentBalance From Wallets WHERE UserId = @UserId
							
							
							SELECT @NewTransactionId, @NewCurrentBalance;
						END
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
                @"
				CREATE PROCEDURE dbo.CancelBet
				@Token VARCHAR(200),  
				@PaymentType varchar(10), 	
				@Amount	DECIMAL(10, 2), 
				@Currency	varchar(10), 
				@CreateDate smalldatetime,
				@OldTransactionId int,
				@TransactionId int,
				@Status int 
				AS 
				BEGIN
					DECLARE @UserId VARCHAR(200)
					DECLARE @NewTransactionId int;
					DECLARE @OldAmount DECIMAL(10, 2)
					DECLARE @NewCurrentBalance DECIMAL(10, 2)
					DECLARE @IsAlreadyProcessed int
					
					SET @IsAlreadyProcessed = 0;

					SELECT @IsAlreadyProcessed = Id from Transactions WHERE Id = @TransactionId
					
					IF @IsAlreadyProcessed = 0
					BEGIN
						SELECT @UserId = UserId FROM Tokens WHERE PrivateToken = @Token
						
						SET IDENTITY_INSERT Transactions ON
						
						INSERT INTO dbo.Transactions
					    ([Id], [UserId], [PaymentType], [Amount], [Currency], [CreateDate], [Status])
						VALUES
					        (@TransactionId,
					        @UserId, 
					        @PaymentType, 
					        @Amount,
					        @Currency,
							@CreateDate,
							@Status)
						SET @NewTransactionId = SCOPE_IDENTITY()
					
	   					SELECT @OldAmount = Amount FROM Transactions WHERE Id = @OldTransactionId
						
						
						UPDATE dbo.Wallets
						SET CurrentBalance = CurrentBalance + @OldAmount
						WHERE UserId = @UserId
						
						
						SELECT @NewCurrentBalance = CurrentBalance FROM Wallets WHERE UserId = @UserId
						
						
						SELECT @NewTransactionId, @NewCurrentBalance;
					END
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
                @"
					CREATE PROCEDURE dbo.ChangeWin
					@Token VARCHAR(200),  
					@PaymentType varchar(10), 	
					@Amount	DECIMAL(10, 2), 
					@Currency	varchar(10), 
					@CreateDate smalldatetime,
					@OldTransactionId int,
					@Status int 
					AS 
					BEGIN
						DECLARE @UserId VARCHAR(200)
						DECLARE @NewTransactionId int;
						DECLARE @OldAmount DECIMAL(10, 2)
						DECLARE @NewCurrentBalance DECIMAL(10, 2)
					   
						SELECT @UserId = UserId FROM Tokens WHERE PrivateToken = @Token
						
						INSERT INTO dbo.Transactions
					    ([UserId], [PaymentType], [Amount], [Currency], [CreateDate], [Status])
						VALUES
					        (@UserId, 
					        @PaymentType, 
					        @Amount,
					        @Currency,
							@CreateDate,
							@Status)
						SET @NewTransactionId = SCOPE_IDENTITY()

   						SELECT @OldAmount = Amount FROM Transactions WHERE Id = @OldTransactionId
						
						
						UPDATE dbo.Wallets
						SET CurrentBalance = CurrentBalance - @OldAmount + @Amount
						WHERE UserId = @UserId
						
						
						SELECT @NewCurrentBalance = CurrentBalance FROM Wallets WHERE UserId = @UserId
						
						
						SELECT @NewTransactionId, @NewCurrentBalance;
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
                @"
					CREATE PROCEDURE dbo.GetBalance
					@Token VARCHAR(200)
					AS
					BEGIN
						DECLARE @UserId VARCHAR(200)
						DECLARE @CurrentBalance DECIMAL(10, 2)
						
						SELECT @UserId = UserId FROM Tokens WHERE PrivateToken = @Token
						
						SELECT @CurrentBalance = CurrentBalance FROM Wallets WHERE UserId = @UserId
						
						SELECT @CurrentBalance
					END"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


        // Transactions
        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[GetTransactionById]
             	@TransactionId int
             	AS
             	BEGIN
             		SELECT Id, UserId, PaymentType, Amount, Currency, CreateDate, Status
					FROM Transactions WHERE Id = @TransactionId;
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
                    FROM [dbo].[Transactions] WHERE CreateDate >= @StartDate AND CreateDate < @EndDate;
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
				@UserId	VARCHAR(200), 
	            @CurrentBalance DECIMAL(10, 2) 
			    AS
			      BEGIN
        			UPDATE	Wallets
        			SET		
						CurrentBalance = @CurrentBalance,
						UserId = @UserId
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
                @"CREATE PROCEDURE [dbo].[UpdateTokenByPublicKey]
        		@Id	int output, 
	            @UserId varchar(200),  
				@PublicToken varchar(200),  
				@PublicTokenStatus varchar(100),  
				@PrivateToken varchar(200), 
				@PrivateTokenStatus varchar(100) 
			    AS
			      BEGIN
        			UPDATE	Tokens
        			SET		
						UserId = @UserId,
        				PublicToken = @PublicToken,
        				PublicTokenStatus = @PublicTokenStatus,
						PrivateToken = @PrivateToken,
        				PrivateTokenStatus = @PrivateTokenStatus
        			WHERE PublicToken = @PublicToken
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
                @"CREATE PROCEDURE [dbo].[GetTokenByPublicKey]
					@PublicToken varchar(200)
                    AS 
                    BEGIN
                        SELECT UserId, PublicToken, PublicTokenStatus, PrivateToken, PrivateTokenStatus
                        FROM [dbo].[Tokens] WHERE PublicToken = @PublicToken;
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


        try
        {
            db.Query(
                @"CREATE PROCEDURE [dbo].[GetUserById]
             	@UserId VARCHAR(200)
             	AS
             	BEGIN
             		SELECT Id, WalletId, UserName, Email, PhoneNumber from [dbo].[AspNetUsers] WHERE Id = @UserId;
             	END;"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}