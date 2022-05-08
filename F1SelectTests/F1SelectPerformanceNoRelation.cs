using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using ORM.Cache;
using ORM.Core;
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

public class F1SelectPerformanceNoRelation : F1GlobalConfig.F1GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfCustomers;
    
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
            listInsertCustomers = NBuilderHelper.GetListOfCustomerInformation1To1(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertCustomers;
            SetUpGenericF1();
            listInsertCustomers = tempList;
        }
        
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            _dbContext.Add(listInsertCustomers[i]);
        }
        ResetCache();
    }
    
    [IterationCleanup]
    public void CleanUp()
    {
        CleanCustomers();
    }

    [Benchmark]
    public void SelectData()
    {
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            listGetCustomers.Add(_dbContext.Get<Customers>(listInsertCustomers[i].Id));
        }
    }
}