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

|             Method |       Mean |     Error |    StdDev |
|------------------- |-----------:|----------:|----------:|
|     ApproxAnimated |   2.513 ms | 0.0292 ms | 0.0258 ms |
|    DigitalAnimated |   2.710 ms | 0.0354 ms | 0.0314 ms |
|      PhotoAnimated |   2.649 ms | 0.0514 ms | 0.0481 ms |
|        NeuAnimated |  26.609 ms | 0.2104 ms | 0.1968 ms |
|      ApproxSticker |   9.389 ms | 0.1875 ms | 0.1841 ms |
|     DigitalSticker |  10.500 ms | 0.1444 ms | 0.1351 ms |
|       PhotoSticker |  10.487 ms | 0.1950 ms | 0.1824 ms |
|         NeuSticker | 125.619 ms | 2.0152 ms | 1.8851 ms |
| ApproxStickerImage |  11.391 ms | 0.0828 ms | 0.0734 ms |
|      MagickSticker |  67.885 ms | 1.0467 ms | 0.8740 ms |

// * Hints *
Outliers
  Benchmark.ApproxAnimated: Runtime=.NET Core 3.1     -> 1 outlier  was  removed (2.83 ms)
  Benchmark.DigitalAnimated: Runtime=.NET Core 3.1    -> 1 outlier  was  removed (3.01 ms)
  Benchmark.PhotoAnimated: Runtime=.NET Core 3.1      -> 1 outlier  was  removed (3.08 ms)
  Benchmark.NeuAnimated: Runtime=.NET Core 3.1        -> 1 outlier  was  detected (26.11 ms)
  Benchmark.PhotoSticker: Runtime=.NET Core 3.1       -> 1 outlier  was  removed, 2 outliers were detected (10.06 ms, 11.10 ms)
  Benchmark.ApproxStickerImage: Runtime=.NET Core 3.1 -> 1 outlier  was  removed (12.07 ms)
  Benchmark.MagickSticker: Runtime=.NET Core 3.1      -> 2 outliers were removed (72.58 ms, 98.67 ms)

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 ms   : 1 Millisecond (0.001 sec)
```