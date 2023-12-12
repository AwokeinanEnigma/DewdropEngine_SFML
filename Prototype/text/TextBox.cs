
 using DewDrop.Entities;
 using DewDrop.Graphics;
 using DewDrop.GUI;
 using DewDrop.GUI.Fonts;
 using DewDrop.UserInput;
using Mother4.Scripts.Text;
using SFML.Graphics;
using DewDrop.Utilities;
 using SFML.Window;
 namespace Mother4.GUI
{
	public class TextBox : Entity
	{
		public event TextBox.CompletionHandler OnTextboxComplete;

		public event TextBox.TypewriterCompletionHandler OnTypewriterComplete;

		public bool Visible
		{
			get
			{
				return this.visible;
			}
		}

		public TextBox(RenderPipeline pipeline, int colorIndex)
		{
			this.pipeline = pipeline;
			this.state = TextBox.AnimationState.SlideIn;
			this.topLetterboxY = -14f;
			this.bottomLetterboxY = 180f;
			//this.dimmer = new ScreenDimmer(this.pipeline, Color.Transparent, 0, 2147450879);
			Vector2 finalCenter = ViewManager.Instance.FinalCenter;
			Vector2 Vector2 = finalCenter - ViewManager.Instance.View.Size / 2f;
			Vector2 size = new Vector2(320f, 14f);
			this.topLetterbox = new ShapeRenderer(new RectangleShape(size), new Vector2(0f, this.topLetterboxY), new Vector2(0f, 0f), size, 2147450880);
			this.topLetterbox.Shape.FillColor = Color.Black;
			this.topLetterbox.Visible = false;
			this.pipeline.Add(this.topLetterbox);
			this.bottomLetterbox = new ShapeRenderer(new RectangleShape(size), new Vector2(0f, this.bottomLetterboxY), new Vector2(0f, 0f), size, 2147450878);
			this.bottomLetterbox.Shape.FillColor = Color.Black;
			this.bottomLetterbox.Visible = false;
			this.pipeline.Add(this.bottomLetterbox);
			this.typewriterBox = new TypewriterBox(pipeline, new Vector2(Vector2.X + TextBox.TEXT_POSITION.X, Vector2.Y + TextBox.TEXT_POSITION.Y), TextBox.TEXT_SIZE, 2147450880, DButtons.Select, true, new TextBlock(new List<TextLine>()));
			this.window = new WindowBox("window.gdat", 0,new Vector2(Vector2.X + TextBox.BOX_POSITION.X, Vector2.Y + TextBox.BOX_POSITION.Y), TextBox.BOX_SIZE, 2147450879);
			this.window.Visible = false;
			this.pipeline.Add(this.window);
			this.advanceArrow = new SpriteGraphic("realcursor.dat", "down", new Vector2(Vector2.X + TextBox.BUTTON_POSITION.X, Vector2.Y + TextBox.BUTTON_POSITION.Y), 2147450880);
			this.advanceArrow.Visible = false;
			this.pipeline.Add(this.advanceArrow);
			this.nametag = new Nametag(string.Empty, new Vector2(Vector2.X + TextBox.NAMETAG_POSITION.X, Vector2.Y + TextBox.NAMETAG_POSITION.Y), 2147450880);
			this.nametag.Visible = false;
			this.pipeline.Add(this.nametag);
			this.visible = false;
			this.nametagVisible = false;
			this.arrowVisible = false;
			this.hideAdvanceArrow = false;
			this.typewriterBox.OnTypewriterComplete += this.TypewriterComplete;
			this.typewriterBox.OnTextWait += this.TextWait;
			Input.OnButtonPressed += this.ButtonPressed;
			ViewManager.Instance.OnMove += this.OnViewMove;
		}

		public void ClearTypewriterComplete()
		{
			this.OnTypewriterComplete = null;
		}

		protected virtual void TextWait()
		{
			this.textboxWaitForPlayer = true;
			if (!this.hideAdvanceArrow)
			{
				this.arrowVisible = true;
				this.advanceArrow.Visible = this.arrowVisible;
			}
		}

		protected virtual void TypewriterComplete()
		{
			this.textboxWaitForPlayer = true;
			this.typewriterDone = true;
			if (!this.hideAdvanceArrow)
			{
				this.arrowVisible = true;
				this.advanceArrow.Visible = this.arrowVisible;
			}
			if (this.OnTypewriterComplete != null)
			{
				this.OnTypewriterComplete();
			}
		}

		protected virtual void ButtonPressed(object? o, DButtons b)
		{
			if (this.textboxWaitForPlayer && b == DButtons.Select)
			{
				this.textboxWaitForPlayer = false;
				if (!this.typewriterDone)
				{
					this.typewriterBox.ContinueFromWait();
					if (!this.hideAdvanceArrow)
					{
						this.arrowVisible = false;
						this.advanceArrow.Visible = this.arrowVisible;
						return;
					}
				}
				else if (this.OnTextboxComplete != null)
				{
					this.OnTextboxComplete();
				}
			}
		}

