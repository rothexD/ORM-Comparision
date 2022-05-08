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

public class F0DeletePerformance1To1 : F0GlobalConfig.F0GlobalConfig
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
            SetUpGenericF0();
            listInsertKnight = NBuilderHelper.GetKnights(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertKnight;
            SetUpGenericF0();
            listInsertKnight = tempList;
        }


        for (var i = 0; i < listInsertKnight.Count; i++)
        {
            context.Knights.Add(listInsertKnight[i]);
            context.SaveChanges();
        }
        ResetCache();
    }
    [IterationCleanup]
    public void CleanUp()
    { 
        CleanKnights();
    }

    [Benchmark]
    public void DeleteData()
    {
        context.AttachRange(listInsertKnight);
        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            context.Knights.Remove(listInsertKnight[i]);
           // context.Weapons.Remove(listInsertKnight[i].FkWeapon);
           context.SaveChanges();
        }
      
    }
}