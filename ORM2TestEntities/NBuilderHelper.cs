using FizzWare.NBuilder;

namespace ORM2TestEntiteis;

public static class NBuilderHelper
{
    private static Random dice = new Random(Guid.NewGuid().GetHashCode());
    private static int counterCustomerIDCounter = 0;
    private static  int CounterBookIdCounter = 0;
    private static  int CounterChapterIdCounter = 0;
    private static  int CounterPagesIdCounter = 0;
    private static int InformationIdCounter;
    
    public static List<customers> GetListOfCustomerInformation1To1(int size)
    {
        return Builder<customers>.CreateListOfSize(size).All()
            .With(c => c.customerid = ++counterCustomerIDCounter)
            .With(c => c.email = Faker.Internet.Email())
            .With(c => c.lastname = Faker.Name.First()).With(c => c.lastname = Faker.Name.Last())
            .With(c => c.isvip = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.customerlikescars = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.customerlikescolorgreen = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.defaultpurchasepricemultiplicator = dice.Next() / dice.Next())
            .With(c => c.totalarticlesaddedtoshoppingcart = dice.Next())
            .Build().ToList();
    }
    
    public static List<books> GetListOfBooks(int sizeListBooks,int minSizeChapters,int maxSizeChapters,int minSizePages,int maxSizePages)
    {
        return Builder<books>.CreateListOfSize(sizeListBooks).All()
            .With(c => c.authorname = Faker.Name.FullName())
            .With(c => c.bookname = Faker.Name.FullName())
            .With(c => c.price = Faker.RandomNumber.Next())
            .With(c => c.bookid = ++CounterBookIdCounter).Do(c =>
            {
                int randomNumber = dice.Next(minSizeChapters, maxSizeChapters);
                var list = new List<chapters>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreateChapter(c,CounterBookIdCounter, minSizePages, maxSizePages));
                }
                c.chapters = (list);
            }).Build().ToList();
    }

    public static chapters CreateChapter(books reference,int bookID,int minSizePages, int MaxSizePages)
    {
        return Builder<chapters>.CreateNew()
            .With(c => c.chapterid = ++CounterChapterIdCounter)
            .With(c => c.chaptername = Faker.Name.FullName())
            .With(c => c.fk_books_bookid = reference)
            .Do(c=>
            {
                int randomNumber = dice.Next(minSizePages, MaxSizePages);
                var list = new List<pages>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreatePage(c,CounterChapterIdCounter));
                }
                c.Pages = (list);
            })
            .Build();
    }

    public static pages CreatePage(chapters reference,int chapterID)
    {
        return Builder<pages>.CreateNew()
            .With(c => c.pagesid = ++CounterPagesIdCounter)
            .With(c => c.text = Faker.Lorem.Sentence(300))
            .With(c => c.fk_chapter_Id = reference).Build();
    }

    private static int billsIDCounter;
    private static int articlesIdCounter;
    public static List<bills> GetBillsAndArticles(int listBillSize,int listArticleSize,int minArticlesPerBillAddAttempt,int maxArticlesPerBillAddAttempt)
    {
        var bills = GetBills(listBillSize);
        var articles = GetArticles(listArticleSize);
        var offset = dice.Next(0, listArticleSize);
        foreach (var item in bills)
        {
            var breakoff = dice.Next(minArticlesPerBillAddAttempt, maxArticlesPerBillAddAttempt);
            for (int i = 0; i < breakoff; i++)
            {
                item.articles.Add(articles[(offset+i)%listArticleSize]);
            }
            item.articles = item.articles.Distinct().ToList();
        }
        return bills.ToList();
    }

    private static List<bills>  GetBills(int listBillSize)
    {
        return Builder<bills>.CreateListOfSize(listBillSize).All()
            .With(c => c.billsid = ++billsIDCounter)
            .With(c => c.purchaseprice = dice.Next()).Build().ToList();
    }
    public static List<articles> GetArticles(int listArticleSize)
    {
        return Builder<articles>.CreateListOfSize(listArticleSize).All()
            .With(c => c.articlesid = ++articlesIdCounter)
            .With(c => c.articlename = Faker.Name.FullName())
            .With(c => c.articleprice = dice.Next())
            .With(c => c.ishidden = Convert.ToBoolean(dice.Next(0,2)))
            .Build().ToList();
    }
    
    private static int KnightId= 0;
    private static int WeaponId= 0;
    public static IList<knight> GetKnights(int size)
    {
        return Builder<knight>.CreateListOfSize(size).All()
            .With(c => c.id = KnightId++)
            .With(c => c.name = Faker.Name.Last())
            .With(c => c.weapon = GetWeapon(c))
            //.With(c => c.fk_WeaponId = c.Weapon.Id)
            .Build();
    }

    public static weapon GetWeapon(knight knight)
    {
        return Builder<weapon>.CreateNew()
            .With(c => c.id = WeaponId++)
            .With(c => c.damage = dice.Next(0, 100))
            .With(c => c.knight = knight)
            .With(c => c.weaponname = Faker.Name.First()).Build();
    }
}