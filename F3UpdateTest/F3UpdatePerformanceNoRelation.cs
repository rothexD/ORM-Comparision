using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using OR_Mapper.Framework.Database;
using Orm3TestEntities;

namespace F3UpdateTest;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F3UpdatePerformanceNoRelation : F3GlobalConfig.F3GlobalConfig
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
            SetUpGenericF3();
            listInsertCustomers = NBuilderHelper.GetListOfCustomerInformation1To1(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertCustomers;
            SetUpGenericF3();
            listInsertCustomers = tempList;
            for (int i = 0; i < listInsertCustomers.Count; i++)
            {
                Customers temp = listInsertCustomers[i];
                temp.Email = "bbbbbbbb";
                temp.Customerlikescolorgreen = !temp.Customerlikescolorgreen;
                temp.Lastname = "bbbbbbbb";
            }
        }
        
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            listInsertCustomers[i].Save();
        }
        ResetCache();
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanCustomers();
    }
    [Benchmark]
    public void UpdateData()
    {
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            Customers temp = listInsertCustomers[i];
            temp.Email = "aaaaaaaaa";
            temp.Customerlikescolorgreen = !temp.Customerlikescolorgreen;
            temp.Lastname ="aaaaaaaaa";
            
            Db.Save(temp);
        }
    }
}