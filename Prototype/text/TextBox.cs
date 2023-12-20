using DewDrop;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using Mother4.GUI;
using Mother4.Scripts.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections;

namespace Mother4.GUI
{
    public class TextBox : Entity
    {  
        public override string Name => "Text Box";

        public event TextBox.CompletionHandler OnTextboxComplete;

        public event TextBox.TypewriterCompletionHandler OnTypewriterComplete;

        public bool Visible => _visible;
        
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        readonly RenderPipeline _pipeline;
        readonly TypewriterBox _typewriterBox;

        readonly Nametag _nametag;

        readonly WindowBox _window;

        readonly ShapeRenderer _topLetterbox;
        readonly ShapeRenderer _bottomLetterbox;

        bool _visible;

        bool _nametagVisible;

        bool _arrowVisible;
        readonly bool _hideAdvanceArrow;
        readonly Graphic _advanceArrow;

        bool _textboxWaitForPlayer;
        bool _typewriterDone;

        TextBox.AnimationState _state;

        float _letterboxProgress;
        float _topLetterboxY;
        float _bottomLetterboxY;
         
        float _textboxY;

        bool _canTransitionIn;
        bool _canTransitionOut;

        //   readonly ScreenDimmer dimmer;

        enum AnimationState
        {
            SlideIn,
            Textbox,
            SlideOut,
            Hide
        }

        public delegate void CompletionHandler();

        public delegate void TypewriterCompletionHandler();

        public TextBox(RenderPipeline pipeline, int colorIndex)
        {
            _pipeline = pipeline;
            _state = TextBox.AnimationState.SlideIn;
            _topLetterboxY = -14f;
            _bottomLetterboxY = 180f;
            //dimmer = new ScreenDimmer(pipeline, Color.Transparent, 0, 2147450879);
            Vector2 finalCenter = ViewManager.Instance.FinalCenter;
            Vector2 Vector2 = finalCenter - ViewManager.Instance.View.Size / 2f;
            Vector2 size = new Vector2(320f, 14f);
            Vector2 size2 = new Vector2(320f, 14);
            _topLetterbox = new ShapeRenderer(new RectangleShape(size), 
                new Vector2(0f, _topLetterboxY),
                size2,//new Vector2(0f, 0f), 
                Vector2.Zero, 2147450880);
            _topLetterbox.FillColor = Color.Black;
            _topLetterbox.OutlineColor = Color.White;
            _topLetterbox.DrawRegardlessOfVisibility    = true;
            _topLetterbox.Visible = false;
            _canTransitionIn = true;
            _canTransitionOut = true;
            _pipeline.Add(_topLetterbox);
            _bottomLetterbox = new ShapeRenderer(new RectangleShape(size), new Vector2(0f, _bottomLetterboxY), size, Vector2.Zero, 2147450878);
            _bottomLetterbox.FillColor = Color.Black;
            _bottomLetterbox.OutlineColor = Color.White;
            _bottomLetterbox.DrawRegardlessOfVisibility = true;
            _bottomLetterbox.Visible = false;
            _pipeline.Add(_bottomLetterbox);
            _typewriterBox = new TypewriterBox(pipeline, new Vector2(Vector2.X + TextBox.TEXT_POSITION.X, Vector2.Y + TextBox.TEXT_POSITION.Y), TextBox.TEXT_SIZE, 2147450880, DButtons.Select, true, new TextBlock(new List<TextLine>()));
            _window = new WindowBox("window.gdat",
                0, // palatte
                // position
                new Vector2(Vector2.X + TextBox.BOX_POSITION.X,
                    Vector2.Y + TextBox.BOX_POSITION.Y),
                // size
                TextBox.BOX_SIZE,
                2147450879)
            {
                Visible = false
            };
            _pipeline.Add(_window);
            _advanceArrow = new SpriteGraphic("realcursor.dat", "down", new Vector2(Vector2.X + TextBox.BUTTON_POSITION.X, Vector2.Y + TextBox.BUTTON_POSITION.Y), 2147450880)
            {
                Visible = false
            };
            _pipeline.Add(_advanceArrow);
            _nametag = new Nametag("Corpse", new Vector2(Vector2.X + TextBox.NAMETAG_POSITION.X, Vector2.Y + TextBox.NAMETAG_POSITION.Y), 2147450880)
            {
                Visible = false,
            };
            _pipeline.Add(_nametag);
            _visible = false;
            _nametagVisible = false;
            _arrowVisible = false;
            _hideAdvanceArrow = false;
            _typewriterBox.OnTypewriterComplete += TypewriterComplete;
            _typewriterBox.OnTextWait += TextWait;
            Input.OnButtonPressed += OnButtonPressed;
            ViewManager.Instance.OnMove += OnViewMove;
        }

