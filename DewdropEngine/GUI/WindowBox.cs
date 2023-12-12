
 using DewDrop.Graphics;
 using DewDrop.Resources;
 using SFML.Graphics;
using SFML.System;
using DewDrop.Utilities;

namespace DewDrop.GUI
{
	public class WindowBox : Renderable
	{
	
		public override Vector2 RenderPosition
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				this.ConfigureTransform();
			}
		}

		public override Vector2 Origin
		{
			get
			{
				return _origin;
			}
			set
			{
				_origin = value;
				this.ConfigureQuads();
			}
		}

		public override Vector2 Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
				this.ConfigureQuads();
			}
		}

		public string FrameStyle
		{
			get
			{
				return this._style;
			}
			set
			{
				this.SetStyle(value);
			}
		}

		public uint Palette
		{
			get
			{
				return this._palette;
			}
			set
			{
				this._palette = value;
			}
		}
		
		bool _beamRepeat;
		uint _palette;
		string _style;
		
		SpriteGraphic _frame;

		RenderStates _states;
		Transform _transform;
		VertexArray _verts;
		Shader _shader;
		

		public WindowBox(string window, uint palette, Vector2 position, Vector2 size, int depth)
		{
			this._style = window;
			this._palette = palette;
			_position = position;
			_size = size;
			_depth = depth;
			_visible = true;
			SetStyle(window);
		}

		void SetStyle(string newStyle)
		{
			this._style = newStyle;
			this._beamRepeat = false;
			this._frame = new SpriteGraphic(_style, "center", _position, _depth);
			this._frame.CurrentPalette = this._palette;
			this._shader = new Shader(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));
			this._shader.SetParameter("image", this._frame.Texture.Image);
			this._shader.SetParameter("palette", _frame.Spritesheet.Palette);
			this._shader.SetParameter("palIndex", _frame.Spritesheet.CurrentPaletteFloat);
			this._shader.SetParameter("palSize", _frame.Spritesheet.PaletteSize);
			this._shader.SetParameter("blend", Color.White);
			this._shader.SetParameter("blendMode", 1f);
			this._states = new RenderStates(BlendMode.Alpha, this._transform, this._frame.Texture.Image, this._shader);
			this._verts = new VertexArray(PrimitiveType.Quads);
			this.ConfigureQuads();
			this.ConfigureTransform();
		}

		void ConfigureTransform()
		{
			this._transform = new Transform(1f, 0f, _position.X, 0f, 1f, _position.Y, 0f, 0f, 1f);
			this._states.Transform = this._transform;
		}

		void ConfigureQuads()
		{
			SpriteDefinition spriteDefinition = this._frame.GetSpriteDefinition("topleft");
			SpriteDefinition spriteDefinition2 = this._frame.GetSpriteDefinition("topright");
			SpriteDefinition spriteDefinition3 = this._frame.GetSpriteDefinition("bottomleft");
			SpriteDefinition spriteDefinition4 = this._frame.GetSpriteDefinition("bottomright");
			SpriteDefinition spriteDefinition5 = this._frame.GetSpriteDefinition("top");
			SpriteDefinition spriteDefinition6 = this._frame.GetSpriteDefinition("bottom");
			SpriteDefinition spriteDefinition7 = this._frame.GetSpriteDefinition("left");
			SpriteDefinition spriteDefinition8 = this._frame.GetSpriteDefinition("right");
			SpriteDefinition spriteDefinition9 = this._frame.GetSpriteDefinition("center");
			Vector2 bounds = spriteDefinition.Bounds;
			Vector2 bounds2 = spriteDefinition5.Bounds;
			Vector2 bounds3 = spriteDefinition2.Bounds;
			Vector2 bounds4 = spriteDefinition8.Bounds;
			Vector2 bounds5 = spriteDefinition4.Bounds;
			Vector2 bounds6 = spriteDefinition6.Bounds;
			Vector2 bounds7 = spriteDefinition3.Bounds;
			Vector2 bounds8 = spriteDefinition7.Bounds;
			Vector2 bounds9 = spriteDefinition9.Bounds;
			int num = (int)_size.X - (int)bounds.X - (int)bounds3.X;
			int num2 = (int)_size.X - (int)bounds7.X -(int) bounds5.X;
			int num3 = (int)_size.Y - (int)bounds3.Y - (int)bounds5.Y;
			int num4 = (int)_size.Y - (int)bounds.Y - (int)bounds7.Y;
			this._verts.Clear();
			Vector2 Vector2 = default(Vector2);
			Vector2 Vector22 = Vector2;
			this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition.Coords.X, spriteDefinition.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds.X, 0f), new Vector2((spriteDefinition.Coords.X + bounds.X), spriteDefinition.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds.X, bounds.Y), new Vector2((spriteDefinition.Coords.X + bounds.X), (spriteDefinition.Coords.Y + bounds.Y))));
			this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds.Y), new Vector2(spriteDefinition.Coords.X, (spriteDefinition.Coords.Y + bounds.Y))));
			Vector22 += new Vector2((num + bounds.X), 0f);
			this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition2.Coords.X, spriteDefinition2.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds3.X, 0f), new Vector2((spriteDefinition2.Coords.X + bounds3.X), spriteDefinition2.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds3.X, bounds3.Y), new Vector2((spriteDefinition2.Coords.X + bounds3.X), (spriteDefinition2.Coords.Y + bounds3.Y))));
			this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds3.Y), new Vector2(spriteDefinition2.Coords.X, (spriteDefinition2.Coords.Y + bounds3.Y))));
			Vector22 += new Vector2(0f, (num3 + bounds3.Y));
			this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition4.Coords.X, spriteDefinition4.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds5.X, 0f), new Vector2((spriteDefinition4.Coords.X + bounds5.X), spriteDefinition4.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds5.X, bounds5.Y), new Vector2((spriteDefinition4.Coords.X + bounds5.X), (spriteDefinition4.Coords.Y + bounds5.Y))));
			this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds5.Y), new Vector2(spriteDefinition4.Coords.X, (spriteDefinition4.Coords.Y + bounds5.Y))));
			Vector22 -= new Vector2((num2 + bounds7.X), 0f);
			this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition3.Coords.X, spriteDefinition3.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds7.X, 0f), new Vector2((spriteDefinition3.Coords.X + bounds7.X), spriteDefinition3.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(bounds7.X, bounds7.Y), new Vector2((spriteDefinition3.Coords.X + bounds7.X), (spriteDefinition3.Coords.Y + bounds7.Y))));
			this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds7.Y), new Vector2(spriteDefinition3.Coords.X, (spriteDefinition3.Coords.Y + bounds7.Y))));
			Vector22 += new Vector2(bounds8.X, (-num4));
			this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition9.Coords.X, spriteDefinition9.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(num, 0f), new Vector2((spriteDefinition9.Coords.X + bounds9.X), spriteDefinition9.Coords.Y)));
			this._verts.Append(new Vertex(Vector22 + new Vector2(num, num4), new Vector2((spriteDefinition9.Coords.X + bounds9.X), (spriteDefinition9.Coords.Y + bounds9.Y))));
			this._verts.Append(new Vertex(Vector22 + new Vector2(0f, num4), new Vector2(spriteDefinition9.Coords.X, (spriteDefinition9.Coords.Y + bounds9.Y))));
			if (!this._beamRepeat)
			{
				Vector22 = Vector2;
				Vector22 += new Vector2(bounds.X, 0f);
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num, 0f), new Vector2((spriteDefinition5.Coords.X + bounds2.X), spriteDefinition5.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num, bounds2.Y), new Vector2((spriteDefinition5.Coords.X + bounds2.X), (spriteDefinition5.Coords.Y + bounds2.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds2.Y), new Vector2(spriteDefinition5.Coords.X, (spriteDefinition5.Coords.Y + bounds2.Y))));
				Vector22 = Vector2;
				Vector22 += new Vector2((bounds.X + num), bounds3.Y);
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds4.X, 0f), new Vector2((spriteDefinition8.Coords.X + bounds4.X), spriteDefinition8.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds3.X, num3), new Vector2((spriteDefinition8.Coords.X + bounds4.X), (spriteDefinition8.Coords.Y + bounds4.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, num3), new Vector2(spriteDefinition8.Coords.X, (spriteDefinition8.Coords.Y + bounds4.Y))));
				Vector22 = Vector2;
				Vector22 += new Vector2(bounds7.X, (bounds.Y + num4));
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num2, 0f), new Vector2((spriteDefinition6.Coords.X + bounds6.X), spriteDefinition6.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num2, bounds6.Y), new Vector2((spriteDefinition6.Coords.X + bounds6.X), (spriteDefinition6.Coords.Y + bounds6.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds6.Y), new Vector2(spriteDefinition6.Coords.X, (spriteDefinition6.Coords.Y + bounds6.Y))));
				Vector22 = Vector2;
				Vector22 += new Vector2(0f, bounds.Y);
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds8.X, 0f), new Vector2((spriteDefinition7.Coords.X + bounds8.X), spriteDefinition7.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds8.X, num4), new Vector2((spriteDefinition7.Coords.X + bounds8.X), (spriteDefinition7.Coords.Y + bounds8.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, num4), new Vector2(spriteDefinition7.Coords.X, (spriteDefinition7.Coords.Y + bounds8.Y))));
				return;
			}
			int num5 = (int)(num / bounds2.X);
			int num6 = (int)(num % bounds2.X);
			Vector22 = Vector2 + new Vector2(bounds.X, 0f);
			for (int i = 0; i < num5; i++)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds2.X, 0f), new Vector2((spriteDefinition5.Coords.X + bounds2.X), spriteDefinition5.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds2.X, bounds2.Y), new Vector2((spriteDefinition5.Coords.X + bounds2.X), (spriteDefinition5.Coords.Y + bounds2.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds2.Y), new Vector2(spriteDefinition5.Coords.X, (spriteDefinition5.Coords.Y + bounds2.Y))));
				Vector22 += new Vector2(bounds2.X, 0f);
			}
			if (num6 != 0)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition5.Coords.X, spriteDefinition5.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num6, 0f), new Vector2((spriteDefinition5.Coords.X + num6), spriteDefinition5.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num6, bounds2.Y), new Vector2((spriteDefinition5.Coords.X + num6), (spriteDefinition5.Coords.Y + bounds2.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds2.Y), new Vector2(spriteDefinition5.Coords.X, (spriteDefinition5.Coords.Y + bounds2.Y))));
			}
			int num7 = (int)(num2 / bounds6.X);
			int num8 = (int)(num2 % bounds6.X);
			Vector22 = Vector2 + new Vector2(bounds.X, (bounds.Y + num4));
			for (int j = 0; j < num7; j++)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds6.X, 0f), new Vector2((spriteDefinition6.Coords.X + bounds6.X), spriteDefinition6.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds6.X, bounds6.Y), new Vector2((spriteDefinition6.Coords.X + bounds6.X), (spriteDefinition6.Coords.Y + bounds6.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds6.Y), new Vector2(spriteDefinition6.Coords.X, (spriteDefinition6.Coords.Y + bounds6.Y))));
				Vector22 += new Vector2(bounds6.X, 0f);
			}
			if (num8 != 0)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition6.Coords.X, spriteDefinition6.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num8, 0f), new Vector2((spriteDefinition6.Coords.X + num8), spriteDefinition6.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(num8, bounds6.Y), new Vector2((spriteDefinition6.Coords.X + num8), (spriteDefinition6.Coords.Y + bounds6.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds6.Y), new Vector2(spriteDefinition6.Coords.X, (spriteDefinition6.Coords.Y + bounds6.Y))));
			}
			int num9 = (int)(num4 / bounds8.Y);
			int num10 = (int)(num4 % bounds8.Y);
			Vector22 = Vector2 + new Vector2(0f, bounds.Y);
			for (int k = 0; k < num9; k++)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds8.X, 0f), new Vector2((spriteDefinition7.Coords.X + bounds8.X), spriteDefinition7.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds8.X, bounds8.Y), new Vector2((spriteDefinition7.Coords.X + bounds8.X), (spriteDefinition7.Coords.Y + bounds8.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds8.Y), new Vector2(spriteDefinition7.Coords.X, (spriteDefinition7.Coords.Y + bounds8.Y))));
				Vector22 += new Vector2(0f, bounds8.Y);
			}
			if (num10 != 0)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition7.Coords.X, spriteDefinition7.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds8.X, 0f), new Vector2((spriteDefinition7.Coords.X + bounds8.X), spriteDefinition7.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds8.X, num10), new Vector2((spriteDefinition7.Coords.X + bounds8.X), (spriteDefinition7.Coords.Y + num10))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, num10), new Vector2(spriteDefinition7.Coords.X, (spriteDefinition7.Coords.Y + num10))));
			}
			int num11 = (int)(num3 / bounds4.Y);
			int num12 = (int)(num3 % bounds4.Y);
			Vector22 = Vector2 + new Vector2((bounds.X + num), bounds.Y);
			for (int l = 0; l < num11; l++)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds4.X, 0f), new Vector2((spriteDefinition8.Coords.X + bounds4.X), spriteDefinition8.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds4.X, bounds4.Y), new Vector2((spriteDefinition8.Coords.X + bounds4.X), (spriteDefinition8.Coords.Y + bounds4.Y))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, bounds4.Y), new Vector2(spriteDefinition8.Coords.X, (spriteDefinition8.Coords.Y + bounds4.Y))));
				Vector22 += new Vector2(0f, bounds4.Y);
			}
			if (num12 != 0)
			{
				this._verts.Append(new Vertex(Vector22, new Vector2(spriteDefinition8.Coords.X, spriteDefinition8.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds4.X, 0f), new Vector2((spriteDefinition8.Coords.X + bounds4.X), spriteDefinition8.Coords.Y)));
				this._verts.Append(new Vertex(Vector22 + new Vector2(bounds4.X, num12), new Vector2((spriteDefinition8.Coords.X + bounds4.X), (spriteDefinition8.Coords.Y + num12))));
				this._verts.Append(new Vertex(Vector22 + new Vector2(0f, num12), new Vector2(spriteDefinition8.Coords.X, (spriteDefinition8.Coords.Y + num12))));
			}
		}

		public override void Draw(RenderTarget target)
		{
			target.Draw(this._verts, this._states);
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				this._verts.Dispose();
				this._frame.Dispose();
			}
			_disposed = true;
		}
	}
}