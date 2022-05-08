using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM0Entities;

namespace F0DeleteTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter("", true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]
public class F0DeletePerformance1ToN : F0GlobalConfig.F0GlobalConfig
{
    [ParamsSource(nameof(valuesForNumberOfStatements))]
    public int numberOfStatements;

    public IEnumerable<int> valuesForNumberOfStatements()
    {
        return NumberOfBooks;
    }

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
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements, MinNumberOfChaptersPerBook,
                MaxNumberOfChaptersPerBook, MinNumberOfPagesPerChapter, MaxNumberOfPagesPerChapter);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF0();
            listInsertBooks = tempList;
        }

        

        for (var i = 0; i < listInsertBooks.Count; i++)
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
    public void DeleteData()
    {
        context.AttachRange(listInsertBooks);
        for (var i = 0; i < listInsertBooks.Count; i++)
        {
            
            context.Books.Remove(listInsertBooks[i]);
            context.SaveChanges();
        }
    }
}