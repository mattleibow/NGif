using SkiaSharp;
using SkiaSharp.Extended.Encoding;
using System.IO;

namespace Example
{
	class ExampleMain
	{
		static void Main(string[] args)
		{
			//var imageFilePaths = new[] { "Res/01.png", "Res/02.png", "Res/03.png" };
			var imageFilePaths = new[] { "Res/sticker.png" };

			SKGifEncoderOptions options = new SKGifEncoderOptions
			{
				RepeatCount = 0,
				Quantizer = new SKNeuQuantizer(),
			};

			using var stream = File.Create("output.gif");
			using var encoder = new SKGifEncoder(stream, options);

			for (int i = 0, count = imageFilePaths.Length; i < count; i++)
			{
				using var image = SKImage.FromEncodedData(imageFilePaths[i]);
				var frameInfo = new SKGifEncoderFrameInfo
				{
					Duration = 500,
					TransparentColor = SKColor.Empty
				};
				encoder.AddFrame(image, frameInfo);
			}
		}
	}
}
