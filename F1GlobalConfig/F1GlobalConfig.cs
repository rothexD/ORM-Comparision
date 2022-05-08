using GlobalConfigBase;
using ORM.Cache;
using ORM.Core;
using ORM.PostgresSQL;
using ORM1Entities;
using Serilog;

namespace F1GlobalConfig;

public class F1GlobalConfig : GlobalConfig
{
    protected string FilePath = "C:\\Users\\lukas\\Documents\\Github\\ORMTestBachelorArbeit\\Data\\F1CreateAndDrop.sql";
    
    protected DbContext _dbContext;
    protected IList<Bills> listInsertBills;
    protected IList<Bills> listGetBills;
    
    protected IList<Customers> listInsertCustomers;
    protected IList<Customers> listGetCustomers;
    
    protected IList<Books> listInsertBooks;
    protected IList<Books> listGetBooks;

    protected IList<Knight> listInsertKnight;
    protected IList<Knight> listGetKnight;
    
    public void ResetCache()
    {
        var logger = new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Debug().CreateLogger();
        _dbContext._cache = new TrackingCache(logger);
    }
    public void SetUpGenericF1()
    {
        listInsertKnight = new List<Knight>();
        listGetKnight = new List<Knight>();
    
        listInsertBills = new List<Bills>();
        listGetBills = new List<Bills>();

        listInsertCustomers = new List<Customers>();
        listGetCustomers = new List<Customers>();

        listInsertBooks = new List<Books>();
        listGetBooks = new List<Books>();
        
        
       
      
        if (_dbContext == null)
        {
            var _databaseWrapper =
                new PostgresDb(ConnectionString);

            var _logger = new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Debug().CreateLogger();
            Log.Logger = _logger;
           
            var cache = new TrackingCache(_logger);
            
            _dbContext = new DbContext(_databaseWrapper, cache,_logger);
        }
        else
        {
            ResetCache();
        }
       
    }
 
    #region Entitycomparers

    public void CompareCustomers(Customers shouldBe, Customers actual)
    {
        if (shouldBe.Id != actual.Id)
        {
            Console.WriteLine("ERROR, id missmatch");
        }
        if (shouldBe.Lastname != actual.Lastname)
        {
            Console.WriteLine("ERROR, Lastname missmatch");
        }
        if (shouldBe.Isvip != actual.Isvip)
        {
            Console.WriteLine("Error, Isvip missmatch");
        }
        if (shouldBe.Customerlikescolorgreen != actual.Customerlikescolorgreen)
        {
            Console.WriteLine("ERROR, Customerlikescolorgreen missmatch");
        }
        if (shouldBe.Customerlikescars != actual.Customerlikescars)
        {
            Console.WriteLine("ERROR, Customerlikescars missmatch");
        }
        if (shouldBe.Totalarticlesaddedtoshoppingcart != actual.Totalarticlesaddedtoshoppingcart)
        {
            Console.WriteLine("ERROR, Totalarticlesaddedtoshoppingcart missmatch");
        }
        if (shouldBe.Defaultpurchasepricemultiplicator != actual.Defaultpurchasepricemultiplicator)
        {
            Console.WriteLine("ERROR, Defaultpurchasepricemultiplicator missmatch");
        }
    }
    public void CompareBills(Bills shouldBe,Bills actual)
    {
        if (shouldBe.Id != actual.Id)
        {
            Console.WriteLine("ERROR, Billsid missmatch");
        }
        if (shouldBe.Purchaseprice != actual.Purchaseprice)
        {
            Console.WriteLine("ERROR, Price missmatch");
        }
        if (shouldBe.FkArticles.Count != actual.FkArticles.Count )
        {
            Console.WriteLine("ERROR, FkArticles.Count missmatch");
        }
    }
    public void CompareArticles(Articles shouldBe,Articles actual)
    {
        if (shouldBe.Id != actual.Id)
        {
            Console.WriteLine("ERROR, Articlesid missmatch");
        }
        if (shouldBe.Articleprice != actual.Articleprice)
        {
            Console.WriteLine("ERROR, Articleprice missmatch");
        }
        if (shouldBe.Articlename != actual.Articlename)
        {
            Console.WriteLine("ERROR, Articlename missmatch");
        }
        if (shouldBe.Ishidden != actual.Ishidden)
        {
            Console.WriteLine("ERROR, Ishidden missmatch");
        }
    }
    public void CompareBook(Books shouldBe,Books actual)
    {
        if (shouldBe.Id != actual.Id)
        {
            Console.WriteLine("ERROR, Bookid missmatch");
        }
        if (shouldBe.Bookname != actual.Bookname)
        {
            Console.WriteLine("ERROR, Bookname missmatch");
        }
        if (shouldBe.Price != actual.Price)
        {
            Console.WriteLine("ERROR, Price missmatch");
        }
        if (shouldBe.Authorname != actual.Authorname)
        {
            Console.WriteLine("ERROR, Authorname missmatch");
        }
    }
    public void CompareChapter(Chapters shouldBe,Chapters actual)
    {
        if (shouldBe.Id != actual.Id)
        {
            Console.WriteLine("ERROR, Chapterid missmatch");
        }
        if (shouldBe.Chaptername != actual.Chaptername)
        {
            Console.WriteLine("ERROR, Chaptername missmatch");
        }
        if (shouldBe.Book.Id != actual.Book.Id)
        {
            Console.WriteLine("ERROR, FkBook.Bookid missmatch");
        }
    }
    public void ComparePages(Pages shouldBe,Pages actual)
    {
        if (shouldBe.Id != actual.Id)
        {
            Console.WriteLine("ERROR, Pagesid missmatch");
        }
        if (shouldBe.Chapter.Id != actual.Chapter.Id )
        {
            Console.WriteLine("ERROR, FkChapterId missmatch");
        }
        if (shouldBe.Text != actual.Text)
        {
            Console.WriteLine("ERROR, Text missmatch");
        }
    }
    #endregion
}