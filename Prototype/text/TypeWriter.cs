﻿using DewDrop.Entities;
 using DewDrop.Graphics;
 using DewDrop.GUI;
 using DewDrop.GUI.Fonts;
using DewDrop.UserInput;
using DewDrop.Utilities;
using Mother4.Scripts.Text;
using SFML.Window;

 namespace Mother4.GUI
{
	public  class TypewriterBox : Entity
	{
		public event TypewriterBox.TypewriterCompleteHandler OnTypewriterComplete;

		public event TypewriterBox.WaitCommandHandler OnTextWait;

		public event TypewriterBox.TriggerCommandHandler OnTextTrigger;

		public bool UseBeeps
		{
			get
			{
				return this.useBeeps;
			}
			set
			{
				this.useBeeps = value;
			}
		}

		public int DisplayLines
		{
			get
			{
				return Math.Min(3, this.textBlock.Lines.Count);
			}
		}

		public override Vector2 Position
		{
			get
			{
				return _position;
			}
			set
			{
				this.Reposition(value);
			}
		}

		Vector2 _size;
		public TypewriterBox(RenderPipeline pipeline, Vector2 position, Vector2 size, int depth, DewDrop.UserInput.DButtons advance, bool showBullets, TextBlock textBlock)
		{
			this.pipeline = pipeline;
			_position = position;
			_size = size;
			this.depth = depth;
			this.advance = advance;
			this.showBullets = showBullets;
			this.textBlock = textBlock;
			this.origTextSpeed = 0.5f;
			this.textSpeed = this.origTextSpeed;
			int num = (int)(size.Y / 18);
			this.texts = new TextRenderer[num];
			for (int i = 0; i < this.texts.Length; i++)
			{
				this.texts[i] = new TextRenderer(_position + new Vector2(8f, 14 * i), this.depth + 1, new FontData(), (i < this.textBlock.Lines.Count) ? this.textBlock.Lines[i].Text : string.Empty, 0, 0);
				this.pipeline.Add(this.texts[i]);
			}
			this.SetUpBullets();
			this.topLineIndex = 0;
			this.currentLineIndex = 0;
			this.currentTextIndex = 0;
			this.currentText = this.texts[this.currentTextIndex];
			this.textPos = 0;
			this.textLen = this.currentText.Text.Length;
			this.useBeeps = true;
		}

		private void SetUpBullets()
		{
			if (this.showBullets)
			{
				if (this.bullets != null) {
					foreach (var bullet in this.bullets) {
						this.pipeline.Remove(bullet);
						bullet.Dispose();
					}
				}
				this.bulletVisibility = new bool[this.texts.Length];
				this.bullets = new Graphic[this.texts.Length];
				for (int i = 0; i < this.bullets.Length; i++)
				{
					this.bullets[i] = new SpriteGraphic("bullet.dat", "bullet", _position + new Vector2(8f, 26 * i), this.depth + 1);
					this.pipeline.Add(this.bullets[i]); 
					
				}
				
				this.SetBulletVisibility();
				return;
			}
			this.bullets = Array.Empty<Graphic>();
		}

		private void SetBulletVisibility()
		{
			for (int i = 0; i < this.bullets.Length; i++)
			{
				this.bulletVisibility[i] = (this.topLineIndex + i < this.textBlock.Lines.Count && this.texts[i].Length > 0 && this.textBlock.Lines[this.topLineIndex + i].HasBullet);
				this.bullets[i].Visible = (this.showBullets && this.bulletVisibility[i]);
			}
		}

		public void Reposition(Vector2 newPosition)
		{
			_position = Vector2.Truncate(newPosition);
			for (int i = 0; i < this.texts.Length; i++)
			{
				this.texts[i].RenderPosition = _position + new Vector2(8f, (float)(14 * i));
				if (this.showBullets)
				{
					this.bullets[i].RenderPosition = _position + new Vector2(1f, (float)(13 + 14 * i));
				}
			}
		}

		public void Reset(TextBlock textBlock)
		{
			this.textBlock = textBlock;
			this.topLineIndex = 0;
			this.currentLineIndex = 0;
			this.currentTextIndex = 0;
			this.currentText = this.texts[this.currentTextIndex];
			for (int i = 0; i < this.texts.Length; i++)
			{
				this.texts[i].Reset((i < this.textBlock.Lines.Count) ? this.textBlock.Lines[i].Text : string.Empty, 0, 0);
			}
			this.SetUpBullets();
			this.totalCharCount = 0f;
			this.pauseTimer = 0;
			this.pauseDuration = 0;
			this.commandIndex = 0;
			this.paused = false;
			this.waiting = false;
			this.nextCharWaiter = 0f;
			this.textPos = 0;
			this.textLen = this.currentText.Text.Length;
			this.finshed = false;
		}

		public void Show()
		{
			if (!this.visible)
			{
				this.visible = true;
				for (int i = 0; i < this.texts.Length; i++)
				{
					this.texts[i].Visible = true;
					if (this.showBullets)
					{
						this.bullets[i].Visible = this.bulletVisibility[i];
					}
				}
			}
		}

