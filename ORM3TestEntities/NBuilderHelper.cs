using System.Diagnostics.CodeAnalysis;
using FizzWare.NBuilder;
using OR_Mapper.Framework;

namespace Orm3TestEntities;

public static class NBuilderHelper
{
    private static Random dice = new Random(Guid.NewGuid().GetHashCode());
    private static int counterCustomerIDCounter = 0;
    private static  int CounterBookIdCounter = 0;
    private static  int CounterChapterIdCounter = 0;
    private static  int CounterPagesIdCounter = 0;
    private static int InformationIdCounter;
    
    public static List<Customers> GetListOfCustomerInformation1To1(int size)
    {
        return Builder<Customers>.CreateListOfSize(size).All()
            .With(c => c.Id = ++counterCustomerIDCounter)
            .With(c => c.Email = Faker.Internet.Email())
            .With(c => c.Lastname = Faker.Name.First()).With(c => c.Lastname = Faker.Name.Last())
            .With(c => c.Isvip = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Customerlikescars = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Customerlikescolorgreen = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Defaultpurchasepricemultiplicator = dice.Next() / dice.Next())
            .With(c => c.Totalarticlesaddedtoshoppingcart = dice.Next())
            .Build().ToList();
    }
    
    public static List<Books> GetListOfBooks(int sizeListBooks,int minSizeChapters,int maxSizeChapters,int minSizePages,int maxSizePages)
    {
        return Builder<Books>.CreateListOfSize(sizeListBooks).All()
            .With(c => c.Authorname = Faker.Name.FullName())
            .With(c => c.Bookname = Faker.Name.FullName())
            .With(c => c.Price = Faker.RandomNumber.Next())
            .With(c => c.Id = ++CounterBookIdCounter).Do(c =>
            {
                int randomNumber = dice.Next(minSizeChapters, maxSizeChapters);
                var list = new List<Chapters>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreateChapter(c,CounterBookIdCounter, minSizePages, maxSizePages));
                }
                c.Chapters = new List<Chapters>(list);
            }).Build().ToList();
    }

    public static Chapters CreateChapter(Books reference,int bookID,int minSizePages, int MaxSizePages)
    {
        return Builder<Chapters>.CreateNew()
            .With(c => c.Id = ++CounterChapterIdCounter)
            .With(c => c.Chaptername = Faker.Name.FullName())
            .With(c => c.Book = reference)
            .Do(c=>
            {
                int randomNumber = dice.Next(minSizePages, MaxSizePages);
                var list = new List<Pages>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreatePage(c,CounterChapterIdCounter));
                }
                c.Pages = new List<Pages>(list);
            })
            .Build();
    }

    public static Pages CreatePage(Chapters reference,int chapterID)
    {
        return Builder<Pages>.CreateNew()
            .With(c => c.Id = ++CounterPagesIdCounter)
            .With(c => c.Text = Faker.Lorem.Sentence(300))
            .With(c => c.Chapter = reference).Build();
    }

    private static int billsIDCounter;
    private static int articlesIdCounter;
    public static List<Bills> GetBillsAndArticles(int listBillSize,int listArticleSize,int minArticlesPerBillAddAttempt,int maxArticlesPerBillAddAttempt)
    {
        var bills = GetBills(listBillSize);
        var articles = GetArticles(listArticleSize);
       
        var offset = dice.Next(0, listArticleSize);
        foreach (var item in bills)
        {
            var breakoff = dice.Next(minArticlesPerBillAddAttempt, maxArticlesPerBillAddAttempt);
            for (int i = 0; i < breakoff; i++)
            {
                item.Articles.Add(articles[(offset+i)%listArticleSize]);
            }
            item.Articles = item.Articles.Distinct().ToList();
        }
        return bills.ToList();
    }

    private static List<Bills>  GetBills(int listBillSize)
    {
        return Builder<Bills>.CreateListOfSize(listBillSize).All()
            .With(c => c.Id = ++billsIDCounter)
            .With(c => c.Purchaseprice = dice.Next()).Build().ToList();
    }
    public static List<Articles> GetArticles(int listArticleSize)
    {
        return Builder<Articles>.CreateListOfSize(listArticleSize).All()
            .With(c => c.Id = ++articlesIdCounter)
            .With(c => c.Articlename = Faker.Name.FullName())
            .With(c => c.Articleprice = dice.Next())
            .With(c => c.Ishidden = Convert.ToBoolean(dice.Next(0,2)))
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
            .With(c => c.Knight = knight)
            .With(c => c.weaponname = Faker.Name.First()).Build();
    }
}