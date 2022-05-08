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

public class F1InsertPerformance1ToN : F1GlobalConfig.F1GlobalConfig
{

    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
    }
    
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBooks;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;
    
    private int cache = -1;
    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            SetUpGenericF1();
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements,MinNumberOfChaptersPerBook,MaxNumberOfChaptersPerBook,MinNumberOfPagesPerChapter,MaxNumberOfPagesPerChapter);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF1();
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
            List<Chapters> tempPointerI = listInsertBooks[i].Chapter;
            listInsertBooks[i].Chapter = new List<Chapters>();
            _dbContext.Add(listInsertBooks[i]);
            
            for (int k = 0; k < tempPointerI.Count; k++)
            {
                List<Pages> tempPointerK = tempPointerI[k].Pages;
                tempPointerI[k].Pages = new List<Pages>();
                _dbContext.Add(tempPointerI[k]);
                    
                for (int j = 0; j < tempPointerK.Count; j++)
                {
                    _dbContext.Add(tempPointerK[j]);
                }
            }
        }
    }
}