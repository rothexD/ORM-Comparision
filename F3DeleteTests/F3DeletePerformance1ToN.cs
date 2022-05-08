using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using OR_Mapper.Framework.Database;
using Orm3TestEntities;

namespace F3DeleteTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter("", true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]
public class F3DeletePerformance1ToN : F3GlobalConfig.F3GlobalConfig
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
            SetUpGenericF3();
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements, MinNumberOfChaptersPerBook,
                MaxNumberOfChaptersPerBook, MinNumberOfPagesPerChapter, MaxNumberOfPagesPerChapter);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF3();
            listInsertBooks = tempList;
        }

        

        for (var i = 0; i < listInsertBooks.Count; i++)
        {
            listInsertBooks[i].Save();
            for (var k = 0; k < listInsertBooks[i].Chapters.Count; k++)
            {
                listInsertBooks[i].Chapters[k].Save();
                for (var j = 0; j < listInsertBooks[i].Chapters[k].Pages.Count; j++)
                    listInsertBooks[i].Chapters[k].Pages[j].Save();
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
    public void DeleteData()
    {
        for (var i = 0; i < listInsertBooks.Count; i++)
        {
            for (var j = 0; j < listInsertBooks[i].Chapters.Count; j++)
            {
                for (var k = 0; k < listInsertBooks[i].Chapters[j].Pages.Count; k++)
                {
                    Db.Delete(listInsertBooks[i].Chapters[j].Pages[k]);
                }
                Db.Delete(listInsertBooks[i].Chapters[j]);
            }
            Db.Delete(listInsertBooks[i]);
        }
    }
}