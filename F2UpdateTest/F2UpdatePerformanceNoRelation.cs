﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM.Core;
using ORM.Postgres.Extensions;
using ORM2TestEntiteis;
using NBuilderHelper = ORM2TestEntiteis.NBuilderHelper;

namespace F2UpdateTest;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter(fileNameSuffix:"",indentJson:true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]

public class F2UpdatePerformanceNoRelation : F2GlobalConfig.F2GlobalConfig
{
    public IEnumerable<int> valuesForNumberOfStatements() => NumberOfCustomers;
    
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
            listInsertCustomers = NBuilderHelper.GetListOfCustomerInformation1To1(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertCustomers;
            SetUpGenericF2();
            listInsertCustomers = tempList;
            for (int i = 0; i < listInsertCustomers.Count; i++)
            {
                customers temp = listInsertCustomers[i];
                temp.email = "aaaaaaaaa";
                temp.customerlikescolorgreen = !temp.customerlikescolorgreen;
                temp.lastname = "aaaaaaaaa";
            }
        }
        
        ResetCache();
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            _dbContext.Save(listInsertCustomers[i]);
        }
        ResetCache();
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanCustomers();
    }
    
    [Benchmark]
    public void UpdateData()
    {
        for (int i = 0; i < listInsertCustomers.Count; i++)
        {
            customers temp = listInsertCustomers[i];
            temp.email = "aaaaaaaaa";
            temp.customerlikescolorgreen = !temp.customerlikescolorgreen;
            temp.lastname = "aaaaaaaaa";
            
            _dbContext.Save(temp);
        }
    }
}