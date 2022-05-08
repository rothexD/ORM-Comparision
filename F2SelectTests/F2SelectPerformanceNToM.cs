using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM.Core;
using ORM.Postgres.Extensions;
using ORM.Postgres.SqlDialect;
using ORM2TestEntiteis;

namespace F2SelectTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter("", true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]
public class F2SelectPerformanceNToM : F2GlobalConfig.F2GlobalConfig
{
    private int cache = -1;

    [ParamsSource(nameof(valuesForNumberOfStatements))]
    public int numberOfStatements;

    public IEnumerable<int> valuesForNumberOfStatements()
    {
        return NumberOfBills;
    }


    [GlobalSetup]
    public void GlobalSetup()
    {
        DbContext.Configure(options =>
        {
            options.UseNoCache();
            options.UsePostgres(ConnectionString);
        });
        _dbContext = new F2Context();
    }

    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            Console.WriteLine("not cached");
            if (listInsertBills != null) listInsertBills.Clear();

            SetUpGenericF2();
            listInsertBills = NBuilderHelper.GetBillsAndArticles(numberOfStatements, NumberOfArticles,
                MinArticlesToTryAdd, MaxArticlesToTryAdd);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBills;
            SetUpGenericF2();
            listInsertBills = tempList;
        }

        ResetCache();
       
        for (var i = 0; i < listInsertBills.Count; i++) _dbContext.Save(listInsertBills[i]);
        
        ResetCache();
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanBillsArticlesAlt();
        _dbContext._commandBuilder._connection.Close();
        _dbContext._commandBuilder._connection.Open();
        DefineDatabaseModelUsingScript(FilePath);
        listGetBills.Clear();
        
    }
    [Benchmark]
    public void SelectData()
    {
        return;
        var result2 = _dbContext.GetAll<bills>();
        foreach (var item in result2)
        {
            _ = item.purchaseprice;
            listGetBills.Add(item);
            _ = item.articles.Count;
        }
    }
}