        public void ClearTypewriterComplete()
        {
            OnTypewriterComplete = null;
        }

        protected virtual void TextWait()
        {
            _textboxWaitForPlayer = true;
            if (!_hideAdvanceArrow)
            {
                _arrowVisible = true;
                _advanceArrow.Visible = _arrowVisible;
            }
        }

        public class RollingText {
            public string Text { get; set; }
            public string Name;
        }
        public readonly Queue RollingTextQueue = new Queue();
        

        protected virtual void TypewriterComplete()
        {
            _textboxWaitForPlayer = true;
            _typewriterDone = true;
            if (!_hideAdvanceArrow)
            {
                _arrowVisible = true;
                _advanceArrow.Visible = _arrowVisible;
            }

            if (OnTypewriterComplete != null)
            {
                OnTypewriterComplete();
            }
        }

        protected virtual void OnButtonPressed(object? sender, DButtons dButtons)
        {
            if (_textboxWaitForPlayer && dButtons == DButtons.Select)
            {
                if (RollingTextQueue.Count > 0 && _typewriterDone) {
                    RollingText rt = (RollingText)RollingTextQueue.Dequeue();
                    Reset(rt.Text, rt.Name, true, true);
                    _typewriterDone = false;
                    Recenter();
                    return;
                }
                
                _textboxWaitForPlayer = false;
                if (!_typewriterDone)
                {
                    _typewriterBox.ContinueFromWait();
                    if (!_hideAdvanceArrow)
                    {
                        _arrowVisible = false;
                        _advanceArrow.Visible = _arrowVisible;
                        return;
                    }
                }
                else if (OnTextboxComplete != null)
                {
                    OnTextboxComplete();
                }
            }
        }

        protected virtual void Recenter () {
            Vector2 finalCenter = ViewManager.Instance.FinalCenter;
            Vector2 viewSize = ViewManager.Instance.View.Size/2f;
            Vector2 textBoxPosition = BOX_POSITION;
            Vector2 buttonPosition = BUTTON_POSITION;
            Vector2 nametagPosition = NAMETAG_POSITION;
            Vector2 textPosition = TEXT_POSITION;

            _position = new Vector2(finalCenter.X - viewSize.X + textBoxPosition.X, finalCenter.Y - viewSize.Y + textBoxPosition.Y);
            _window.RenderPosition = new Vector2(_position.X, _position.Y + _textboxY);
            _advanceArrow.RenderPosition = new Vector2(finalCenter.X - viewSize.X + buttonPosition.X, finalCenter.Y - viewSize.Y + buttonPosition.Y);
            _nametag.RenderPosition = new Vector2(finalCenter.X - viewSize.X + nametagPosition.X, finalCenter.Y - viewSize.Y + nametagPosition.Y + _textboxY);
            _typewriterBox.Reposition(new Vector2(finalCenter.X - viewSize.X + textPosition.X, finalCenter.Y - viewSize.Y + textPosition.Y + _textboxY));
            _topLetterbox.RenderPosition = new Vector2(finalCenter.X - viewSize.X, finalCenter.Y - viewSize.Y + _topLetterboxY);
            _bottomLetterbox.RenderPosition = new Vector2(finalCenter.X - viewSize.X, finalCenter.Y - viewSize.Y + _bottomLetterboxY);
        }

