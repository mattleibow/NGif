namespace SkiaSharp.Extended.Encoding
{
	public struct SKGifEncoderFrameInfo
	{
		public int Duration { get; set; }

		public SKCodecAnimationDisposalMethod DisposalMethod { get; set; }

		public SKColor TransparentColor { get; set; }
	}
}
