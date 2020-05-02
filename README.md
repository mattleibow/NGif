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
|     ApproxAnimated |   2.316 ms | 0.0348 ms | 0.0308 ms |
|    DigitalAnimated |   2.426 ms | 0.0456 ms | 0.0404 ms |
|      PhotoAnimated |   2.438 ms | 0.0439 ms | 0.0367 ms |
|        NeuAnimated |  27.972 ms | 0.5414 ms | 0.5793 ms |
|      ApproxSticker |   7.967 ms | 0.1548 ms | 0.1373 ms |
|     DigitalSticker |   8.750 ms | 0.1719 ms | 0.2466 ms |
|       PhotoSticker |   8.589 ms | 0.1628 ms | 0.2000 ms |
|         NeuSticker | 128.558 ms | 2.4605 ms | 2.6327 ms |
| ApproxStickerImage |   9.582 ms | 0.1887 ms | 0.2173 ms |
|      MagickSticker |  67.693 ms | 0.7761 ms | 0.7260 ms |
|        NGifSticker | 343.996 ms | 4.2268 ms | 3.9537 ms |
|      SystemSticker |  19.830 ms | 0.4101 ms | 0.7395 ms |

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 ms   : 1 Millisecond (0.001 sec)
```