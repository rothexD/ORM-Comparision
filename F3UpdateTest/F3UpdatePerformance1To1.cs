using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Orm3TestEntities;

namespace F3UpdateTest;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F3UpdatePerformance1To1 : F3GlobalConfig.F3GlobalConfig
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
            for (var i = 0; i < listInsertKnight.Count; i++)
            {
                listInsertKnight[i].name = "bbbbbbbb";
                listInsertKnight[i].weapon.weaponname = "bbbbbbbb";
                listInsertKnight[i].weapon.damage = i + i - 2;
            }
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
    public void UpdateData()
    {
        for (int i = 0; i < listInsertKnight.Count; i++)
        {      
            listInsertKnight[i].name = "aaaaaaaa";
            listInsertKnight[i].weapon.weaponname = "aaaaaaaa";
            listInsertKnight[i].weapon.damage = i + i;

            listInsertKnight[i].weapon.Save();
            listInsertKnight[i].Save();
        }
    }
}