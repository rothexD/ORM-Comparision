using FizzWare.NBuilder;

namespace ORM1Entities;

public static class NBuilderHelper
{
    private static Random dice = new Random(Guid.NewGuid().GetHashCode());
    private static int counterCustomerID = 0;
    private static  int CounterBookId = 0;
    private static  int CounterChapterId = 0;
    private static  int CounterPagesId = 0;
    private static int additionalInformationID = 0;
    
    public static IList<Customers> GetListOfCustomerInformation1To1(int size)
    {
        return Builder<Customers>.CreateListOfSize(size).All()
            .With(c => c.Id = ++counterCustomerID)
            .With(c => c.Email = Faker.Internet.Email())
            .With(c => c.Lastname = Faker.Name.Last())
            .With(c => c.Isvip = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Customerlikescars = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Customerlikescolorgreen = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Defaultpurchasepricemultiplicator = dice.Next() / dice.Next())
            .With(c => c.Totalarticlesaddedtoshoppingcart = dice.Next())
            .Build();
    }
 
    
    public static IList<Books> GetListOfBooks(int sizeListBooks,int minSizeChapters,int maxSizeChapters,int minSizePages,int maxSizePages)
    {
        return Builder<Books>.CreateListOfSize(sizeListBooks).All()
            .With(c => c.Authorname = Faker.Name.FullName())
            .With(c => c.Bookname = Faker.Name.FullName())
            .With(c => c.Price = Faker.RandomNumber.Next())
            .With(c => c.Id = ++CounterBookId).Do(c =>
            {
                int randomNumber = dice.Next(minSizeChapters, maxSizeChapters);
                var list = new List<Chapters>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreateChapter(c, minSizePages, maxSizePages));
                }
                c.Chapter = list;
            }).Build();
    }

    public static Chapters CreateChapter(Books reference,int minSizePages, int MaxSizePages)
    {
        return Builder<Chapters>.CreateNew()
            .With(c => c.Id = ++CounterChapterId)
            .With(c => c.Chaptername = Faker.Name.FullName())
            .With(c => c.Book = reference)
            .Do(c=>
            {
                int randomNumber = dice.Next(minSizePages, MaxSizePages);
                var list = new List<Pages>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreatePage(c));
                }
               c.Pages = list;
            })
            .Build();
    }

    public static Pages CreatePage(Chapters reference)
    {
        return Builder<Pages>.CreateNew()
            .With(c => c.Id = ++CounterPagesId)
            .With(c => c.Text = Faker.Lorem.Sentence(300))
            .With(c => c.Chapter = reference)
            .Build();
    }

    private static int billsIDCounter =0;
    private static int articlesIdCounter =0;
    public static IList<Bills> GetBillsAndArticles(int listBillSize,int listArticleSize,int minArticlesPerBillAddAttempt,int maxArticlesPerBillAddAttempt)
    {
        var bills = GetBills(listBillSize);
        var articles = GetArticles(listArticleSize);
        
        var offset = dice.Next(0, listArticleSize);
        foreach (var item in bills)
        {
            var breakoff = dice.Next(minArticlesPerBillAddAttempt, maxArticlesPerBillAddAttempt);
            for (int i = 0; i < breakoff; i++)
            {
                item.FkArticles.Add(articles[(offset+i)% listArticleSize]);
            }
            item.FkArticles = item.FkArticles.Distinct().ToList();
        }
        return bills;
    }

    private static IList<Bills>  GetBills(int listBillSize)
    {
        return Builder<Bills>.CreateListOfSize(listBillSize).All()
            .With(c => c.Id = ++billsIDCounter)
            .With(c => c.Purchaseprice = dice.Next()).Build();
    }
    public static IList<Articles> GetArticles(int listArticleSize)
    {
        return Builder<Articles>.CreateListOfSize(listArticleSize).All()
            .With(c => c.Id = ++articlesIdCounter)
            .With(c => c.Articlename = Faker.Name.FullName())
            .With(c => c.Articleprice = dice.Next())
            .With(c => c.Ishidden = Convert.ToBoolean(dice.Next(0,2)))
            .Build();
    }

    private static int KnightId= 0;
    private static int WeaponId= 0;
    public static IList<Knight> GetKnights(int size)
    {
        return Builder<Knight>.CreateListOfSize(size).All()
            .With(c => c.Id = KnightId++)
            .With(c => c.Name = Faker.Name.Last())
            .With(c => c.Weapon = GetWeapon(c))
            //.With(c => c.fk_WeaponId = c.Weapon.Id)
            .Build();
    }

    public static Weapon GetWeapon(Knight knight)
    {
        return Builder<Weapon>.CreateNew()
            .With(c => c.Id = WeaponId++)
            .With(c => c.Damage = dice.Next(0, 100))
            .With(c => c.Knight = knight)
            .With(c => c.WeaponName = Faker.Name.First()).Build();
    }
}