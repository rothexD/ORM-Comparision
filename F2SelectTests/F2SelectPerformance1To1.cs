using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM.Core;
using ORM.Postgres.Extensions;
using ORM2TestEntiteis;

namespace F2SelectTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F2SelectPerformance1To1 : F2GlobalConfig.F2GlobalConfig
{

    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfKnights;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
        DbContext.Configure(options =>
        {
            options.UseStateTrackingCache();
            options.UsePostgres(ConnectionString);
        });

        _dbContext = new F2Context();
    }
    private int cache = -1;
    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            SetUpGenericF2();
            listInsertKnight = NBuilderHelper.GetKnights(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertKnight;
            SetUpGenericF2();
            listInsertKnight = tempList;
        }
        
        ResetCache();
        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            _dbContext.Save(listInsertKnight[i].weapon);
            _dbContext.Save(listInsertKnight[i]);
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
            listGetKnight.Add(_dbContext.GetById<knight>(listInsertKnight[i].id));

            _ = listGetKnight[i].weapon.damage;
        }
    }
}