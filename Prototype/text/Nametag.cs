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
		get => nameText.Text;
		set => SetName(value);
	}

	public Nametag(string nameString, Vector2 position, int depth)
	{
		_position = position;
		_depth = depth;
		this.nameText = new TextRenderer(_position + Nametag.TEXT_POSITION, _depth + 1, new FontData(), nameString);
		this.left = new SpriteGraphic(Nametag.RESOURCE_NAME, "left", _position, _depth);
		this.center = new SpriteGraphic(Nametag.RESOURCE_NAME, "center", this.left.RenderPosition + new Vector2(this.left.Size.X, 0f), _depth);
		this.center.Scale = new Vector2(this.nameText.Size.X + 2f, 1f);
		this.right = new SpriteGraphic(Nametag.RESOURCE_NAME, "right", this.center.RenderPosition + new Vector2(this.nameText.Size.X + 2f, 0f), _depth);
		this.CalculateSize();
	}

	private void Reposition(Vector2 newPosition)
	{
		_position = newPosition;
		this.nameText.RenderPosition = _position + Nametag.TEXT_POSITION;
		this.left.RenderPosition = _position;
		this.center.RenderPosition = this.left.RenderPosition + new Vector2(this.left.Size.X, 0f);
		this.right.RenderPosition = this.center.RenderPosition + new Vector2(this.nameText.Size.X + 2f, 0f);
	}

	private void SetName(string newName)
	{
		this.nameText.Reset(newName, 0, newName.Length);
		this.center.Scale = new Vector2(this.nameText.Size.X + 2f, 1f);
		this.right.RenderPosition = this.center.RenderPosition + new Vector2(this.nameText.Size.X + 2f, 0f);
		this.CalculateSize();
	}

	private void CalculateSize()
	{
		Size = new Vector2(this.left.Size.X + this.nameText.Size.X + 2f + this.right.Size.X, this.left.Size.Y);
	}

	public override void Draw(RenderTarget target)
	{
		this.left.Draw(target);
		this.center.Draw(target);
		this.right.Draw(target);
		this.nameText.Draw(target);
	}

	protected override void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			this.left.Dispose();
			this.center.Dispose();
			this.right.Dispose();
			this.nameText.Dispose();
		}
		base.Dispose(disposing);
	}

	private const string LEFT_SPRITE_NAME = "left";

	private const string CENTER_SPRITE_NAME = "center";

	private const string RIGHT_SPRITE_NAME = "right";

	private const int MARGIN = 2;

	private const string RESOURCE_NAME = "nametag.dat";

	static readonly Vector2 TEXT_POSITION = new Vector2(4f, -4f);

	private SpriteGraphic left;

	private SpriteGraphic center;

	private SpriteGraphic right;

	private TextRenderer nameText;
}