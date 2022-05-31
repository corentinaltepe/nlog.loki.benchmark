# nlog.loki.benchmark

This repository benchmarks the client libraries NLog.Targets.Loki (HTTP) and NLog.Targets.Loki.gRPC. Below are the results with:

- NLog.Targets.Loki v1.4.3
- NLog.Targets.Loki.gRPC v0.1.0-preview6

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
AMD Ryzen 7 2700, 1 CPU, 16 logical and 8 physical cores
.NET SDK=6.0.300
  [Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
  Job-BZISFB : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT

Runtime=.NET 6.0  Toolchain=net60  InvocationCount=1  
UnrollFactor=1  
```

| Method | NumberLogs |       Mean |      Error |     StdDev |     Median |     Gen 0 |     Gen 1 | Allocated |
|------- |----------- |-----------:|-----------:|-----------:|-----------:|----------:|----------:|----------:|
|   **Grpc** |          **4** |   **1.642 ms** |  **0.0798 ms** |  **0.2157 ms** |   **1.604 ms** |         **-** |         **-** |     **16 KB** |
|   Http |          4 |   2.141 ms |  0.0866 ms |  0.2511 ms |   2.060 ms |         - |         - |     12 KB |
|   **Grpc** |         **12** |   **1.691 ms** |  **0.0943 ms** |  **0.2533 ms** |   **1.623 ms** |         **-** |         **-** |     **25 KB** |
|   Http |         12 |   2.198 ms |  0.0676 ms |  0.1918 ms |   2.155 ms |         - |         - |     21 KB |
|   **Grpc** |        **100** |   **2.320 ms** |  **0.1839 ms** |  **0.4908 ms** |   **2.187 ms** |         **-** |         **-** |    **127 KB** |
|   Http |        100 |   3.109 ms |  0.0959 ms |  0.2704 ms |   3.023 ms |         - |         - |    120 KB |
|   **Grpc** |       **1000** |  **34.435 ms** |  **7.9829 ms** | **23.5379 ms** |  **52.973 ms** |         **-** |         **-** |  **1,209 KB** |
|   Http |       1000 |  18.424 ms |  0.6830 ms |  1.9924 ms |  18.200 ms |         - |         - |  1,154 KB |
|   **Grpc** |      **10000** | **270.539 ms** | **16.6350 ms** | **47.1909 ms** | **251.887 ms** | **2000.0000** | **1000.0000** | **12,067 KB** |
|   Http |      10000 | 162.423 ms |  3.2316 ms |  4.0870 ms | 161.381 ms | 2000.0000 | 1000.0000 | 11,517 KB |

## Limitations

This benchmark only looks at the client side (the application generating and pushing the logs to Loki), but not at the server side (Loki), nor the networking constraints (run on localhost). While it looks like the HTTP client may be more efficient to push logs to Loki, it doesn't say if it is any easier for the network or the server to implement either gRPC or HTTP.