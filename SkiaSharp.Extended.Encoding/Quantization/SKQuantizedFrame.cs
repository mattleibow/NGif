namespace SkiaSharp.Extended.Encoding
{
	public class SKQuantizedFrame
	{
		public byte[] Palette { get; set; }

		public byte[] IndexedPixels { get; set; }

		public int ColorDepth { get; set; }

		public int PaletteSize { get; set; }
	}
}
