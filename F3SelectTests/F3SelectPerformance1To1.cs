using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using OR_Mapper.Framework.Database;
using Orm3TestEntities;

namespace F3SelectTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F3SelectPerformance1To1 : F3GlobalConfig.F3GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfKnights;
    
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
            listInsertKnight = NBuilderHelper.GetKnights(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertKnight;
            SetUpGenericF3();
            listInsertKnight = tempList;
        }

        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            listInsertKnight[i].weapon.Save();
            listInsertKnight[i].Save();
        }
        ResetCache();
    }

    [IterationCleanup]
    public void CleanUp()
    {
      CleanKnights();
    }
    [Benchmark]
    public void SelectData()
    {
        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            listGetKnight.Add(Db.GetById<knight>(listInsertKnight[i].id));
        }
    }
}