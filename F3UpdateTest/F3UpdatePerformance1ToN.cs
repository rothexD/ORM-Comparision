using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using OR_Mapper.Framework.Database;
using Orm3TestEntities;

namespace F3UpdateTest;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F3UpdatePerformance1ToN : F3GlobalConfig.F3GlobalConfig
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
            SetUpGenericF3();
            listInsertBooks = NBuilderHelper.GetListOfBooks(numberOfStatements,MinNumberOfChaptersPerBook,MaxNumberOfChaptersPerBook,MinNumberOfPagesPerChapter,MaxNumberOfPagesPerChapter);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBooks;
            SetUpGenericF3();
            listInsertBooks = tempList;
            for (int i = 0; i < listInsertBooks.Count; i++)
            {
                Books temp = listInsertBooks[i];
                temp.Bookname = "bbbbbbbb";

                temp.Chapters[1].Chaptername = "bbbbbbbb";
                temp.Chapters[1].Pages[1].Text = "bbbbbbbb";
            }
        }
        
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
            Books temp = listInsertBooks[i];
            temp.Bookname = "aaaaaaaaa";

            temp.Chapters[1].Chaptername = "aaaaaaaaa";
            temp.Chapters[1].Pages[1].Text = "aaaaaaaaa";

            Db.Save(temp);
            Db.Save(temp.Chapters[1]);
            Db.Save(temp.Chapters[1].Pages[1]);
        }
    }
}