		protected virtual void Recenter()
		{
			Vector2 finalCenter = ViewManager.Instance.FinalCenter;
			Vector2 Vector2 = finalCenter - ViewManager.Instance.View.Size / 2f;
			_position = new Vector2(Vector2.X + TextBox.BOX_POSITION.X, Vector2.Y + TextBox.BOX_POSITION.Y);
			this.window.RenderPosition = new Vector2(_position.X, _position.Y + this.textboxY);
			this.advanceArrow.RenderPosition = new Vector2(Vector2.X + TextBox.BUTTON_POSITION.X, Vector2.Y + TextBox.BUTTON_POSITION.Y);
			this.nametag.RenderPosition = new Vector2(Vector2.X + TextBox.NAMETAG_POSITION.X, Vector2.Y + TextBox.NAMETAG_POSITION.Y + this.textboxY);
			this.typewriterBox.Reposition(new Vector2(Vector2.X + TextBox.TEXT_POSITION.X, Vector2.Y + TextBox.TEXT_POSITION.Y + this.textboxY));
			this.topLetterbox.RenderPosition = new Vector2(Vector2.X +BOX_POSITION.X, Vector2.Y + this.topLetterboxY + TextBox.BOX_POSITION.Y);
			this.bottomLetterbox.RenderPosition = new Vector2(Vector2.X + BOX_POSITION.X, Vector2.Y + this.bottomLetterboxY+ TextBox.BOX_POSITION.Y);
		}

		public virtual void Reset(string text, string namestring, bool suppressSlideIn, bool suppressSlideOut)
		{
			this.canTransitionIn = !suppressSlideIn;
			this.canTransitionOut = !suppressSlideOut;
			this.typewriterBox.Reset(TextProcessor.Process(new FontData(), text, (int)TextBox.TEXT_SIZE.X));
			this.arrowVisible = false;
			this.textboxWaitForPlayer = false;
			this.typewriterDone = false;
			if (namestring != null && namestring.Length > 0)
			{
				this.nametag.Name = namestring;
				this.nametagVisible = true;
			}
			else
			{
				this.nametagVisible = false;
			}
			this.nametag.Visible = this.nametagVisible;
			this.window.FrameStyle = "window.gdat";
		}

		private void UpdateLetterboxing(float amount)
		{
			float num = Math.Max(0f, Math.Min(1f, amount));
			this.topLetterboxY = (float)((int)(-14f * (1f - num)));
			this.bottomLetterboxY = (float)(180L - (long)((int)(14f * num)));
			this.topLetterbox.RenderPosition = new Vector2(ViewManager.Instance.Viewrect.Left, ViewManager.Instance.Viewrect.Top + this.topLetterboxY);
			this.bottomLetterbox.RenderPosition= new Vector2(ViewManager.Instance.Viewrect.Left, ViewManager.Instance.Viewrect.Top + this.bottomLetterboxY);
		}

		private void UpdateTextbox(float amount)
		{
			this.textboxY = 4f * (1f - Math.Max(0f, Math.Min(1f, amount)));
			this.typewriterBox.Position = new Vector2((float)((int)(ViewManager.Instance.Viewrect.Left + TextBox.TEXT_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + TextBox.TEXT_POSITION.Y + this.textboxY)));
			this.window.RenderPosition = new Vector2((float)((int)(ViewManager.Instance.Viewrect.Left + TextBox.BOX_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + TextBox.BOX_POSITION.Y + this.textboxY)));
			this.nametag.RenderPosition = new Vector2((float)((int)(ViewManager.Instance.Viewrect.Left + TextBox.NAMETAG_POSITION.X)), (float)((int)(ViewManager.Instance.Viewrect.Top + TextBox.NAMETAG_POSITION.Y + this.textboxY)));
		}

		public virtual void Show()
		{
			if (!this.visible)
			{
				this.visible = true;
				this.Recenter();
				this.window.Visible = true;
				this.topLetterbox.Visible = true;
				this.bottomLetterbox.Visible = true;
				this.typewriterBox.Show();
				this.nametag.Visible = this.nametagVisible;
				this.advanceArrow.Visible = this.arrowVisible;
				this.state = TextBox.AnimationState.SlideIn;
				this.letterboxProgress = (this.canTransitionIn ? 0f : 1f);
				this.UpdateLetterboxing(this.letterboxProgress);
			}
		}

		public virtual void Hide()
		{
			if (this.visible)
			{
				this.visible = false;
				this.Recenter();
				this.advanceArrow.Visible = false;
				this.state = TextBox.AnimationState.SlideOut;
				this.letterboxProgress = (this.canTransitionOut ? 1f : 0f);
				this.UpdateLetterboxing(this.letterboxProgress);
				this.UpdateTextbox(this.letterboxProgress * 2f);
			}
		}

