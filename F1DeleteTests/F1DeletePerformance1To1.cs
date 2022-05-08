using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM1Entities;

namespace F1DeleteTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F1DeletePerformance1To1 : F1GlobalConfig.F1GlobalConfig
{

    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
    }
    
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfKnights;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;
    
    private int cache = -1;
    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            SetUpGenericF1();
            listInsertKnight = NBuilderHelper.GetKnights(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertKnight;
            SetUpGenericF1();
            listInsertKnight = tempList;
        } 
        
        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            var temp = listInsertKnight[i].Weapon;
            temp.Knight = null;
            _dbContext.Add(temp);
            _dbContext.Add(listInsertKnight[i]);
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
        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            _dbContext.Delete<Knight>(listInsertKnight[i].Id);
            _dbContext.Delete<Weapon>(listInsertKnight[i].Weapon.Id);
        }
    }
}