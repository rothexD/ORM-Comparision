using Npgsql;

namespace GlobalConfigBase;

public class GlobalConfig
{
    public static int[] NumberOfBills => new int[]{500};
    public static int NumberOfArticles => 5;
    public static int MinArticlesToTryAdd => 2;
    public static int MaxArticlesToTryAdd => 3;

    public static int[] NumberOfBooks => new int[]{500};
    public static int MinNumberOfChaptersPerBook => 2;
    public static int MaxNumberOfChaptersPerBook => 3;
    public static int MinNumberOfPagesPerChapter => 2;
    public static int MaxNumberOfPagesPerChapter => 3;

    public static int[] NumberOfCustomers => new int[]{500};

    public static int[] NumberOfKnights => new int[] {500};
    
    
     public const string ConnectionString =
        "Server=localhost;Port=5432;Database=remote_database;Uid=remote_user;Pwd=remote_password;Include Error Detail=true";

    public void DefineDatabaseModelUsingScript(string sqlfile)
    {
        using (var con = new NpgsqlConnection(ConnectionString))
        {
            con.Open();
            using (var command = con.CreateCommand())
            {
                command.CommandText = File.ReadAllText(sqlfile);
                command.ExecuteNonQuery();
            }
        }
    }

    public void CleanBooks()
    {
        using (var con = new NpgsqlConnection(ConnectionString))
        {
            con.Open();
            using (var command = con.CreateCommand())
            {
                command.CommandText = "Delete from Pages cascade";
                command.ExecuteNonQuery();
                command.CommandText = "Delete from Chapters cascade";
                command.ExecuteNonQuery();
                command.CommandText = "Delete from Books cascade";
                command.ExecuteNonQuery();
                command.Dispose();
            }

            con.Close();
            con.Dispose();
        }
    }   
    public void CleanKnights()
    {
        using (var con = new NpgsqlConnection(ConnectionString))
        {
            con.Open();
            using (var command = con.CreateCommand())
            {
                command.CommandText = "Delete from Knight cascade";
                command.ExecuteNonQuery();
                command.CommandText = "Delete from Weapon cascade";
                command.ExecuteNonQuery();
                command.Dispose();
            }

            con.Close();
            con.Dispose();
        }
    }


    public void CleanCustomers()
    {
        using (var con = new NpgsqlConnection(ConnectionString))
        {
            con.Open();
            using (var command = con.CreateCommand())
            {
                command.CommandText = "Delete from Customers cascade";
                command.ExecuteNonQuery();
            }

            con.Close();
        }
    }

    public void CleanBillsArticles()
    {
        using (var con = new NpgsqlConnection(ConnectionString))
        {
            con.Open();
            using (var command = con.CreateCommand())
            {
                command.CommandText = "Delete from billsarticles cascade";
                command.ExecuteNonQuery();
                command.CommandText = "Delete from Articles cascade";
                command.ExecuteNonQuery();
                command.CommandText = "Delete from Bills cascade";
                command.ExecuteNonQuery();
                command.Dispose();
            }
            con.Close();
            con.Dispose();
        }
    }
    public void CleanBillsArticlesAlt()
    {
        using (var con = new NpgsqlConnection(ConnectionString))
        {
            con.Open();
            using (var command = con.CreateCommand())
            {
                command.CommandText = "Delete from fk_articles_bills cascade";
                command.ExecuteNonQuery();
                command.CommandText = "Delete from Articles cascade";
                command.ExecuteNonQuery();
                command.CommandText = "Delete from Bills cascade";
                command.ExecuteNonQuery();
                command.Dispose();
            }
            con.Close();
            con.Dispose();
        }
    }

}