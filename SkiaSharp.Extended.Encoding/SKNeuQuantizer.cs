using System;

namespace SkiaSharp.Extended.Encoding
{
	public class SKNeuQuantizer : SKQuantizer
	{
		private NeuQuant neuQuant;

		public SKNeuQuantizer(int samplingFactor = 10)
		{
			SamplingFactor = samplingFactor;
		}

		public int SamplingFactor { get; }

		public override SKQuantizedFrame Quantize(ReadOnlySpan<SKColor> colors)
		{
			var pixels = new byte[3 * colors.Length];

			for (int i = 0, k = 0; i < colors.Length; i++)
			{
				var c = colors[i];
				pixels[k++] = c.Red;
				pixels[k++] = c.Green;
				pixels[k++] = c.Blue;
			}

			int len = pixels.Length;
			int nPix = len / 3;

			neuQuant = new NeuQuant(len, SamplingFactor);
			var palette = neuQuant.Process(pixels);

			var frame = new SKQuantizedFrame();
			frame.ColorDepth = 8;
			frame.PaletteSize = 7;
			frame.IndexedPixels = new byte[nPix];
			frame.Palette = palette;
			for (int i = 0, k = 0; i < nPix; i++)
			{
				int index = neuQuant.Map(
					pixels[k++] & 0xff,
					pixels[k++] & 0xff,
					pixels[k++] & 0xff);
				frame.IndexedPixels[i] = (byte)index;
			}

			return frame;
		}

		public override byte GetColorIndex(SKColor color)
		{
			if (neuQuant == null)
				throw new InvalidOperationException("There was no frame.");

			return (byte)neuQuant.Map(color.Blue, color.Green, color.Red);
		}
	}
}
