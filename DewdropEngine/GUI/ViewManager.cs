#region

using DewDrop.Entities;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

#endregion

namespace DewDrop.GUI;

public class ViewManager
{
    public static ViewManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ViewManager();
            }

            return instance;
        }
    }

    public event OnMoveHandler OnMove;

    public event OnMoveToCompleteHandler OnMoveToComplete;

    public View View => view;

    public FloatRect Viewrect
    {
        get
        {
            SetViewRect();
            return _viewRect;
        }
    }

    public Vector2 FinalCenter => GetCenter();

    public Vector2 FinalTopLeft => GetCenter() - Engine.Screen_Size / 2;

    public Vector2 Center
    {
        get => _viewCenter;
        set
        {
            _previousViewCenter = _viewCenter;
            _viewCenter = value;
            if (_previousViewCenter != _viewCenter && OnMove != null)
            {
                OnMove(this, new Vector2(view.Center));
            }
        }
    }

    public Vector2 TopLeft => _viewCenter - (Engine.Screen_Size / 2).Vector2f;

    public Entity EntityFollow
    {
        get => _followActor;
        set => _followActor = value;
    }

    public Vector2 Offset
    {
        get => _offset;
        set => _offset = value;
    }

    public MoveMode MoveToMode
    {
        get => _moveToMode;
        set => _moveToMode = value;
    }
    
    
    private static ViewManager instance;

    private View view;

    private readonly RenderTarget window;

    private Entity _followActor;

    private FloatRect _viewRect;

    private Vector2 _previousViewCenter;

    private Vector2 _viewCenter;

    private Vector2 _offset;

    private Vector2 _shakeOffset;

    private Vector2 _shakeIntensity;

    private int _shakeDuration;

    private int _shakeProgress;

    private bool _isMovingTo;

    private Vector2 _moveFromPosition;

    private Vector2 _moveToPosition;

    private float _moveToSpeed;

    private MoveMode _moveToMode;


    private ViewManager()
    {
        window = Engine.RenderTexture;
        view = new View(new Vector2f(0f, 0f), new Vector2f(320f, 180f));
        window.SetView(view);
        _viewCenter = Vector2.Zero;
        _offset = Vector2.Zero;
        _shakeOffset = Vector2.Zero;
        _viewRect = default;
        SetViewRect();
    }

    private Vector2 GetCenter()
    {
        return Vector2.Truncate(_viewCenter + _offset + _shakeOffset);
    }

    private void SetViewRect()
    {
        _viewRect.Left = view.Center.X - view.Size.X / 2f;
        _viewRect.Top = view.Center.Y - view.Size.Y / 2f;
        _viewRect.Width = view.Size.X;
        _viewRect.Height = view.Size.Y;
    }

    private float CalculateSmoothMovement(float progress)
    {
        return (float)(1.0 - (Math.Cos(progress * 2.0 * 3.141592653589793) / 2.0 + 0.5)) * (_moveToSpeed - 0.5f) + 0.5f;
    }

    public void Update()
    {
        if (_shakeProgress < _shakeDuration)
        {
            float num = _shakeIntensity.x * (1f - _shakeProgress / (float)_shakeDuration);
            float num2 = _shakeIntensity.y * (1f - _shakeProgress / (float)_shakeDuration);
            _shakeOffset.x = -num + 1 * num * 2f;
            _shakeOffset.y = -num2 + 1 * num2 * 2f;
            _shakeProgress++;
        }

        if (_followActor != null)
        {
            if (!_isMovingTo)
            {
                _previousViewCenter = _viewCenter;
                _viewCenter = _followActor.Position;
            }
            else
            {
                _moveToPosition = _followActor.Position;
            }
        }

        if (_isMovingTo)
        {
            float num3 = Vector2.Magnitude(_moveFromPosition - _moveToPosition);
            float num4 = Vector2.Magnitude(_viewCenter - _moveToPosition);
            float num5 = num3 > 0f ? 1f - num4 / num3 : 1f;
            float num6 = 1f;
            switch (_moveToMode)
            {
                case MoveMode.Linear:
                    num6 = _moveToSpeed;
                    break;
                case MoveMode.Smoothed:
                    num6 = CalculateSmoothMovement(num5);
                    break;
                case MoveMode.ExpIn:
                    num6 = (1f - num5) * (_moveToSpeed - 0.5f) + 0.5f;
                    break;
                case MoveMode.ExpOut:
                    num6 = num5 * (_moveToSpeed - 0.5f) + 0.5f;
                    break;
            }

            _previousViewCenter = _viewCenter;
            _viewCenter += Vector2.Normalize(_moveToPosition - _moveFromPosition) * Math.Max(0.1f, num6);
            if (num4 - num6 <= 0.5f)
            {
                _viewCenter = _moveToPosition;
                _isMovingTo = false;
                OnMoveToComplete?.Invoke(this);
            }
        }

        view.Center = Vector2.Truncate(_viewCenter + _offset + _shakeOffset);
        if (_previousViewCenter != _viewCenter && OnMove != null)
        {
            OnMove(this, new Vector2(view.Center));
        }
    }

    public void MoveTo(Entity entity, float speed)
    {
        if (entity != null)
        {
            _followActor = entity;
            MoveTo(entity, speed);
        }
    }

    public void MoveTo(Vector2 position, float speed)
    {
        if (speed > 0f)
        {
            _moveFromPosition = _viewCenter;
            _moveToPosition = position;
            _moveToSpeed = speed;
            _isMovingTo = true;
            return;
        }

        _moveFromPosition = _viewCenter;
        _moveToPosition = position;
        _viewCenter = _moveToPosition;
        _moveToSpeed = 0f;
        _isMovingTo = false;
    }

    public void MoveTo(float x, float y, float speed)
    {
        MoveTo(new Vector2(x, y), speed);
    }

    public void CancelMoveTo()
    {
        _isMovingTo = false;
    }

    public void Move(float x, float y)
    {
        Move(new Vector2(x, y));
    }

    public void Move(Vector2 offset)
    {
        view.Move(offset.TranslateToV2F());
    }

    public void UseView()
    {
        window.SetView(view);
    }

    public void UseDefault()
    {
        window.SetView(window.DefaultView);
    }

    public void Reset()
    {
        view.Reset(new FloatRect(0f, 0f, 320f, 180f));
        _viewCenter = new Vector2(view.Center);
        _shakeOffset = Vector2.Zero;
    }

    public void Shake(Vector2 intensity, float duration)
    {
        _shakeIntensity = intensity;
        _shakeDuration = (int)(duration * 60f);
        _shakeProgress = 0;
        _shakeOffset = Vector2.Zero;
    }

    public enum MoveMode
    {
        Linear,
        Smoothed,
        ExpIn,
        ExpOut
    }

    public delegate void OnMoveHandler(ViewManager sender, Vector2 newCenter);

    public delegate void OnMoveToCompleteHandler(ViewManager sender);
}