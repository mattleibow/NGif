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
|     ApproxAnimated |   2.455 ms | 0.0359 ms | 0.0318 ms |
|    DigitalAnimated |   2.612 ms | 0.0453 ms | 0.0402 ms |
|      PhotoAnimated |   2.657 ms | 0.0320 ms | 0.0267 ms |
|        NeuAnimated |  28.318 ms | 0.5590 ms | 0.5741 ms |
|      ApproxSticker |   9.916 ms | 0.0763 ms | 0.0596 ms |
|     DigitalSticker |  10.634 ms | 0.2001 ms | 0.2055 ms |
|       PhotoSticker |  10.645 ms | 0.2034 ms | 0.1903 ms |
|         NeuSticker | 129.872 ms | 1.7166 ms | 1.6057 ms |
| ApproxStickerImage |  10.542 ms | 0.1995 ms | 0.1959 ms |
|      MagickSticker |  67.693 ms | 0.7761 ms | 0.7260 ms |
|        NGifSticker | 343.996 ms | 4.2268 ms | 3.9537 ms |

// * Hints *
Outliers
  Benchmark.ApproxAnimated: Runtime=.NET Core 3.1  -> 1 outlier  was  removed (2.95 ms)
  Benchmark.DigitalAnimated: Runtime=.NET Core 3.1 -> 1 outlier  was  removed (2.87 ms)
  Benchmark.PhotoAnimated: Runtime=.NET Core 3.1   -> 2 outliers were removed (2.75 ms, 3.03 ms)
  Benchmark.NeuAnimated: Runtime=.NET Core 3.1     -> 1 outlier  was  removed (31.61 ms)
  Benchmark.ApproxSticker: Runtime=.NET Core 3.1   -> 3 outliers were removed (10.24 ms..10.41 ms)

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 ms   : 1 Millisecond (0.001 sec)
```