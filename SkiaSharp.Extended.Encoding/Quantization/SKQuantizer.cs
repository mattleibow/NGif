using System;

namespace SkiaSharp.Extended.Encoding
{
	public abstract class SKQuantizer : IDisposable
	{
		public abstract SKQuantizedFrame Quantize(ReadOnlySpan<SKColor> pixels);

		public abstract byte GetColorIndex(SKColor color);

		public virtual void Dispose()
		{
		}
	}
}