        public virtual void Reset(string text, string namestring, bool slideIn, bool slideOut)
        {
            _canTransitionIn = slideIn;
            _canTransitionOut = slideOut;
            _typewriterBox.Reset(TextProcessor.Process(new FontData(), text, (int)TextBox.TEXT_SIZE.X));
            _arrowVisible = false;
            _textboxWaitForPlayer = false;
            _typewriterDone = false;
            if (namestring != null && namestring.Length > 0)
            {

                _nametag.Name = namestring;
                _nametagVisible = true;
            }
            else
            {
                _nametagVisible = false;
            }
            _nametag.Visible = _nametagVisible;
            _window.FrameStyle = "window.gdat";
        }

        float numa = -13f;
         void UpdateLetterboxing(float amount)
        {
            float num = Math.Max(0f, Math.Min(1f, amount));
            _topLetterboxY = -180 + (float)((int)(-14*(1f - num)));// - _topLetterbox.Size.y;
            _bottomLetterboxY = (float)(180L - (long)((int)(14f * num)));
            
            _topLetterbox.RenderPosition = new Vector2f(ViewManager.Instance.Viewrect.Left, ViewManager.Instance.Viewrect.Top + _topLetterboxY);
            _bottomLetterbox.RenderPosition = new Vector2f(ViewManager.Instance.Viewrect.Left, ViewManager.Instance.Viewrect.Top + _bottomLetterboxY);
            /*
            float num = Math.Max(0f, Math.Min(1f, amount));
           // _topLetterbox.Visible = false;
            _topLetterboxY = (int)(-0.5f * (1f - num));
            _bottomLetterboxY = 180L - (int)(14f * num);; //
            _topLetterbox.RenderPosition = new Vector2(ViewManager.Instance.Viewrect.Left, ViewManager.Instance.Viewrect.Top + _topLetterboxY);
            _bottomLetterbox.RenderPosition = new Vector2(ViewManager.Instance.Viewrect.Left, ViewManager.Instance.Viewrect.Top + _bottomLetterboxY);
           // _bottomLetterbox.RenderPosition = new Vector2((int)(ViewManager.Instance.Viewrect.Left), (int)(ViewManager.Instance.Viewrect.Top + LETTERBOX_POSITION.y + _bottomLetterboxY));; //(int)(ViewManager.Instance.Viewrect.Top + TextBox.BOX_POSITION.Y + _textboxY); //
            */
        }

         void UpdateTextbox(float amount)
        {
            _textboxY = 4f * (1f - Math.Max(0f, Math.Min(1f, amount)));
            _typewriterBox.Position = new Vector2((int)(ViewManager.Instance.Viewrect.Left + TextBox.TEXT_POSITION.X), (int)(ViewManager.Instance.Viewrect.Top + TextBox.TEXT_POSITION.Y + _textboxY));
            _window.RenderPosition = new Vector2((int)(ViewManager.Instance.Viewrect.Left + TextBox.BOX_POSITION.X), (int)(ViewManager.Instance.Viewrect.Top + TextBox.BOX_POSITION.Y + _textboxY));
            _nametag.RenderPosition = new Vector2((int)(ViewManager.Instance.Viewrect.Left + TextBox.NAMETAG_POSITION.X), (int)(ViewManager.Instance.Viewrect.Top + TextBox.NAMETAG_POSITION.Y + _textboxY));
        }

        public virtual void Show()
        {
            if (!_visible)
            {
                _visible = true;
                Recenter();
                _window.Visible = true;
                _topLetterbox.Visible = true;
                _bottomLetterbox.Visible = true;
                _typewriterBox.Show();
                _nametag.Visible = _nametagVisible;
                _advanceArrow.Visible = _arrowVisible;
                _state = TextBox.AnimationState.SlideIn;
                _letterboxProgress = (_canTransitionIn ? 0f : 1f);
                UpdateLetterboxing(_letterboxProgress);
            }
        }

        public virtual void Hide()
        {
            if (_visible)
            {
                _visible = false;
                Recenter();
                _advanceArrow.Visible = false;
                _state = TextBox.AnimationState.SlideOut;
                _letterboxProgress = (_canTransitionOut ? 1f : 0f);
                UpdateLetterboxing(_letterboxProgress);
                UpdateTextbox(_letterboxProgress * 2f);
            }
        }

