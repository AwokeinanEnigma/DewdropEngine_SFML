#region

using DewDrop.Resources;
using SFML.Graphics;

#endregion

namespace DewDrop.GUI.Fonts;

public class FontData : IDisposable {
	bool _disposed;

	public Font Font { get; }

	public int XCompensation { get; }

	public int YCompensation { get; }

	public int LineHeight { get; }

	public int WHeight { get; }

	public uint Size { get; }

	public float AlphaThreshold { get; }

	public FontData () {
		Font = new Font(EmbeddedResourcesHandler.GetResourceStream("openSansPX.ttf"));

		Size = 16U;
		WHeight = (int)Font.GetGlyph(41U, Size, false, 1).Bounds.Height;
		LineHeight = (int)(WHeight*1.20000004768372);
		AlphaThreshold = 0.0f;
	}

	public FontData (Font font, uint fontSize, int lineHeight, int xComp, int yComp) {
		Font = font;
		Size = fontSize;
		LineHeight = lineHeight;
		XCompensation = xComp;
		YCompensation = yComp;
		WHeight = (int)Font.GetGlyph(41U, Size, false, 1).Bounds.Height;
		// Console.WriteLine($"wHeight = {wHeight}");
		AlphaThreshold = 0.8f;
	}

	~FontData () {
		Dispose(false);
	}

	protected virtual void Dispose (bool disposing) {
		if (!_disposed && disposing) {
			Font.Dispose();
		}

		_disposed = true;
	}

	public void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
