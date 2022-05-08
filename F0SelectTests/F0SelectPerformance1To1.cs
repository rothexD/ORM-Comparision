﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using Microsoft.EntityFrameworkCore;
using ORM0Entities;
using ORM0Entities.autogenerated;

namespace F0SelectTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F0SelectPerformance1To1 : F0GlobalConfig.F0GlobalConfig
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
    public void SelectData()
    {
        for (int i = 0; i < listInsertKnight.Count; i++)
        {
            listGetKnight.Add(context.Knights.Include(c => c.FkWeapon).First(c => c.Id == listInsertKnight[i].Id));
        }
    }
}