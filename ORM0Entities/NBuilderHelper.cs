﻿using System.Xml;
using Faker;
using FizzWare.NBuilder;
using ORM0Entities.autogenerated;

namespace ORM0Entities;

public static class NBuilderHelper
{
    private static Random dice = new Random(Guid.NewGuid().GetHashCode());
    private static int counterCustomerIDCounter = 0;
    private static  int CounterBookIdCounter = 0;
    private static  int CounterChapterIdCounter = 0;
    private static  int CounterPagesIdCounter = 0;
    private static int InformationIdCounter;
    public static IList<Customer> GetListOfCustomerInformation1To1(int size)
    {
        return Builder<Customer>.CreateListOfSize(size).All()
            .With(c => c.Customerid = ++counterCustomerIDCounter)
            .With(c => c.Email = Faker.Internet.Email())
            .With(c => c.Lastname = Faker.Name.First()).With(c => c.Lastname = Faker.Name.Last())
            .With(c => c.Isvip = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Customerlikescars = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Customerlikescolorgreen = Convert.ToBoolean(dice.Next(0, 2)))
            .With(c => c.Defaultpurchasepricemultiplicator = dice.Next() / dice.Next())
            .With(c => c.Totalarticlesaddedtoshoppingcart = dice.Next())
            .Build();
    }
    public static IList<Book> GetListOfBooks(int sizeListBooks,int minSizeChapters,int maxSizeChapters,int minSizePages,int maxSizePages)
    {
        return Builder<Book>.CreateListOfSize(sizeListBooks).All()
            .With(c => c.Authorname = Faker.Name.FullName())
            .With(c => c.Bookname = Faker.Name.FullName())
            .With(c => c.Price = Faker.RandomNumber.Next())
            .With(c => c.Bookid = ++CounterBookIdCounter).Do(c =>
            {
                int randomNumber = dice.Next(minSizeChapters, maxSizeChapters);
                var list = new List<Chapter>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreateChapter(CounterBookIdCounter, minSizePages, maxSizePages));
                }
                c.Chapters = list;
            }).Build();
    }

    public static Chapter CreateChapter(int bookID,int minSizePages, int MaxSizePages)
    {
        return Builder<Chapter>.CreateNew()
            .With(c => c.Chapterid = ++CounterChapterIdCounter)
            .With(c => c.Chaptername = Faker.Name.FullName())
            .With(c => c.FkBookid = bookID)
            .Do(c=>
            {
                int randomNumber = dice.Next(minSizePages, MaxSizePages);
                var list = new List<Page>();
                for (int i = 0; i < randomNumber; i++)
                {
                    list.Add(CreatePage(CounterChapterIdCounter));
                }
                c.Pages = list;
            })
            .Build();
    }

    public static Page CreatePage(int chapterID)
    {
        return Builder<Page>.CreateNew()
            .With(c => c.Pagesid = ++CounterPagesIdCounter)
            .With(c => c.Text = Faker.Lorem.Sentence(300))
            .With(c => c.FkChapterId = chapterID).Build();
    }

    private static int billsIDCounter;
    private static int articlesIdCounter;
    public static IList<Bill> GetBillsAndArticles(int listBillSize,int listArticleSize,int minArticlesPerBillAddAttempt,int maxArticlesPerBillAddAttempt,IList<Article> articles)
    {
        articles.Clear();
        var bills = GetBills(listBillSize);
        articles = GetArticles(listArticleSize);

        var offset = dice.Next(0, listArticleSize);
        foreach (var item in bills)
        {
            var breakoff = dice.Next(minArticlesPerBillAddAttempt, maxArticlesPerBillAddAttempt);
            for (int i = 0; i < breakoff; i++)
            {
                item.FkArticles.Add(articles[(offset+i) % listArticleSize]);
            }
            item.FkArticles = item.FkArticles.Distinct().ToList();
            foreach (var item2 in item.FkArticles)
            {
                item2.FkBills.Add(item);
            }
        }
        return bills;
    }

    private static IList<Bill>  GetBills(int listBillSize)
    {
        return Builder<Bill>.CreateListOfSize(listBillSize).All()
            .With(c => c.Billsid = ++billsIDCounter)
            .With(c => c.Purchaseprice = dice.Next()).Build();
    }
    public static IList<Article> GetArticles(int listArticleSize)
    {
        return Builder<Article>.CreateListOfSize(listArticleSize).All()
            .With(c => c.Articlesid = ++articlesIdCounter)
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
            .With(c => c.FkWeapon = GetWeapon(c))
            //.With(c => c.FkWeaponid = c.we.Id)
            .Build();
    }

    public static Weapon GetWeapon(Knight knight)
    {
        return Builder<Weapon>.CreateNew()
            .With(c => c.Id = WeaponId++)
            .With(c => c.Damage = dice.Next(0, 100))
            .With(c => c.Knight = knight)
            .With(c => c.Weaponname = Faker.Name.First()).Build();
    }
}