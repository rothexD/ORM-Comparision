``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
12th Gen Intel Core i5-12600K, 1 CPU, 16 logical and 10 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-SCIIUE : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

InvocationCount=1  IterationCount=20  RunStrategy=Throughput  
UnrollFactor=1  WarmupCount=20  

```
|     Method | numberOfStatements |     Mean |   Error |  StdDev |
|----------- |------------------- |---------:|--------:|--------:|
| UpdateData |                500 | 471.4 ms | 4.99 ms | 5.75 ms |