		public void SetDimmer(float dim)
		{
		//	this.dimmer.ChangeColor(new Color(0, 0, 0, (byte)(255f * dim)), 30);
		}

		private void OnViewMove(ViewManager sender, Vector2 center)
		{
			if (this.visible || this.letterboxProgress > 0f)
			{
				this.Recenter();
			}
		}

		private float TweenLetterboxing(float progress)
		{
			return (float)(0.5 - Math.Cos((double)progress * 3.141592653589793) / 2.0);
		}

		public override string Name => "ok actually textbox";
		public override void Update()
		{
			base.Update();
			switch (this.state)
			{
			case TextBox.AnimationState.SlideIn:
				if (this.letterboxProgress < 1f)
				{
					this.UpdateLetterboxing(this.TweenLetterboxing(this.letterboxProgress));
					this.UpdateTextbox(this.letterboxProgress);
					this.letterboxProgress += 0.2f;
				}
				else
				{
					this.state = TextBox.AnimationState.Textbox;
					this.UpdateLetterboxing(1f);
					this.UpdateTextbox(1f);
				}
				break;
			case TextBox.AnimationState.Textbox:
				if (this.visible)
				{
					this.typewriterBox.Update();
				}
				break;
			case TextBox.AnimationState.SlideOut:
				if (this.letterboxProgress > 0f)
				{
					this.UpdateLetterboxing(this.TweenLetterboxing(this.letterboxProgress));
					this.UpdateTextbox(this.letterboxProgress);
					this.letterboxProgress -= 0.2f;
				}
				else
				{
					this.state = TextBox.AnimationState.Hide;
					this.UpdateLetterboxing(0f);
					this.UpdateTextbox(0f);
					this.typewriterBox.Hide();
					this.nametag.Visible = false;
					this.window.Visible = false;
					this.topLetterbox.Visible = false;
					this.bottomLetterbox.Visible = false;
				}
				break;
			}
		//	this.dimmer.Update();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.advanceArrow.Dispose();
			this.window.Dispose();
			this.nametag.Dispose();
			this.topLetterbox.Dispose();
			this.bottomLetterbox.Dispose();
			this.typewriterBox.OnTypewriterComplete -= this.TypewriterComplete;
			this.typewriterBox.OnTextWait -= this.TextWait;
			Input.OnButtonPressed -= this.ButtonPressed;
			ViewManager.Instance.OnMove -= this.OnViewMove;
			this.typewriterBox.Dispose();
		}

		protected const DButtons ADVANCE_BUTTON = DButtons.Select;

		protected const float LETTERBOX_HEIGHT = 14f;

		protected const float LETTERBOX_SPEED = 0.2f;

		protected const float TEXTBOX_Y_OFFSET = 4f;

		public const int DEPTH = 2147450880;

		protected static Vector2 BOX_POSITION = new Vector2(-20, 120f);

		protected static Vector2 BOX_SIZE = new Vector2(231f, 56f);

		protected static Vector2 TEXT_POSITION = new Vector2(TextBox.BOX_POSITION.X + 10f, TextBox.BOX_POSITION.Y + 8f);

		protected static Vector2 TEXT_SIZE = new Vector2(TextBox.BOX_SIZE.X - 31f, TextBox.BOX_SIZE.Y - 8f);

		protected static Vector2 NAMETAG_POSITION = new Vector2(TextBox.BOX_POSITION.X + 3f, TextBox.BOX_POSITION.Y - 14f);

		protected static Vector2 NAMETEXT_POSITION = new Vector2(TextBox.NAMETAG_POSITION.X + 5f, TextBox.NAMETAG_POSITION.Y + 1f);

		protected static Vector2 BUTTON_POSITION = new Vector2(TextBox.BOX_POSITION.X + TextBox.BOX_SIZE.X - 14f, TextBox.BOX_POSITION.Y + TextBox.BOX_SIZE.Y - 6f);

		protected RenderPipeline pipeline;

		protected TypewriterBox typewriterBox;

		protected Nametag nametag;

		protected Graphic advanceArrow;

		private WindowBox window;

		private ShapeRenderer topLetterbox;

		private ShapeRenderer bottomLetterbox;

		protected bool visible;

		protected bool nametagVisible;

		protected bool arrowVisible;

		protected bool textboxWaitForPlayer;

		protected bool hideAdvanceArrow;

		protected bool typewriterDone;

		private TextBox.AnimationState state;

		private float letterboxProgress;

		private float topLetterboxY;

		private float bottomLetterboxY;

		private float textboxY;

		private bool canTransitionIn;

		private bool canTransitionOut;

		//private ScreenDimmer dimmer;

		private enum AnimationState
		{
			SlideIn,
			Textbox,
			SlideOut,
			Hide
		}

		public delegate void CompletionHandler();

		public delegate void TypewriterCompletionHandler();
	}
}