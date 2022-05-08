using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using Npgsql;
using ORM.Core;
using ORM.Core.Caching;
using ORM.Postgres.Extensions;
using ORM2TestEntiteis;

namespace F2InsertTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F2InsertPerformance1ToN : F2GlobalConfig.F2GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBooks;
    
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
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements,MinNumberOfChaptersPerBook,MaxNumberOfChaptersPerBook,MinNumberOfPagesPerChapter,MaxNumberOfPagesPerChapter);

            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF2();
            listInsertBooks = tempList;
        }
        ResetCache();
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
            _dbContext.Save(listInsertBooks[i]);
            for (int k = 0; k< listInsertBooks[i].chapters.Count; k++)
            { 
                _dbContext.Save(listInsertBooks[i].chapters[k]);
                
                for (int j = 0; j < listInsertBooks[i].chapters[k].Pages.Count; j++)
                {
                    _dbContext.Save(listInsertBooks[i].chapters[k].Pages[j]);
                } 
            }
        } 
    }
}