using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM.Core.FluentApi;
using ORM1Entities;

namespace F1UpdateTEst;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F1UpdatePerformanceNToM : F1GlobalConfig.F1GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfBills;
    
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
            listInsertBills = NBuilderHelper.GetBillsAndArticles(numberOfStatements,NumberOfArticles,MinArticlesToTryAdd,MaxArticlesToTryAdd);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertBills;
            SetUpGenericF1();
            listInsertBills = tempList;
            for (int i = 0; i < listInsertBills.Count; i++)
            {
                Bills temp = listInsertBills[i];
                temp.Purchaseprice = i + i-2;

                temp.FkArticles[1].Articlename = "bbbbbbbb";
                temp.FkArticles[1].Ishidden = !temp.FkArticles[1].Ishidden;
            }
        }
        
        for (int i = 0; i < listInsertBills.Count; i++)
        {
            List<Articles> tempPointerI = listInsertBills[i].FkArticles;
            
            for (int k = 0; k < tempPointerI.Count; k++)
            {
                if (FluentApi.Get<Articles>().EqualTo("id", tempPointerI[k].Id).Execute(_dbContext).Count == 0)
                {
                    tempPointerI[k].FkBills = new List<Bills>();
                    tempPointerI[k] = _dbContext.Add(tempPointerI[k]);
                }
            }
            _dbContext.Add(listInsertBills[i]);
        }
        
        ResetCache();
    }
    
    [IterationCleanup]
    public void CleanUp()
    {
        CleanBillsArticles();
    }
    
    [Benchmark]
    public void UpdateData()
    {
        for (int i = 0; i < listInsertBills.Count; i++)
        {
            Bills temp = listInsertBills[i];
            temp.Purchaseprice = 1;

            temp.FkArticles[1].Articlename = "aaaaaaaaa";
            temp.FkArticles[1].Ishidden = !temp.FkArticles[1].Ishidden;

            _dbContext.Update(temp);
            _dbContext.Update(temp.FkArticles[1]);
        }
    }
}