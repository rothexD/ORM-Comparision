using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM.Core;
using ORM.Postgres.Extensions;
using ORM2TestEntiteis;

namespace F2DeleteTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F2DeletePerformanceNToM : F2GlobalConfig.F2GlobalConfig
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
    public void DeleteData()
    {
        for (int i = 0; i < listInsertBills.Count; i++)
        {
          _dbContext.DeleteById<bills>(listInsertBills[i].billsid);
          
        }
    }
}