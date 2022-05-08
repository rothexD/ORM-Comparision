using GlobalConfigBase;
using OR_Mapper.Framework.Caching;
using OR_Mapper.Framework.Database;
using Orm3TestEntities;

namespace F3GlobalConfig;

public class F3GlobalConfig : GlobalConfig
{
    protected string FilePath = "C:\\Users\\lukas\\Documents\\Github\\ORMTestBachelorArbeit\\Data\\F3CreateAndDrop.sql";
    
    protected IList<Bills> listInsertBills;
    protected IList<Bills> listGetBills;
    
    protected IList<Customers> listInsertCustomers;
    protected IList<Customers> listGetCustomers;
    
    protected IList<Books> listInsertBooks;
    protected IList<Books> listGetBooks;
    
    protected IList<knight> listInsertKnight;
    protected IList<knight> listGetKnight;

    public void ResetCache()
    {
        Db.Cache = new Cache();
    }

    public void SetUpGenericF3()
    {
        listInsertKnight = new List<knight>();
        listGetKnight = new List<knight>();
        
        listInsertBills = new List<Bills>();
        listGetBills = new List<Bills>();

        listInsertCustomers = new List<Customers>();
        listGetCustomers = new List<Customers>();

        listInsertBooks = new List<Books>();
        listGetBooks = new List<Books>();

        Db.DbSchema = "public";
        Db.ConnectionString = ConnectionString;
        Db.Cache = new Cache();
    }

    #region Entity Comparers

    
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

    public void CompareChapter(Chapters shouldBe, Chapters actual)
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

    public void CompareCusomters(Customers shouldBe, Customers actual)
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
    #endregion
}