        public void SetDimmer(float dim)
        {
           // dimmer.ChangeColor(new Color(0, 0, 0, (byte)(255f * dim)), 30);
        }

         void OnViewMove(ViewManager sender, Vector2 center)
        {
            if (_visible || _letterboxProgress > 0f)
            {
                Recenter();
            }
        }

         float TweenLetterboxing(float progress)
        {
            return (float)(0.5 - Math.Cos(progress * 3.141592653589793) / 2.0);
        }

        public override void Update()
        {
            base.Update();
            switch (_state)
            {
                case TextBox.AnimationState.SlideIn:
                    if (_letterboxProgress < 1f)
                    {
                        UpdateLetterboxing(TweenLetterboxing(_letterboxProgress));
                        UpdateTextbox(_letterboxProgress);
                        _letterboxProgress += 0.2f;
                    }
                    else
                    {
                        _state = TextBox.AnimationState.Textbox;
                        UpdateLetterboxing(1f);
                        UpdateTextbox(1f);
                    }
                    break;
                case TextBox.AnimationState.Textbox:
                    if (_visible)
                    {
                        _typewriterBox.Update();
                    }
                    break;
                case TextBox.AnimationState.SlideOut:
                    if (_letterboxProgress > 0f)
                    {
                        UpdateLetterboxing(TweenLetterboxing(_letterboxProgress));
                        UpdateTextbox(_letterboxProgress);
                        _letterboxProgress -= 0.2f;
                    }
                    else
                    {
                        _state = TextBox.AnimationState.Hide;
                        UpdateLetterboxing(0f);
                        UpdateTextbox(0f);
                        _typewriterBox.Hide();
                        _nametag.Visible = false;
                        _window.Visible = false;
                        _topLetterbox.Visible = false;
                        _bottomLetterbox.Visible = false;
                    }
                    break;
            }
          //  dimmer.Update();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _advanceArrow.Dispose();
            _window.Dispose();
            _nametag.Dispose();
            _topLetterbox.Dispose();
            _bottomLetterbox.Dispose();
            _typewriterBox.OnTypewriterComplete -= TypewriterComplete;
            _typewriterBox.OnTextWait -= TextWait;
            Input.OnButtonPressed -= OnButtonPressed;
            ViewManager.Instance.OnMove -= OnViewMove;
            _typewriterBox.Dispose();
        }

        protected const DButtons ADVANCE_BUTTON = DButtons.Select;

        protected const float LETTERBOX_HEIGHT = 14f;

        protected const float LETTERBOX_SPEED = 0.2f;

        protected const float TEXTBOX_Y_OFFSET = 4f;

        public const int DEPTH = 2147450880;

        protected static Vector2 BOX_POSITION = new Vector2(51, 120f);
        protected static Vector2 LETTERBOX_POSITION = BOX_POSITION - new Vector2(0f, -28f);

        protected static Vector2 BOX_SIZE = new Vector2(230, 56f);

        protected static Vector2 TEXT_POSITION = new Vector2(TextBox.BOX_POSITION.X + 10f, TextBox.BOX_POSITION.Y + 8f);

        protected static Vector2 TEXT_SIZE = new Vector2(TextBox.BOX_SIZE.X - 31f, TextBox.BOX_SIZE.Y - 8f);

        protected static Vector2 NAMETAG_POSITION = new Vector2(TextBox.BOX_POSITION.X + 3f, TextBox.BOX_POSITION.Y - 14f);

        protected static Vector2 NAMETEXT_POSITION = new Vector2(TextBox.NAMETAG_POSITION.X + 5f, TextBox.NAMETAG_POSITION.Y + 1f);

        protected static Vector2 BUTTON_POSITION = new Vector2(TextBox.BOX_POSITION.X + TextBox.BOX_SIZE.X - 14f, TextBox.BOX_POSITION.Y + TextBox.BOX_SIZE.Y - 6f);
    }
}