﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;
using ORM0Entities.autogenerated;
using ORM1Entities;
using NBuilderHelper = ORM0Entities.NBuilderHelper;

namespace F0UpdateTest;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F0UpdatePerformance1ToN : F0GlobalConfig.F0GlobalConfig
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
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements,2,4,3,5);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF0();
            listInsertBooks = tempList;
            for (int i = 0; i < listInsertBooks.Count; i++)
            {
                Book temp = listInsertBooks[i];
                temp.Bookname = "bbbbbbbb";

                temp.Chapters[1].Chaptername = "bbbbbbbb";
                temp.Chapters[1].Pages[1].Text = "bbbbbbbb";
            }
        }
        
        
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            context.Books.Add(listInsertBooks[i]);
            context.SaveChanges();
        }
        ResetCache();
    }
    [IterationCleanup]
    public void CleanUp()
    {
        CleanBooks();
    }

    
    [Benchmark]
    public void UpdateData()
    {
        context.AttachRange(listInsertBooks);
        
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            Book temp = listInsertBooks[i];
            temp.Bookname = "aaaaaaaaa";

            temp.Chapters[1].Chaptername = "aaaaaaaaa";
            temp.Chapters[1].Pages[1].Text = "aaaaaaaaa";

            context.Update(temp);
            context.SaveChanges();
        }
    }
}