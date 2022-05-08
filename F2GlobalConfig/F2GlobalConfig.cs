using GlobalConfigBase;
using ORM.Core;
using ORM.Core.Caching;
using ORM2TestEntiteis;
using Serilog;

namespace F2GlobalConfig;

public class F2GlobalConfig : GlobalConfig
{
    protected string FilePath = "C:\\Users\\lukas\\Documents\\Github\\ORMTestBachelorArbeit\\Data\\F2CreateAndDrop.sql";
    
    protected DbContext _dbContext;
    protected IList<bills> listInsertBills;
    protected IList<bills> listGetBills;
    
    protected IList<customers> listInsertCustomers;
    protected IList<customers> listGetCustomers;
    
    protected IList<books> listInsertBooks;
    protected IList<books> listGetBooks;
   
    protected IList<knight> listInsertKnight;
    protected IList<knight> listGetKnight;

    public void ResetCache()
    {
        _dbContext._cache = new EntityCache();
    }

    public void SetUpGenericF2()
    {
        listInsertKnight = new List<knight>();
        listGetKnight = new List<knight>();

        listInsertBills = new List<bills>();
        listGetBills = new List<bills>();

        listInsertCustomers = new List<customers>();
        listGetCustomers = new List<customers>();

        listInsertBooks = new List<books>();
        listGetBooks = new List<books>();

        if (_dbContext == null)
        {
            _dbContext = new F2Context();
        }
        else
        {
            ResetCache();
        }

    }

    #region EntityComparers
    
    public void CompareBook(books shouldBe,books actual)
    {
        if (shouldBe.bookid != actual.bookid)
        {
            Console.WriteLine("ERROR, Bookid missmatch");
        }
        if (shouldBe.bookname != actual.bookname)
        {
            Console.WriteLine("ERROR, Bookname missmatch");
        }
        if (shouldBe.price != actual.price)
        {
            Console.WriteLine("ERROR, Price missmatch");
        }
        if (shouldBe.authorname != actual.authorname)
        {
            Console.WriteLine("ERROR, Authorname missmatch");
        }
        if (shouldBe.chapters.Count != actual.chapters.Count)
        {
            Console.WriteLine("ERROR, Chapters.Count missmatch");
        }
    }
    public void CompareChapter(chapters shouldBe,chapters actual)
    {
        if (shouldBe.chapterid != actual.chapterid)
        {
            Console.WriteLine("ERROR, Chapterid missmatch");
        }
        if (shouldBe.chaptername != actual.chaptername)
        {
            Console.WriteLine("ERROR, Chaptername missmatch");
        }
        if (shouldBe.fk_books_bookid.bookid != actual.fk_books_bookid.bookid)
        {
            Console.WriteLine("ERROR, FkBook.Bookid missmatch");
        }
    }
    public void ComparePages(pages shouldBe,pages actual)
    {
        if (shouldBe.pagesid != actual.pagesid)
        {
            Console.WriteLine("ERROR, Pagesid missmatch");
        }
        if (shouldBe.fk_chapter_Id.chapterid != actual.fk_chapter_Id.chapterid )
        {
            Console.WriteLine("ERROR, FkChapterId missmatch");
        }
        if (shouldBe.text != actual.text)
        {
            Console.WriteLine("ERROR, Text missmatch");
        }
    }

    public void CompareCustomers(customers shouldBe, customers actual)
    {
        if (shouldBe.customerid != actual.customerid)
        {
            Console.WriteLine("ERROR, id missmatch");
        }
        if (shouldBe.lastname != actual.lastname)
        {
            Console.WriteLine("ERROR, Lastname missmatch");
        }
        if (shouldBe.isvip != actual.isvip)
        {
            Console.WriteLine("Error, Isvip missmatch");
        }
        if (shouldBe.customerlikescolorgreen != actual.customerlikescolorgreen)
        {
            Console.WriteLine("ERROR, Customerlikescolorgreen missmatch");
        }
        if (shouldBe.customerlikescars != actual.customerlikescars)
        {
            Console.WriteLine("ERROR, Customerlikescars missmatch");
        }
        if (shouldBe.totalarticlesaddedtoshoppingcart != actual.totalarticlesaddedtoshoppingcart)
        {
            Console.WriteLine("ERROR, Totalarticlesaddedtoshoppingcart missmatch");
        }
        if (shouldBe.defaultpurchasepricemultiplicator != actual.defaultpurchasepricemultiplicator)
        {
            Console.WriteLine("ERROR, Defaultpurchasepricemultiplicator missmatch");
        }
    }
    
    public void CompareBills(bills shouldBe,bills actual)
    {
        if (shouldBe.billsid != actual.billsid)
        {
            Console.WriteLine("ERROR, Billsid missmatch");
        }
        if (shouldBe.purchaseprice != actual.purchaseprice)
        {
            Console.WriteLine("ERROR, Purcahsedate missmatch");
        }
    }
    public void CompareArticles(articles shouldBe,articles actual)
    {
        if (shouldBe.articlesid != actual.articlesid)
        {
            Console.WriteLine("ERROR, Articlesid missmatch");
        }
        if (shouldBe.articleprice != actual.articleprice)
        {
            Console.WriteLine("ERROR, Articleprice missmatch");
        }
        if (shouldBe.articlename != actual.articlename)
        {
            Console.WriteLine("ERROR, Articlename missmatch");
        }
        if (shouldBe.ishidden != actual.ishidden)
        {
            Console.WriteLine("ERROR, Ishidden missmatch");
        }
    }
    #endregion
}