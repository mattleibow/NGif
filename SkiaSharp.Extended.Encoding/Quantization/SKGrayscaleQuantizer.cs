using System;

namespace SkiaSharp.Extended.Encoding
{
	public abstract class SKGrayscaleQuantizer : SKQuantizer
	{
		private static readonly byte[] palette = new byte[256 * 3];

		static SKGrayscaleQuantizer()
		{
			for (int i = 0, k = 0; i <= byte.MaxValue; i++)
			{
				palette[k++] = (byte)i;
				palette[k++] = (byte)i;
				palette[k++] = (byte)i;
			}
		}

		public override SKQuantizedFrame Quantize(ReadOnlySpan<SKColor> pixels)
		{
			var indexed = new byte[pixels.Length];
			for (int i = 0; i < indexed.Length; i++)
			{
				indexed[i] = GetColorIndex(pixels[i]);
			}

			return new SKQuantizedFrame
			{
				Palette = palette,
				IndexedPixels = indexed,
				ColorDepth = 8,
				PaletteSize = 7,
			};
		}
	}

	public class SKDigitalGrayscaleQuantizer : SKGrayscaleQuantizer
	{
		public override byte GetColorIndex(SKColor color) =>
			(byte)(0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue); // Digital: ITU BT.601
	}

	public class SKPhotometricGrayscaleQuantizer : SKGrayscaleQuantizer
	{
		public override byte GetColorIndex(SKColor color) =>
			(byte)(0.2126 * color.Red + 0.7152 * color.Green + 0.0722 * color.Blue); // Photometric/digital: ITU BT.709
	}

	public class SKApproximateGrayscaleQuantizer : SKGrayscaleQuantizer
	{
		public override byte GetColorIndex(SKColor color)
		{
			byte r = color.Red;
			byte g = color.Green;
			byte b = color.Blue;

			int gray = (r * 3 + g * 4 + b) >> 3;

			return (byte)gray;
		}
	}
}