		public void Hide()
		{
			if (this.visible)
			{
				this.visible = false;
				for (int i = 0; i < this.texts.Length; i++)
				{
					this.texts[i].Visible = false;
					if (this.showBullets)
					{
						this.bullets[i].Visible = false;
					}
				}
			}
		}

		public void ContinueFromWait()
		{
			this.waiting = false;
		}


		public override string Name => "textbox lol";
		public override void Update()
		{
			
			if (!this.visible)
			{
				return;
			}
			this.textSpeed = this.origTextSpeed;
			if (Input.Instance[this.advance])
			{
				this.textSpeed = this.origTextSpeed + 0.5f;
			}	
			this.SetBulletVisibility();
			this.currentText.Length = Math.Min(this.textLen, this.textPos + 1);
			if (!this.waiting)
			{
				if (this.paused)
				{
					if (this.pauseTimer < this.pauseDuration)
					{
						this.pauseTimer++;
						return;
					}
					this.pauseTimer = 0;
					this.pauseDuration = 0;
					this.paused = false;
					return;
				}
				else if (!this.paused)
				{
					if (this.textPos < this.textLen)
					{
						this.nextCharWaiter += this.textSpeed;
						if (this.nextCharWaiter >= 1f)
						{
							int num = 0;
							int num2 = (int)this.nextCharWaiter;
							while (num < num2 && !this.paused)
							{
								this.textPos++;
								this.totalCharCount += 1f;
								this.HandleCommands();
								num++;
							}
							this.nextCharWaiter = 0f;
							if (this.useBeeps && this.totalCharCount % 3f == 0f)
							{
								return;
							}
						}
					}
					else if (!this.finshed)
					{
						this.HandleCommands();
						if (this.currentTextIndex < this.texts.Length - 1)
						{
							this.currentTextIndex++;
							this.currentText = this.texts[this.currentTextIndex];
							this.textPos = 0;
							this.commandIndex = 0;
							this.textLen = this.currentText.Text.Length;
							this.currentLineIndex++;
							return;
						}
						if (this.currentLineIndex < this.textBlock.Lines.Count - 1)
						{
							for (int i = 1; i < this.texts.Length; i++)
							{
								this.texts[i - 1].Reset(this.texts[i].Text, this.texts[i].Index, this.texts[i].Length);
							}
							this.topLineIndex++;
							this.currentLineIndex++;
							this.texts[this.currentTextIndex].Reset(this.textBlock.Lines[this.currentLineIndex].Text, 0, 0);
							this.currentText = this.texts[this.currentTextIndex];
							this.textPos = 0;
							this.commandIndex = 0;
							this.textLen = this.currentText.Text.Length;
							return;
						}
						if (this.OnTypewriterComplete != null)
						{
							this.OnTypewriterComplete();
						}
						this.finshed = true;
					}
				}
			}
		}

		private bool HandleCommands()
		{
			bool result = false;
			if (this.currentLineIndex < this.textBlock.Lines.Count && this.textBlock.Lines[this.currentLineIndex].Commands.Length > 0)
			{
				while (!this.paused && this.commandIndex < this.textBlock.Lines[this.currentLineIndex].Commands.Length && this.totalCharCount >= (float)this.textBlock.Lines[this.currentLineIndex].Commands[this.commandIndex].Position)
				{
					ITextCommand textCommand = this.textBlock.Lines[this.currentLineIndex].Commands[this.commandIndex];
					if (textCommand is TextPause)
					{
						this.pauseDuration = (textCommand as TextPause).Duration;
						this.paused = true;
					}
					else if (textCommand is TextWait)
					{
						this.waiting = true;
						Outer.Log("wait");
						if (this.OnTextWait != null)
						{
							this.OnTextWait();
						}
					}
					else if (textCommand is TextTrigger && this.OnTextTrigger != null)
					{
						this.OnTextTrigger(textCommand as TextTrigger);
					}
					this.commandIndex++;
					result = true;
				}
			}
			return result;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public const int LINE_HEIGHT = 14;

		public const int BULLET_MARGIN = 8;

		public const int TEXT_OFFSET_Y = 0;

		public const int BULLET_OFFSET_Y = 4;

		private int depth;

		private DewDrop.UserInput.DButtons advance;

		private int textPos;

		private int textLen;

		private bool finshed;

		private bool visible;

		private TextBlock textBlock;
		
		private bool useBeeps;

		private RenderPipeline pipeline;

		private int topLineIndex;

		private int currentLineIndex;

		private int currentTextIndex;

		private TextRenderer currentText;

		private TextRenderer[] texts;

		private Graphic[] bullets;

		private bool[] bulletVisibility;

		private bool showBullets;

		private float totalCharCount;

		private int pauseTimer;

		private int pauseDuration;

		private int commandIndex;

		private bool paused;

		private bool waiting;

		private float textSpeed;

		private float origTextSpeed;

		private float nextCharWaiter;

		public delegate void TypewriterCompleteHandler();

		public delegate void WaitCommandHandler();

		public delegate void TriggerCommandHandler(TextTrigger trigger);
	}
}