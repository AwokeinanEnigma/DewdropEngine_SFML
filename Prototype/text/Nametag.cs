using DewDrop.Graphics;
 using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

namespace Mother4.GUI; 

public class Nametag : Renderable
{
	public override Vector2 RenderPosition
	{
		get => _position;
		set => Reposition(value);
	}

	public string Name
	{
		get => _nameText.Text;
		set => SetName(value);
	}

	public Nametag(string nameString, Vector2 position, int depth)
	{
		_position = position;
		_depth = depth;
		this._nameText = new TextRenderer(_position + Nametag._TextPosition, _depth + 1, new FontData(), nameString);
		this._left = new SpriteGraphic(Nametag.ResourceName, "left", _position, _depth);
		this._center = new SpriteGraphic(Nametag.ResourceName, "center", this._left.RenderPosition + new Vector2(this._left.Size.X, 0f), _depth);
		this._center.Scale = new Vector2(this._nameText.Size.X + 2f, 1f);
		this._right = new SpriteGraphic(Nametag.ResourceName, "right", this._center.RenderPosition + new Vector2(this._nameText.Size.X + 2f, 0f), _depth);
		this.CalculateSize();
	}

	private void Reposition(Vector2 newPosition)
	{
		_position = newPosition;
		this._nameText.RenderPosition = _position + Nametag._TextPosition;
		this._left.RenderPosition = _position;
		this._center.RenderPosition = this._left.RenderPosition + new Vector2(this._left.Size.X, 0f);
		this._right.RenderPosition = this._center.RenderPosition + new Vector2(this._nameText.Size.X + 2f, 0f);
	}

	private void SetName(string newName)
	{
		this._nameText.Reset(newName, 0, newName.Length);
		this._center.Scale = new Vector2(this._nameText.Size.X + 2f, 1f);
		this._right.RenderPosition = this._center.RenderPosition + new Vector2(this._nameText.Size.X + 2f, 0f);
		this.CalculateSize();
	}

	private void CalculateSize()
	{
		Size = new Vector2(this._left.Size.X + this._nameText.Size.X + 2f + this._right.Size.X, this._left.Size.Y);
	}

	public override void Draw(RenderTarget target)
	{
		this._left.Draw(target);
		this._center.Draw(target);
		this._right.Draw(target);
		this._nameText.Draw(target);
	}

	protected override void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			this._left.Dispose();
			this._center.Dispose();
			this._right.Dispose();
			this._nameText.Dispose();
		}
		base.Dispose(disposing);
	}
	
	private const string ResourceName = "nametag.dat";
	static readonly Vector2 _TextPosition = new Vector2(4f, -4f);
	private readonly SpriteGraphic _left;
	private readonly SpriteGraphic _center;
	private readonly SpriteGraphic _right;

	private readonly TextRenderer _nameText;
}