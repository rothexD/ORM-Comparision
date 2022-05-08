using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Data;
using ORM.Core;
using ORM.Core.Caching;
using ORM.Postgres.Extensions;
using ORM2TestEntiteis;

namespace F2SelectTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F2SelectPerformance1ToN : F2GlobalConfig.F2GlobalConfig
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
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            _dbContext.Save(listInsertBooks[i]);
            for (int k = 0; k< listInsertBooks[i].chapters.Count; k++)
            {
                _dbContext.Save(listInsertBooks[i].chapters[k]);
            }
        }
        ResetCache();
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanBooks();
    }
    
    [Benchmark]
    public void SelectData()
    {
       // throw new NotImplementedException("broken");
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            listGetBooks.Add(_dbContext.GetById<books>(listInsertBooks[i].bookid));
            for (int j = 0; j < listGetBooks[i].chapters.Count; j++)
            {
                for (int k = 0; k < listGetBooks[i].chapters[j].Pages.Count;k++)
                {
                    _ = listGetBooks[i].chapters[j].Pages[k].pagesid;
                }
            }
        }
    }
}