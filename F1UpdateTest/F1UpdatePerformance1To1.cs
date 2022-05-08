using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using ORM1Entities;

namespace F1UpdateTEst;

[SimpleJob(RunStrategy.Throughput)]
[JsonExporter("", true)]
[RPlotExporter]
[CsvMeasurementsExporter]
[IterationCount(20)]
[WarmupCount(20)]
public class F1UpdatePerformance1To1 : F1GlobalConfig.F1GlobalConfig
{
    private int cache = -1;

    [ParamsSource(nameof(valuesForNumberOfStatements))]
    public int numberOfStatements;

    [GlobalSetup]
    public void GlobalSetup()
    {
        DefineDatabaseModelUsingScript(FilePath);
    }

    public IEnumerable<int> valuesForNumberOfStatements()
    {
        return NumberOfKnights;
    }

    [IterationSetup]
    public void SetUp()
    {
        if (cache != numberOfStatements)
        {
            SetUpGenericF1();
            listInsertKnight = NBuilderHelper.GetKnights(numberOfStatements);
            cache = numberOfStatements;
        }
        else
        {
            var tempList = listInsertKnight;
            SetUpGenericF1();
            listInsertKnight = tempList;
            for (var i = 0; i < listInsertKnight.Count; i++)
            {
                listInsertKnight[i].Name = "bbbbbbbb";
                listInsertKnight[i].Weapon.WeaponName = "bbbbbbbb";
                listInsertKnight[i].Weapon.Damage = i + i - 2;
            }
        }

        for (var i = 0; i < listInsertKnight.Count; i++)
        {
            var temp = listInsertKnight[i].Weapon;
            temp.Knight = null;
            _dbContext.Add(temp);
            _dbContext.Add(listInsertKnight[i]);
        }

        ResetCache();
    }

    [IterationCleanup]
    public void CleanUp()
    {
        CleanKnights();
    }

    [Benchmark]
    public void UpdateData()
    {
        for (var i = 0; i < listInsertKnight.Count; i++)
        {
            listInsertKnight[i].Name = "aaaaaaaa";
            listInsertKnight[i].Weapon.WeaponName = "aaaaaaaa";
            listInsertKnight[i].Weapon.Damage = i + i;

            _dbContext.Update(listInsertKnight[i]);
            _dbContext.Update(listInsertKnight[i].Weapon);
        }
    }
}