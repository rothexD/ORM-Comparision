using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;
using ORM0Entities;

namespace F0SelectTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F0SelectPerformance1ToN : F0GlobalConfig.F0GlobalConfig
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
    public void SelectData()
    {
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            listGetBooks.Add(context.Books.Include(c => c.Chapters)
                .ThenInclude(c => c.Pages).First(c => c.Bookid == listInsertBooks[i].Bookid));
        }
    }
}