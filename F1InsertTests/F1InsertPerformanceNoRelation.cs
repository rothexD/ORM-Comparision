using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using Npgsql;
using ORM.Cache;
using ORM.Core;
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

public class F1InsertPerformanceNoRelation : F1GlobalConfig.F1GlobalConfig
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
    }
    
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfCustomers;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;
    
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
    }
    
    [IterationCleanup]
    public void CleanUp()
    {
        CleanCustomers();
    }
    [Benchmark]
    public void InsertData()
    {
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            _dbContext.Add(listInsertCustomers[i]);
        }
    }
}