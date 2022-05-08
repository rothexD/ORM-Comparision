using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM1Entities;

namespace F1DeleteTests;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F1DeletePerformance1ToN : F1GlobalConfig.F1GlobalConfig
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

                tempPointerI[k].Pages = tempPointerK;
            }

            listInsertBooks[i].Chapter = tempPointerI;
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
        for (int i = 0; i < listInsertBooks.Count; i++)
        {
            for (int j = 0; j < listInsertBooks[i].Chapter.Count; j++)
            {
                for (int k = 0; k < listInsertBooks[i].Chapter[j].Pages.Count; k++)
                {
                    _dbContext.Delete<Pages>(listInsertBooks[i].Chapter[j].Pages[k].Id);
                }
                _dbContext.Delete<Chapters>(listInsertBooks[i].Chapter[j].Id);
            }
            _dbContext.Delete<Books>(listInsertBooks[i].Id);
        }
    }
}