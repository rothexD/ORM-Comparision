using System.Diagnostics.CodeAnalysis;
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

public class F1InsertPerformance1To1 : F1GlobalConfig.F1GlobalConfig
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
            var temp = listInsertKnight[i].Weapon;
            temp.Knight = null;
            _dbContext.Add(temp);
            _dbContext.Add(listInsertKnight[i]);
        }
    }
}