using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using Npgsql;
using OR_Mapper.Framework.Database;
using Orm3TestEntities;

namespace F3InsertTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F3InsertPerformance1To1 : F3GlobalConfig.F3GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfKnights;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;

    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript("F3CreateAndDrop.sql");
    }
    private int cache = -1;
    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            SetUpGenericF3();
            listInsertKnight = NBuilderHelper.GetKnights(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertKnight;
            SetUpGenericF3();
            listInsertKnight = tempList;
        }
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanKnights();
    }
    [Benchmark]
    public void InsertData()
    {
        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            listInsertKnight[i].weapon.Save();
            listInsertKnight[i].Save();
        }
    }
}