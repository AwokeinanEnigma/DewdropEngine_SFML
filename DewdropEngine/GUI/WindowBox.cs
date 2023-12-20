
 using DewDrop.Graphics;
 using DewDrop.Resources;
 using SFML.Graphics;
 using DewDrop.Utilities;
 using SFML.Graphics.Glsl; 
 
 // ReSharper disable StringLiteralTypo
 // ReSharper disable SuggestVarOrType_SimpleTypes
 namespace DewDrop.GUI {
	 /// <summary>
	 /// Represents a window box that can be rendered on the screen.
	 /// </summary>
	 public class WindowBox : Renderable {
		 /// <summary>
		 /// Gets or sets the currently displayed position of the WindowBox.
		 /// </summary>
		 public override Vector2 RenderPosition {
			 get {
				 return _position;
			 }
			 set {
				 _position = value;
				 ConfigureTransform();
			 }
		 }

		 /// <summary>
		 /// Gets or sets the origin of the WindowBox.
		 /// </summary>
		 public override Vector2 Origin {
			 get {
				 return _origin;
			 }
			 set {
				 _origin = value;
				 ConfigureQuads();
			 }
		 }

		 /// <summary>
		 /// Gets or sets the size of the WindowBox.
		 /// </summary>
		 public override Vector2 Size {
			 get {
				 return _size;
			 }
			 set {
				 _size = value;
				 ConfigureQuads();
			 }
		 }

		 /// <summary>
		 /// Gets or sets the frame style of the WindowBox.
		 /// This should be a string pointing to a .gdat file.
		 /// </summary>
		 public string FrameStyle {
			 get {
				 return _style;
			 }
			 set {
				 SetStyle(value);
			 }
		 }

		 /// <summary>
		 /// Gets or sets the palette of the WindowBox.
		 /// </summary>
		 public uint Palette {
			 get {
				 return _palette;
			 }
			 set {
				 _palette = value;
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

		 /// <summary>
		 /// Initializes a new instance of the WindowBox class with specified frame style, palette, position, size, and depth.
		 /// </summary>
		 /// <param name="window">The window of the WindowBox.</param>
		 /// <param name="palette">The palette of the WindowBox.</param>
		 /// <param name="position">The position of the WindowBox.</param>
		 /// <param name="size">The size of the WindowBox.</param>
		 /// <param name="depth">The depth of the WindowBox.</param>
		 public WindowBox (string window, uint palette, Vector2 position, Vector2 size, int depth) {
			 _style = window;
			 _palette = palette;
			 _position = position;
			 _size = size;
			 _depth = depth;
			 _visible = true;
			 SetStyle(window);
		 }

		 /// <summary>
		 /// Sets the style of the WindowBox.
		 /// </summary>
		 /// <param name="newStyle">The new style of the WindowBox.</param>
		 void SetStyle (string newStyle) {
			 _style = newStyle;
			 _beamRepeat = false;
			 _frame = new SpriteGraphic(_style, "center", _position, _depth);
			 _frame.CurrentPalette = _palette;
			 _shader = new Shader(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

			 _shader.SetUniform("image", Shader.CurrentTexture);
			 _shader.SetUniform("palette", _frame.Spritesheet.Palette);
			 _shader.SetUniform("palIndex", _frame.Spritesheet.CurrentPaletteFloat);
			 _shader.SetUniform("palSize", _frame.Spritesheet.PaletteSize);
			 _shader.SetUniform("blend", new Vec4(Color.White));
			 _shader.SetUniform("blendMode", 1f);

			 _states = new RenderStates(BlendMode.Alpha, _transform, _frame.Texture.Image, _shader);
			 _verts = new VertexArray(PrimitiveType.Quads);
			 ConfigureQuads();
			 ConfigureTransform();
		 }

		 /// <summary>
		 /// Configures the transform of the WindowBox.
		 /// </summary>
		 void ConfigureTransform () {
			 _transform = new Transform(1f, 0f, _position.X, 0f, 1f, _position.Y, 0f, 0f, 1f);
			 _states.Transform = _transform;
		 }

		 /// <summary>
		 /// Configures the quads of the WindowBox.
		 /// </summary>
		 void ConfigureQuads () {
			 SpriteDefinition topLeftDef = _frame.GetSpriteDefinition("topleft");
			 SpriteDefinition topRightDef = _frame.GetSpriteDefinition("topright");
			 SpriteDefinition bottomLeftDef = _frame.GetSpriteDefinition("bottomleft");
			 SpriteDefinition bottomRightDef = _frame.GetSpriteDefinition("bottomright");
			 SpriteDefinition topDef = _frame.GetSpriteDefinition("top");
			 SpriteDefinition bottomDef = _frame.GetSpriteDefinition("bottom");
			 SpriteDefinition leftDef = _frame.GetSpriteDefinition("left");
			 SpriteDefinition rightDef = _frame.GetSpriteDefinition("right");
			 SpriteDefinition centerDef = _frame.GetSpriteDefinition("center");

			 Vector2 topLeftBounds = topLeftDef.Bounds;
			 Vector2 topDefBounds = topDef.Bounds;
			 Vector2 topRightBounds = topRightDef.Bounds;
			 Vector2 rightDefBounds = rightDef.Bounds;
			 Vector2 bottomRightDefBounds = bottomRightDef.Bounds;
			 Vector2 bottomBounds = bottomDef.Bounds;
			 Vector2 bottomleftBounds = bottomLeftDef.Bounds;
			 Vector2 leftDefBounds = leftDef.Bounds;
			 Vector2 centerBounds = centerDef.Bounds;

			 int topWidth = (int)_size.X - (int)topLeftBounds.X - (int)topRightBounds.X;
			 int bottomWidth = (int)_size.X - (int)bottomleftBounds.X - (int)bottomRightDefBounds.X;
			 int rightWidth = (int)_size.Y - (int)topRightBounds.Y - (int)bottomRightDefBounds.Y;
			 int leftWidth = (int)_size.Y - (int)topLeftBounds.Y - (int)bottomleftBounds.Y;

			 _verts.Clear();

			 Vector2 defaultVector = default(Vector2);

			 Vector2 pointer = defaultVector;
			 _verts.Append(new Vertex(pointer, new Vector2(topLeftDef.Coords.X, topLeftDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(topLeftBounds.X, 0f), new Vector2((topLeftDef.Coords.X + topLeftBounds.X), topLeftDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(topLeftBounds.X, topLeftBounds.Y), new Vector2((topLeftDef.Coords.X + topLeftBounds.X), (topLeftDef.Coords.Y + topLeftBounds.Y))));
			 _verts.Append(new Vertex(pointer + new Vector2(0f, topLeftBounds.Y), new Vector2(topLeftDef.Coords.X, (topLeftDef.Coords.Y + topLeftBounds.Y))));

			 pointer += new Vector2((topWidth + topLeftBounds.X), 0f);
			 _verts.Append(new Vertex(pointer, new Vector2(topRightDef.Coords.X, topRightDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(topRightBounds.X, 0f), new Vector2((topRightDef.Coords.X + topRightBounds.X), topRightDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(topRightBounds.X, topRightBounds.Y), new Vector2((topRightDef.Coords.X + topRightBounds.X), (topRightDef.Coords.Y + topRightBounds.Y))));
			 _verts.Append(new Vertex(pointer + new Vector2(0f, topRightBounds.Y), new Vector2(topRightDef.Coords.X, (topRightDef.Coords.Y + topRightBounds.Y))));

			 pointer += new Vector2(0f, (rightWidth + topRightBounds.Y));
			 _verts.Append(new Vertex(pointer, new Vector2(bottomRightDef.Coords.X, bottomRightDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(bottomRightDefBounds.X, 0f), new Vector2((bottomRightDef.Coords.X + bottomRightDefBounds.X), bottomRightDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(bottomRightDefBounds.X, bottomRightDefBounds.Y), new Vector2((bottomRightDef.Coords.X + bottomRightDefBounds.X), (bottomRightDef.Coords.Y + bottomRightDefBounds.Y))));
			 _verts.Append(new Vertex(pointer + new Vector2(0f, bottomRightDefBounds.Y), new Vector2(bottomRightDef.Coords.X, (bottomRightDef.Coords.Y + bottomRightDefBounds.Y))));

			 pointer -= new Vector2((bottomWidth + bottomleftBounds.X), 0f);
			 _verts.Append(new Vertex(pointer, new Vector2(bottomLeftDef.Coords.X, bottomLeftDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(bottomleftBounds.X, 0f), new Vector2((bottomLeftDef.Coords.X + bottomleftBounds.X), bottomLeftDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(bottomleftBounds.X, bottomleftBounds.Y), new Vector2((bottomLeftDef.Coords.X + bottomleftBounds.X), (bottomLeftDef.Coords.Y + bottomleftBounds.Y))));
			 _verts.Append(new Vertex(pointer + new Vector2(0f, bottomleftBounds.Y), new Vector2(bottomLeftDef.Coords.X, (bottomLeftDef.Coords.Y + bottomleftBounds.Y))));

			 pointer += new Vector2(leftDefBounds.X, (-leftWidth));
			 _verts.Append(new Vertex(pointer, new Vector2(centerDef.Coords.X, centerDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(topWidth, 0f), new Vector2((centerDef.Coords.X + centerBounds.X), centerDef.Coords.Y)));
			 _verts.Append(new Vertex(pointer + new Vector2(topWidth, leftWidth), new Vector2((centerDef.Coords.X + centerBounds.X), (centerDef.Coords.Y + centerBounds.Y))));
			 _verts.Append(new Vertex(pointer + new Vector2(0f, leftWidth), new Vector2(centerDef.Coords.X, (centerDef.Coords.Y + centerBounds.Y))));

			 if (!_beamRepeat) {
				 pointer = defaultVector;
				 pointer += new Vector2(topLeftBounds.X, 0f);
				 _verts.Append(new Vertex(pointer, new Vector2(topDef.Coords.X, topDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(topWidth, 0f), new Vector2((topDef.Coords.X + topDefBounds.X), topDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(topWidth, topDefBounds.Y), new Vector2((topDef.Coords.X + topDefBounds.X), (topDef.Coords.Y + topDefBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, topDefBounds.Y), new Vector2(topDef.Coords.X, (topDef.Coords.Y + topDefBounds.Y))));

				 pointer = defaultVector;
				 pointer += new Vector2((topLeftBounds.X + topWidth), topRightBounds.Y);
				 _verts.Append(new Vertex(pointer, new Vector2(rightDef.Coords.X, rightDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(rightDefBounds.X, 0f), new Vector2((rightDef.Coords.X + rightDefBounds.X), rightDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(topRightBounds.X, rightWidth), new Vector2((rightDef.Coords.X + rightDefBounds.X), (rightDef.Coords.Y + rightDefBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, rightWidth), new Vector2(rightDef.Coords.X, (rightDef.Coords.Y + rightDefBounds.Y))));

				 pointer = defaultVector;
				 pointer += new Vector2(bottomleftBounds.X, (topLeftBounds.Y + leftWidth));
				 _verts.Append(new Vertex(pointer, new Vector2(bottomDef.Coords.X, bottomDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(bottomWidth, 0f), new Vector2((bottomDef.Coords.X + bottomBounds.X), bottomDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(bottomWidth, bottomBounds.Y), new Vector2((bottomDef.Coords.X + bottomBounds.X), (bottomDef.Coords.Y + bottomBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, bottomBounds.Y), new Vector2(bottomDef.Coords.X, (bottomDef.Coords.Y + bottomBounds.Y))));

				 pointer = defaultVector;
				 pointer += new Vector2(0f, topLeftBounds.Y);
				 _verts.Append(new Vertex(pointer, new Vector2(leftDef.Coords.X, leftDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(leftDefBounds.X, 0f), new Vector2((leftDef.Coords.X + leftDefBounds.X), leftDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(leftDefBounds.X, leftWidth), new Vector2((leftDef.Coords.X + leftDefBounds.X), (leftDef.Coords.Y + leftDefBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, leftWidth), new Vector2(leftDef.Coords.X, (leftDef.Coords.Y + leftDefBounds.Y))));
				 return;
			 }

			 int num5 = (int)(topWidth/topDefBounds.X);
			 int num6 = (int)(topWidth%topDefBounds.X);
			 pointer = defaultVector + new Vector2(topLeftBounds.X, 0f);
			 for (int i = 0; i < num5; i++) {
				 _verts.Append(new Vertex(pointer, new Vector2(topDef.Coords.X, topDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(topDefBounds.X, 0f), new Vector2((topDef.Coords.X + topDefBounds.X), topDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(topDefBounds.X, topDefBounds.Y), new Vector2((topDef.Coords.X + topDefBounds.X), (topDef.Coords.Y + topDefBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, topDefBounds.Y), new Vector2(topDef.Coords.X, (topDef.Coords.Y + topDefBounds.Y))));
				 pointer += new Vector2(topDefBounds.X, 0f);
			 }
			 if (num6 != 0) {
				 _verts.Append(new Vertex(pointer, new Vector2(topDef.Coords.X, topDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(num6, 0f), new Vector2((topDef.Coords.X + num6), topDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(num6, topDefBounds.Y), new Vector2((topDef.Coords.X + num6), (topDef.Coords.Y + topDefBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, topDefBounds.Y), new Vector2(topDef.Coords.X, (topDef.Coords.Y + topDefBounds.Y))));
			 }

			 int num7 = (int)(bottomWidth/bottomBounds.X);
			 int num8 = (int)(bottomWidth%bottomBounds.X);

			 pointer = defaultVector + new Vector2(topLeftBounds.X, (topLeftBounds.Y + leftWidth));
			 for (int j = 0; j < num7; j++) {
				 _verts.Append(new Vertex(pointer, new Vector2(bottomDef.Coords.X, bottomDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(bottomBounds.X, 0f), new Vector2((bottomDef.Coords.X + bottomBounds.X), bottomDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(bottomBounds.X, bottomBounds.Y), new Vector2((bottomDef.Coords.X + bottomBounds.X), (bottomDef.Coords.Y + bottomBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, bottomBounds.Y), new Vector2(bottomDef.Coords.X, (bottomDef.Coords.Y + bottomBounds.Y))));
				 pointer += new Vector2(bottomBounds.X, 0f);
			 }
			 if (num8 != 0) {
				 _verts.Append(new Vertex(pointer, new Vector2(bottomDef.Coords.X, bottomDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(num8, 0f), new Vector2((bottomDef.Coords.X + num8), bottomDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(num8, bottomBounds.Y), new Vector2((bottomDef.Coords.X + num8), (bottomDef.Coords.Y + bottomBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, bottomBounds.Y), new Vector2(bottomDef.Coords.X, (bottomDef.Coords.Y + bottomBounds.Y))));
			 }

			 int num9 = (int)(leftWidth/leftDefBounds.Y);
			 int num10 = (int)(leftWidth%leftDefBounds.Y);

			 pointer = defaultVector + new Vector2(0f, topLeftBounds.Y);
			 for (int k = 0; k < num9; k++) {
				 _verts.Append(new Vertex(pointer, new Vector2(leftDef.Coords.X, leftDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(leftDefBounds.X, 0f), new Vector2((leftDef.Coords.X + leftDefBounds.X), leftDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(leftDefBounds.X, leftDefBounds.Y), new Vector2((leftDef.Coords.X + leftDefBounds.X), (leftDef.Coords.Y + leftDefBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, leftDefBounds.Y), new Vector2(leftDef.Coords.X, (leftDef.Coords.Y + leftDefBounds.Y))));
				 pointer += new Vector2(0f, leftDefBounds.Y);
			 }

			 if (num10 != 0) {
				 _verts.Append(new Vertex(pointer, new Vector2(leftDef.Coords.X, leftDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(leftDefBounds.X, 0f), new Vector2((leftDef.Coords.X + leftDefBounds.X), leftDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(leftDefBounds.X, num10), new Vector2((leftDef.Coords.X + leftDefBounds.X), (leftDef.Coords.Y + num10))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, num10), new Vector2(leftDef.Coords.X, (leftDef.Coords.Y + num10))));
			 }

			 int num11 = (int)(rightWidth/rightDefBounds.Y);
			 int num12 = (int)(rightWidth%rightDefBounds.Y);

			 pointer = defaultVector + new Vector2((topLeftBounds.X + topWidth), topLeftBounds.Y);
			 for (int l = 0; l < num11; l++) {
				 _verts.Append(new Vertex(pointer, new Vector2(rightDef.Coords.X, rightDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(rightDefBounds.X, 0f), new Vector2((rightDef.Coords.X + rightDefBounds.X), rightDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(rightDefBounds.X, rightDefBounds.Y), new Vector2((rightDef.Coords.X + rightDefBounds.X), (rightDef.Coords.Y + rightDefBounds.Y))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, rightDefBounds.Y), new Vector2(rightDef.Coords.X, (rightDef.Coords.Y + rightDefBounds.Y))));
				 pointer += new Vector2(0f, rightDefBounds.Y);
			 }
			 if (num12 != 0) {
				 _verts.Append(new Vertex(pointer, new Vector2(rightDef.Coords.X, rightDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(rightDefBounds.X, 0f), new Vector2((rightDef.Coords.X + rightDefBounds.X), rightDef.Coords.Y)));
				 _verts.Append(new Vertex(pointer + new Vector2(rightDefBounds.X, num12), new Vector2((rightDef.Coords.X + rightDefBounds.X), (rightDef.Coords.Y + num12))));
				 _verts.Append(new Vertex(pointer + new Vector2(0f, num12), new Vector2(rightDef.Coords.X, (rightDef.Coords.Y + num12))));
			 }
		 }

		 /// <summary>
		 /// Draws the WindowBox.
		 /// </summary>
		 /// <param name="target">The render target on which to draw.</param>
		 public override void Draw (RenderTarget target) {
			 target.Draw(_verts, _states);
		 }

		 /// <summary>
		 /// Disposes of the WindowBox and its resources.
		 /// </summary>
		 /// <param name="disposing">Indicates whether the WindowBox is currently being disposed.</param>
		 protected override void Dispose (bool disposing) {
			 if (!_disposed && disposing) {
				 _verts.Dispose();
				 _frame.Dispose();
			 }
			 _disposed = true;
		 }
	 }
 }