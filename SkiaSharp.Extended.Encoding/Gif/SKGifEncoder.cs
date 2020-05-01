using System;
using System.IO;

namespace SkiaSharp.Extended.Encoding
{
	public class SKGifEncoder : IDisposable
	{
		private Stream stream;
		private bool firstFrame;
		private SKSizeI frameSize;

		private SKGifEncoderOptions options;
		private SKQuantizer quantizer;

		public SKGifEncoder(Stream stream, SKGifEncoderOptions options = default)
		{
			this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this.options = options;

			quantizer = options.Quantizer ?? new SKNeuQuantizer();
			firstFrame = true;
			frameSize = SKSizeI.Empty;

			WriteString("GIF89a"); // header
		}

		public void AddFrame(SKPixmap im, SKGifEncoderFrameInfo frameInfo = default)
		{
			using var bmp = new SKBitmap();
			bmp.InstallPixels(im);

			AddFrame(bmp, frameInfo);
		}

		public void AddFrame(SKImage im, SKGifEncoderFrameInfo frameInfo = default)
		{
			// make syre we have the pixels
			var raster = im.ToRasterImage(true);

			try
			{
				using var pixmap = raster.PeekPixels();

				AddFrame(pixmap, frameInfo);
			}
			finally
			{
				if (raster != im)
					raster.Dispose();
			}
		}

		public void AddFrame(SKBitmap im, SKGifEncoderFrameInfo frameInfo = default)
		{
			if (stream == null)
				throw new InvalidOperationException("File is already completed.");

			if (im == null)
				throw new ArgumentNullException(nameof(im));

			if (!firstFrame && im.Info.Size != frameSize)
				throw new InvalidOperationException("Image size does not match the first frame.");

			var quantizedFrame = quantizer.Quantize(im.Pixels);

			if (firstFrame)
			{
				frameSize = im.Info.Size;

				WriteLogicalScreenDescriptor(quantizedFrame);
				WritePalette(quantizedFrame);
				WriteNetscapeExtension();
			}

			WriteGraphicControlExtension(quantizedFrame, frameInfo);
			WriteImageDescriptor(quantizedFrame);
			if (!firstFrame)
				WritePalette(quantizedFrame);
			WritePixels(quantizedFrame);

			firstFrame = false;
		}

		public void Dispose()
		{
			Flush();
		}

		public void Flush()
		{
			if (stream != null)
			{
				stream.WriteByte(0x3b); // gif trailer

				stream.Flush();
				stream = null;
			}
		}

		// Write*

		private void WriteGraphicControlExtension(SKQuantizedFrame quantizedFrame, SKGifEncoderFrameInfo frameInfo)
		{
			stream.WriteByte(0x21); // extension introducer
			stream.WriteByte(0xf9); // GCE label
			stream.WriteByte(4); // data block size

			int transp, disp;
			if (frameInfo.TransparentColor != SKColor.Empty)
			{
				transp = 1;
				disp = 2;
			}
			else
			{
				transp = 0;
				disp = 0;
			}
			if (frameInfo.DisposalMethod >= 0)
				disp = ((int)frameInfo.DisposalMethod) & 7; // user override
			disp <<= 2;

			// packed fields
			stream.WriteByte(Convert.ToByte(
				0 | // 1:3 reserved
				disp | // 4:6 disposal
				0 | // 7   user input - 0 = none
				transp)); // 8   transparency flag

			WriteShort(frameInfo.Duration / 10); // delay x 1/100 sec

			// transparent color index
			if (frameInfo.TransparentColor != SKColor.Empty)
				stream.WriteByte(quantizer.GetColorIndex(frameInfo.TransparentColor));
			else
				stream.WriteByte(0);

			stream.WriteByte(0); // block terminator
		}

		private void WriteImageDescriptor(SKQuantizedFrame quantizedFrame)
		{
			// image separator
			stream.WriteByte(0x2c);

			// x, y, w, h
			WriteShort(0);
			WriteShort(0);
			WriteShort(frameSize.Width);
			WriteShort(frameSize.Height);

			// packed fields
			if (firstFrame)
			{
				// no LCT  - GCT is used for first (or only) frame
				stream.WriteByte(0);
			}
			else
			{
				// specify normal LCT
				stream.WriteByte(Convert.ToByte(
					0x80 | // 1 local color table  1=yes
					0 | // 2 interlace - 0=no
					0 | // 3 sorted - 0=no
					0 | // 4-5 reserved
					quantizedFrame.PaletteSize)); // 6-8 size of color table
			}
		}

		private void WriteLogicalScreenDescriptor(SKQuantizedFrame quantizedFrame)
		{
			// logical screen size
			WriteShort(frameSize.Width);
			WriteShort(frameSize.Height);

			// packed fields
			stream.WriteByte(Convert.ToByte(
				0x80 | // 1   : global color table flag = 1 (gct used)
				0x70 | // 2-4 : color resolution = 7
				0x00 | // 5   : gct sort flag = 0
				quantizedFrame.PaletteSize)); // 6-8 : gct size

			stream.WriteByte(0); // background color index
			stream.WriteByte(0); // pixel aspect ratio - assume 1:1
		}

		private void WriteNetscapeExtension()
		{
			if (options.RepeatCount < 0)
				return;

			stream.WriteByte(0x21); // extension introducer
			stream.WriteByte(0xff); // app extension label
			stream.WriteByte(11); // block size
			WriteString("NETSCAPE2.0"); // app id + auth code
			stream.WriteByte(3); // sub-block size
			stream.WriteByte(1); // loop sub-block id
			WriteShort(options.RepeatCount); // loop count (extra iterations, 0=repeat forever)
			stream.WriteByte(0); // block terminator
		}

		private void WritePalette(SKQuantizedFrame quantizedFrame)
		{
			stream.Write(quantizedFrame.Palette, 0, quantizedFrame.Palette.Length);
			int n = (3 * 256) - quantizedFrame.Palette.Length;
			for (int i = 0; i < n; i++)
			{
				stream.WriteByte(0);
			}
		}

		private void WritePixels(SKQuantizedFrame quantizedFrame)
		{
			LZWEncoder encoder = new LZWEncoder(quantizedFrame);
			encoder.Encode(stream);
		}

		private void WriteShort(int value)
		{
			stream.WriteByte(Convert.ToByte(value & 0xff));
			stream.WriteByte(Convert.ToByte((value >> 8) & 0xff));
		}

		private void WriteString(string s)
		{
			char[] chars = s.ToCharArray();
			for (int i = 0; i < chars.Length; i++)
			{
				stream.WriteByte((byte)chars[i]);
			}
		}
	}
}
