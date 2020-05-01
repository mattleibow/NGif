using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using ImageMagick;
using SkiaSharp;
using SkiaSharp.Extended.Encoding;
using System;
using System.IO;
using System.Linq;

namespace Benchmarks
{
	//[SimpleJob(RuntimeMoniker.CoreRt31)]
	//[SimpleJob(RuntimeMoniker.Mono)]
	//[SimpleJob(RuntimeMoniker.Net472)]
	[SimpleJob(RuntimeMoniker.NetCoreApp31)]
	public class Benchmark : IDisposable
	{
		private SKBitmap[] animated;
		private SKBitmap[] sticker;
		private SKImage[] stickerImage;
		private byte[] stickerPixels;

		private SKQuantizer approx = new SKApproximateGrayscaleQuantizer();
		private SKQuantizer digital = new SKDigitalGrayscaleQuantizer();
		private SKQuantizer photo = new SKPhotometricGrayscaleQuantizer();
		private SKQuantizer neu = new SKNeuQuantizer();

		private MemoryStream stream = new MemoryStream();

		public Benchmark()
		{
			animated = new[] { "Res/01.png", "Res/02.png", "Res/03.png" }.Select(SKBitmap.Decode).ToArray();
			sticker = new[] { SKBitmap.Decode("Res/sticker.png") };
			stickerImage = new[] { SKImage.FromEncodedData("Res/sticker.png") };
			stickerPixels = sticker[0].GetPixelSpan().ToArray();
		}

		public void Dispose()
		{
			stream.Dispose();
			stream = null;
		}

		// small, animated, color

		[Benchmark]
		public void ApproxAnimated() => CreateGif(approx, animated);

		[Benchmark]
		public void DigitalAnimated() => CreateGif(digital, animated);

		[Benchmark]
		public void PhotoAnimated() => CreateGif(photo, animated);

		[Benchmark]
		public void NeuAnimated() => CreateGif(neu, animated);

		// large, gray, static

		[Benchmark]
		public void ApproxSticker() => CreateGif(approx, sticker);

		[Benchmark]
		public void DigitalSticker() => CreateGif(digital, sticker);

		[Benchmark]
		public void PhotoSticker() => CreateGif(photo, sticker);

		[Benchmark]
		public void NeuSticker() => CreateGif(neu, sticker);

		// image

		[Benchmark]
		public void ApproxStickerImage() => CreateGif(approx, stickerImage);

		// ImageMagick

		[Benchmark]
		public void MagickSticker()
		{
			using var mi = new MagickImage();
			mi.ReadPixels(stickerPixels, new PixelReadSettings(sticker[0].Width, sticker[0].Height, StorageType.Char, PixelMapping.ABGR));
			mi.Format = MagickFormat.Gif;
			mi.Write(stream);
		}

		//

		private void CreateGif(SKQuantizer quantizer, SKBitmap[] images)
		{
			SKGifEncoderOptions options = new SKGifEncoderOptions
			{
				RepeatCount = 0,
				Quantizer = quantizer,
			};

			using var encoder = new SKGifEncoder(stream, options);

			foreach (var image in images)
			{
				var frameInfo = new SKGifEncoderFrameInfo
				{
					Duration = 500,
					TransparentColor = SKColor.Empty
				};
				encoder.AddFrame(image, frameInfo);
			}
		}

		private void CreateGif(SKQuantizer quantizer, SKImage[] images)
		{
			SKGifEncoderOptions options = new SKGifEncoderOptions
			{
				RepeatCount = 0,
				Quantizer = quantizer,
			};

			using var encoder = new SKGifEncoder(stream, options);

			foreach (var image in images)
			{
				var frameInfo = new SKGifEncoderFrameInfo
				{
					Duration = 500,
					TransparentColor = SKColor.Empty
				};
				encoder.AddFrame(image, frameInfo);
			}
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<Benchmark>();
		}
	}
}
