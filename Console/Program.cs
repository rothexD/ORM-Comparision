// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using F0DeleteTests;
using F0Inserttests;
using F0SelectTests;
using F0UpdateTest;
using F1DeleteTests;
using F1InsertTests;
using F1SelectTests;
using F1UpdateTEst;
using F2DeleteTests;
using F2InsertTests;
using F2SelectTests;
using F2UpdateTest;
using F3DeleteTests;
using F3InsertTests;
using F3SelectTests;
using F3UpdateTest;



var x = Environment.GetCommandLineArgs();

if (x.Length < 2) throw new InvalidOperationException();
var s = x[1];

switch (s)
{
    case "1":
        _ = BenchmarkRunner.Run<F0InsertPerformance1To1>();
        return;
    case "2":
        _ = BenchmarkRunner.Run<F1InsertPerformance1To1>();
        return;
    case "3":
        _ = BenchmarkRunner.Run<F2InsertPerformance1To1>();
        return;
    case "4":
        _ = BenchmarkRunner.Run<F3InsertPerformance1To1>();
        return;
    case "58":
        _ = BenchmarkRunner.Run<F2SelectPerformance1To1>();
        return;
    case "59":
        _ = BenchmarkRunner.Run<F0SelectPerformance1To1>();
        return;
    case "60":
        return;
        _ = BenchmarkRunner.Run<F3SelectPerformance1To1>();
        return;
    case "61":
        _ = BenchmarkRunner.Run<F1SelectPerformance1To1>();
        return;
    case "5":
        _ = BenchmarkRunner.Run<F0UpdatePerformance1To1>();
        return;
    case "6":
        _ = BenchmarkRunner.Run<F1UpdatePerformance1To1>();
        return;
    case "7":
        _ = BenchmarkRunner.Run<F2UpdatePerformance1To1>();
        return;
    case "8":
        _ = BenchmarkRunner.Run<F3UpdatePerformance1To1>();
        return;
    case "13":
        _ = BenchmarkRunner.Run<F0DeletePerformance1To1>();
        return;
    case "14":
        _ = BenchmarkRunner.Run<F1DeletePerformance1To1>();
        return;
    case "15":
        _ = BenchmarkRunner.Run<F2DeletePerformance1To1>();
        return;
    case "16":
        _ = BenchmarkRunner.Run<F3DeletePerformance1To1>();
        return;
    case "9":
        _ = BenchmarkRunner.Run<F0InsertPerformance1ToN>();
        return;
    case "10":
        _ = BenchmarkRunner.Run<F1InsertPerformance1ToN>();
        return;
    case "11":
        _ = BenchmarkRunner.Run<F2InsertPerformance1ToN>();
        return;
    case "12":
        _ = BenchmarkRunner.Run<F3InsertPerformance1ToN>();
        return;
    case "25":
        _ = BenchmarkRunner.Run<F0SelectPerformance1ToN>();
        return;
    case "26":
        _ = BenchmarkRunner.Run<F1SelectPerformance1ToN>();
        return;
    case "27":
        _ = BenchmarkRunner.Run<F2SelectPerformance1ToN>();
        return;
    case "28":
        return; // broken
        _ = BenchmarkRunner.Run<F3SelectPerformance1ToN>();
        return;
    case "41":
        _ = BenchmarkRunner.Run<F0UpdatePerformance1ToN>();
        return;
    case "42":
        _ = BenchmarkRunner.Run<F1UpdatePerformance1ToN>();
        return;
    case "43":
        _ = BenchmarkRunner.Run<F2UpdatePerformance1ToN>();
        return;
    case "45":
        _ = BenchmarkRunner.Run<F3UpdatePerformance1ToN>();
        return;
    case "48":
        _ = BenchmarkRunner.Run<F0DeletePerformance1ToN>();
        return;
    case "49":
        _ = BenchmarkRunner.Run<F1DeletePerformance1ToN>();
        return;
    case "50":
        _ = BenchmarkRunner.Run<F2DeletePerformance1ToN>();
        return;
    case "51":
        _ = BenchmarkRunner.Run<F3DeletePerformance1ToN>();
        return;
    case "17":
        _ = BenchmarkRunner.Run<F0InsertPerformanceNoRelation>();
        return;
    case "18":
        _ = BenchmarkRunner.Run<F1InsertPerformanceNoRelation>();
        return;
    case "19":
        _ = BenchmarkRunner.Run<F2InsertPerformanceNoRelation>();
        return;
    case "20":
        _ = BenchmarkRunner.Run<F3InsertPerformanceNoRelation>();
        return;
    case "21":
        _ = BenchmarkRunner.Run<F0SelectPerformanceNoRelation>();
        return;
    case "22":
        _ = BenchmarkRunner.Run<F1SelectPerformanceNoRelation>();
        return;
    case "23":
        _ = BenchmarkRunner.Run<F2SelectPerformanceNoRelation>();
        return;
    case "24":
        _ = BenchmarkRunner.Run<F3SelectPerformanceNoRelation>();
        return;
    case "29":
        _ = BenchmarkRunner.Run<F0UpdatePerformanceNoRelation>();
        return;
    case "30":
        _ = BenchmarkRunner.Run<F1UpdatePerformanceNoRelation>();
        return;
    case "31":
        _ = BenchmarkRunner.Run<F2UpdatePerformanceNoRelation>();
        return;
    case "32":
        _ = BenchmarkRunner.Run<F3UpdatePerformanceNoRelation>();
        return;
    case "37":
        _ = BenchmarkRunner.Run<F0DeletePerformanceNoRelation>();
        return;
    case "38":
        _ = BenchmarkRunner.Run<F1DeletePerformanceNoRelation>();
        return;
    case "39":
        _ = BenchmarkRunner.Run<F2DeletePerformanceNoRelation>();
        return;
    case "40":
        _ = BenchmarkRunner.Run<F3DeletePerformanceNoRelation>();
        return;
    case "46":
        _ = BenchmarkRunner.Run<F0UpdatePerformanceNToM>();
        return;
    case "47":
        _ = BenchmarkRunner.Run<F1UpdatePerformanceNToM>();
        return;
    case "33":
        _ = BenchmarkRunner.Run<F2InsertPerformanceNToM>();
        return;
    case "52":
        _ = BenchmarkRunner.Run<F0InsertPerforamnceNToM>();
        return;
    case "53":
        _ = BenchmarkRunner.Run<F1InsertPerformanceNToM>();
        return;
    case "54":
        _ = BenchmarkRunner.Run<F0DeletePerformanceNToM>();
        return;
    case "55":
        _ = BenchmarkRunner.Run<F1DeletePerformanceNToM>();
        return;
    case "56":
        _ = BenchmarkRunner.Run<F0SelectPerformanceNToM>();
        return;
    case "57":
        _ = BenchmarkRunner.Run<F1SelectPerformanceNToM>();
        return;
    case "34":
        _ = BenchmarkRunner.Run<F2DeletePerformanceNToM>();
        return;
    case "35":
        return; //broken
        _ = BenchmarkRunner.Run<F2SelectPerformanceNToM>();
        return;
    case "36":
        _ = BenchmarkRunner.Run<F2UpdatePerformanceNToM>();
        return;
    default:
        return;
}