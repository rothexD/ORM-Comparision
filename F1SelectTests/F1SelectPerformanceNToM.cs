using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using ORM.Cache;
using ORM.Core;
using ORM.Core.FluentApi;
using ORM.PostgresSQL;
using ORM1Entities;
using Serilog;

namespace F1SelectTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F1SelectPerformanceNToM : F1GlobalConfig.F1GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBills;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;

    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
    }

    
    private int cache = -1;
    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            SetUpGenericF1();
            listInsertBills = NBuilderHelper.GetBillsAndArticles(numberOfStatements,NumberOfArticles,MinArticlesToTryAdd,MaxArticlesToTryAdd);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBills;
            SetUpGenericF1();
            listInsertBills = tempList;
        }
        
        for (int i = 0; i < listInsertBills.Count; i++)
        {
            List<Articles> tempPointerI = listInsertBills[i].FkArticles;
            
            for (int k = 0; k < tempPointerI.Count; k++)
            {
                if (FluentApi.Get<Articles>().EqualTo("id", tempPointerI[k].Id).Execute(_dbContext).Count == 0)
                {
                    tempPointerI[k].FkBills = new List<Bills>();
                    tempPointerI[k] = _dbContext.Add(tempPointerI[k]);
                }
            }
            _dbContext.Add(listInsertBills[i]);
        }
        
        ResetCache();
    }
    
    [IterationCleanup]
    public void CleanUp()
    {
        CleanBillsArticles();
    }

    

    [Benchmark]
    public void SelectData()
    {
        for (int i = 0; i < listInsertBills.Count; i++)
        {
            listGetBills.Add(_dbContext.Get<Bills>(listInsertBills[i].Id));
        }
    }
}