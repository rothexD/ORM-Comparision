using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM.Core;
using ORM.Postgres.Extensions;
using ORM2TestEntiteis;

namespace F2UpdateTest;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F2UpdatePerformanceNToM : F2GlobalConfig.F2GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBills;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;
    
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
        DbContext.Configure(options =>
        {
            options.UseStateTrackingCache();
            options.UsePostgres(ConnectionString);
        });
        _dbContext = new F2Context();
    }

    
    private int cache = -1;
    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            if (listInsertBills != null)
            {
                listInsertBills.Clear();
            }
            SetUpGenericF2();
            listInsertBills = NBuilderHelper.GetBillsAndArticles(numberOfStatements,NumberOfArticles,MinArticlesToTryAdd,MaxArticlesToTryAdd);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBills;
            SetUpGenericF2();
            listInsertBills = tempList;
            for (int i = 0; i < listInsertBills.Count; i++)
            {
                bills temp = listInsertBills[i];
                temp.purchaseprice = i + i-2;

                temp.articles[1].articlename = "bbbbbbbb";
                temp.articles[1].ishidden = !temp.articles[1].ishidden;
            }
        }
        
        ResetCache();
        for (int i = 0; i < listInsertBills.Count; i++)
        {
            _dbContext.Save(listInsertBills[i]);
        }
        ResetCache();
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanBillsArticlesAlt();
    }

    [Benchmark]
    public void UpdateData()
    {
        for (int i = 0; i < listInsertBills.Count; i++)
        {
            bills temp = listInsertBills[i];
            temp.purchaseprice = 1;

            temp.articles[1].articlename = "aaaaaaaaa";
            temp.articles[1].ishidden = !temp.articles[1].ishidden;

            _dbContext.Save(temp);
        }
    }
}