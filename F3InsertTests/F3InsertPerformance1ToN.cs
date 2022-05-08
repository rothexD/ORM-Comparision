using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using Newtonsoft.Json;
using Npgsql;
using OR_Mapper.Framework.Caching;
using OR_Mapper.Framework.Database;
using Orm3TestEntities;

namespace F3InsertTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F3InsertPerformance1ToN : F3GlobalConfig.F3GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBooks;
    
    [ParamsSource(nameof(valuesForNumberOfStatements))] 
    public int numberOfStatements;


    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript("F3CreateAndDrop.sql");
    }
    
    private int cache = -1;
    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            SetUpGenericF3();
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements,MinNumberOfChaptersPerBook,MaxNumberOfChaptersPerBook,MinNumberOfPagesPerChapter,MaxNumberOfPagesPerChapter);

            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF3();
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
            listInsertBooks[i].Save();
            for (int k = 0; k < listInsertBooks[i].Chapters.Count; k++)
            {
                listInsertBooks[i].Chapters[k].Save();
                for (int j = 0; j < listInsertBooks[i].Chapters[k].Pages.Count; j++)
                {
                    listInsertBooks[i].Chapters[k].Pages[j].Save();
                } 
            }
        }
    }
}