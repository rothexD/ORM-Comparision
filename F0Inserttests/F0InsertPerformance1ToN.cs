﻿using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using Microsoft.EntityFrameworkCore;
using ORM0Entities;
using ORM0Entities.autogenerated;

namespace F0Inserttests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F0InsertPerformance1ToN : F0GlobalConfig.F0GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBooks;
    
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
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements,MinNumberOfChaptersPerBook,MaxNumberOfChaptersPerBook,MinNumberOfPagesPerChapter,MaxNumberOfPagesPerChapter);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF0();
            listInsertBooks = tempList;
        }
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanBooks();
    }

    [Benchmark]
    public void InsertData()
    {
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            context.Books.Add(listInsertBooks[i]);
            context.SaveChanges();
        }
    }
}