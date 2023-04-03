using System.Data.SqlClient;

namespace EWallet;

public static class DatabaseInitializer
{
    public static void AddStoredProcedures(string connectionString)
    {
	    var db = new SqlConnection(connectionString);
	    // SHOULD IMPLEMENT LATER
    }
}