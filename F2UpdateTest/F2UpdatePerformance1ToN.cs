using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM.Core;
using ORM.Postgres.Extensions;
using ORM2TestEntiteis;

namespace F2UpdateTest;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F2UpdatePerformance1ToN : F2GlobalConfig.F2GlobalConfig
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
            for (int i = 0; i < listInsertBooks.Count; i++)
            {
                books temp = listInsertBooks[i];
                temp.bookname = "bbbbbbbb";

                temp.chapters[1].chaptername = "bbbbbbbb";
                temp.chapters[1].Pages[1].text = "bbbbbbbb";
            }
        }
        
        ResetCache();
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
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            books temp = listInsertBooks[i];
            temp.bookname = "aaaaaaaaa";

            temp.chapters[1].chaptername = "aaaaaaaaa";
            temp.chapters[1].Pages[1].text = "aaaaaaaaa";

            _dbContext.Save(temp);
            _dbContext.Save(temp.chapters[1]);
            _dbContext.Save(temp.chapters[1].Pages[1]);
        }
    }
}