# nGif
.NET advanced GIF library

This repo is a fork of NGif-Animated-GIF-Encoder-for-NET by gOODiDEA.NET. He originally ported it from Java but it contained a few bugs and minor annoyances. This fork can animate GIFs frame by frame and supports writing to memory or file streams.

The original implementation also had bugs with transparency appearing as black as well as the last two pixels rendering as the wrong color, but these have been fixed.

See the original project here:
http://www.codeproject.com/Articles/11505/NGif-Animated-GIF-Encoder-for-NET

## Benchmarks

```
// * Summary *

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i9-9980HK CPU 2.40GHz, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4150.0), X64 RyuJIT
  Job-MNUBEA : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT

Runtime=.NET Core 3.1

|          Method |       Mean |     Error |    StdDev |
|---------------- |-----------:|----------:|----------:|
|  ApproxAnimated |   2.495 ms | 0.0236 ms | 0.0209 ms |
| DigitalAnimated |   2.707 ms | 0.0389 ms | 0.0345 ms |
|   PhotoAnimated |   2.662 ms | 0.0447 ms | 0.0373 ms |
|     NeuAnimated |  26.615 ms | 0.3884 ms | 0.3633 ms |
|   ApproxSticker |   9.481 ms | 0.1404 ms | 0.1314 ms |
|  DigitalSticker |  10.391 ms | 0.1978 ms | 0.2278 ms |
|    PhotoSticker |  10.389 ms | 0.2031 ms | 0.1899 ms |
|      NeuSticker | 124.949 ms | 1.4172 ms | 1.3256 ms |

// * Hints *
Outliers
  Benchmark.ApproxAnimated: Runtime=.NET Core 3.1  -> 1 outlier  was  removed (2.73 ms)
  Benchmark.DigitalAnimated: Runtime=.NET Core 3.1 -> 1 outlier  was  removed (3.00 ms)
  Benchmark.PhotoAnimated: Runtime=.NET Core 3.1   -> 2 outliers were removed (2.80 ms, 2.86 ms)

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 ms   : 1 Millisecond (0.001 sec)
```