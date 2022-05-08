using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using Microsoft.Diagnostics.Runtime.ICorDebug;
using Npgsql;
using ORM.Cache;
using ORM.Core;
using ORM.Core.FluentApi;
using ORM.PostgresSQL;
using ORM1Entities;
using Serilog;

namespace F1InsertTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F1InsertPerformanceNToM : F1GlobalConfig.F1GlobalConfig
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
    }
    
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBills;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;
    
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
    }
    [IterationCleanup]
    public void CleanUp()
    {
        CleanBillsArticles();
    }
    [Benchmark]
    public void InsertData()
    {
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
    }
}