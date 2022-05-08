using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM0Entities;

namespace F0DeleteTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F0DeletePerformanceNoRelation : F0GlobalConfig.F0GlobalConfig
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
            SetUpGenericF0();
            listInsertCustomers = NBuilderHelper.GetListOfCustomerInformation1To1(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertCustomers;
            SetUpGenericF0();
            listInsertCustomers = tempList;
        }
        
        
        for (var i = 0; i < listInsertCustomers.Count; i++)
        {
            context.Customers.Add(listInsertCustomers[i]);
            context.SaveChanges();
        }
        ResetCache();
    }
    
    [IterationCleanup]
    public void CleanUp()
    {
        CleanCustomers();
    }

    [Benchmark]
    public void DeleteData()
    {
        context.AttachRange(listInsertCustomers);
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            context.Customers.Remove(listInsertCustomers[i]);
            context.SaveChanges();
        }
    }
}
