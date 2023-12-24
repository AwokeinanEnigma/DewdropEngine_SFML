#region

using DewDrop.Internal; using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace DewDrop.GUI;

/// <summary>
/// Manages the views in the game.
/// </summary>
public class ViewManager {
	public enum MoveMode {
		Linear,
		Smoothed,
		ExpIn,
		ExpOut
	}
    /// <summary>
    /// Gets the instance of the ViewManager.
    /// </summary>
    public static ViewManager Instance { get; private set; }

    /// <summary>
    /// Event that is invoked when the view is moved.
    /// </summary>
    public event MoveHandler OnMove;

    /// <summary>
    /// Event that is invoked when the view has completed moving to a target.
    /// </summary>
    public event MoveToCompleteHandler OnMoveToComplete;

    /// <summary>
    /// Gets the current view.
    /// </summary>
    public View View { get; }

    /// <summary>
    /// Gets the rectangle representing the view's position and size.
    /// </summary>
    public FloatRect Viewrect {
        get {
            SetViewRect();
            return _viewRect;
        }
    }

    /// <summary>
    /// Gets the final center of the view.
    /// </summary>
    public Vector2 FinalCenter => GetCenter();

    /// <summary>
    /// Gets the final top left position of the view.
    /// </summary>
    public Vector2 FinalTopLeft => GetCenter() - Engine.ScreenSize/2;

    /// <summary>
    /// Gets or sets the center of the view.
    /// </summary>
    public Vector2 Center {
        get => _viewCenter;
        set {
            _previousViewCenter = _viewCenter;
            _viewCenter = value;
            if (_previousViewCenter != _viewCenter && OnMove != null) {
                OnMove(this, new Vector2(View.Center));
            }
        }
    }

    /// <summary>
    /// Gets the top left position of the view.
    /// </summary>
    public Vector2 TopLeft => _viewCenter - (Engine.ScreenSize/2);

    /// <summary>
    /// Gets or sets the entity that the view is following.
    /// </summary>
    public GameObject EntityFollow { get; set; }

    /// <summary>
    /// Gets or sets the offset of the view from the entity it is following.
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Gets or sets the mode of movement when moving to a target.
    /// </summary>
    public MoveMode MoveToMode { get; set; }
	
	readonly RenderTarget _window;

	FloatRect _viewRect;
	Vector2 _previousViewCenter;
	Vector2 _viewCenter;

	Vector2 _shakeOffset;
	Vector2 _shakeIntensity;

	int _shakeDuration;
	int _shakeProgress;

	bool _isMovingTo;
	float _moveToSpeed;

	Vector2 _moveFromPosition;
	Vector2 _moveToPosition;
	
	public delegate void MoveHandler (ViewManager sender, Vector2 newCenter);
	public delegate void MoveToCompleteHandler (ViewManager sender);

	public ViewManager () {
		Instance = this;
		MoveToMode = MoveMode.Linear;
		
		_window = Engine.RenderTexture;
		View = new View(new Vector2f(0f, 0f), Engine.ScreenSize);
		_window.SetView(View);
		_viewCenter = Vector2.Zero;
		Offset = Vector2.Zero;
		_shakeOffset = Vector2.Zero;
		_viewRect = default;
		SetViewRect();
	}

	public void SetZoom (float zoom) {
		View.Zoom(zoom);
	}
	Vector2 GetCenter () {
		return Vector2.Truncate(_viewCenter + Offset + _shakeOffset);
	}

	void SetViewRect () {
		_viewRect.Left = View.Center.X - View.Size.X/2f;
		_viewRect.Top = View.Center.Y - View.Size.Y/2f;
		_viewRect.Width = View.Size.X;
		_viewRect.Height = View.Size.Y;
	}

	float CalculateSmoothMovement (float progress) {
		return (float)(1.0 - (Math.Cos(progress*2.0*Math.PI)/2.0 + 0.5))*(_moveToSpeed - 0.5f) + 0.5f;
	}

	public void Update () {
		if (_shakeProgress < _shakeDuration) {
			float num = _shakeIntensity.x*(1f - _shakeProgress/(float)_shakeDuration);
			float num2 = _shakeIntensity.y*(1f - _shakeProgress/(float)_shakeDuration);
			_shakeOffset.x = -num + 1*num*2f;
			_shakeOffset.y = -num2 + 1*num2*2f;
			_shakeProgress++;
		}

		if (EntityFollow != null) {
			if (!_isMovingTo) {
				_previousViewCenter = _viewCenter;
				_viewCenter = EntityFollow.Transform.Position;
			} else {
				_moveToPosition = EntityFollow.Transform.Position;
			}
		}

		if (_isMovingTo) {
			float num3 = Vector2.Magnitude(_moveFromPosition - _moveToPosition);
			float num4 = Vector2.Magnitude(_viewCenter - _moveToPosition);
			float num5 = num3 > 0f ? 1f - num4/num3 : 1f;
			float num6 = 1f;
			switch (MoveToMode) {
			case MoveMode.Linear:
				num6 = _moveToSpeed;
				break;
			case MoveMode.Smoothed:
				num6 = CalculateSmoothMovement(num5);
				break;
			case MoveMode.ExpIn:
				num6 = (1f - num5)*(_moveToSpeed - 0.5f) + 0.5f;
				break;
			case MoveMode.ExpOut:
				num6 = num5*(_moveToSpeed - 0.5f) + 0.5f;
				break;
			}

			_previousViewCenter = _viewCenter;
			_viewCenter += Vector2.Normalize(_moveToPosition - _moveFromPosition)*Math.Max(0.1f, num6);
			if (num4 - num6 <= 0.5f) {
				_viewCenter = _moveToPosition;
				_isMovingTo = false;
				OnMoveToComplete?.Invoke(this);
			}
		}

		View.Center = Vector2.Truncate(_viewCenter + Offset + _shakeOffset);
		if (_previousViewCenter != _viewCenter && OnMove != null) {
			OnMove(this, new Vector2(View.Center));
		}
	}

	/// <summary>
	/// Moves the view to a specified entity at a specified speed.
	/// </summary>
	/// <param name="entity">The entity to move to.</param>
	/// <param name="speed">The speed at which to move.</param>
	public void MoveTo (GameObject entity, float speed) {
		if (entity != null) {
			EntityFollow = entity;
			MoveTo(entity.Transform.Position, speed);
		}
	}

	/// <summary>
	/// Moves the view to a specified position at a specified speed.
	/// </summary>
	/// <param name="position">The position to move to.</param>
	/// <param name="speed">The speed at which to move.</param>
	public void MoveTo (Vector2 position, float speed) {
		if (speed > 0f) {
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

	/// <summary>
	/// Moves the view to a specified position at a specified speed.
	/// </summary>
	/// <param name="x">The x-coordinate of the position to move to.</param>
	/// <param name="y">The y-coordinate of the position to move to.</param>
	/// <param name="speed">The speed at which to move.</param>
	public void MoveTo (float x, float y, float speed) {
		MoveTo(new Vector2(x, y), speed);
	}

	/// <summary>
	/// Cancels the current move to operation.
	/// </summary>
	public void CancelMoveTo () {
		_isMovingTo = false;
	}

	/// <summary>
	/// Moves the view by a specified offset.
	/// </summary>
	public void Move (float x, float y) {
		Move(new Vector2(x, y));
	}

	/// <summary>
	/// Moves the view by a specified offset.
	/// </summary>
	public void Move (Vector2 offset) {
		View.Move(offset);
	}

	public void UseView () {
		// TODO, FOR ANYONE WHO'S WONDERING ABOUT THE VIEW GENERATED JUNK
		// THIS IS THE CAUSE!
		// THERE'S NOTHING YOU CAN DO ABOUT IT!
		// BOTHER SFML DEVS!
		_window.SetView(View);
	}

	public void UseDefault () {
		_window.SetView(_window.DefaultView);
	}

	public void Reset () {
		View.Reset(new FloatRect(0f, 0f, Engine.ScreenSize.x, Engine.ScreenSize.y));
		_viewCenter = new Vector2(View.Center);
		_shakeOffset = Vector2.Zero;
	}

	/// <summary>
	/// Shakes the view with a specified intensity and duration.
	/// </summary>
	/// <param name="intensity">The intensity of the shake.</param>
	/// <param name="duration">How long the shake should last.</param>
	public void Shake (Vector2 intensity, float duration) {
		_shakeIntensity = intensity;
		_shakeDuration = (int)(duration*60f);
		_shakeProgress = 0;
		_shakeOffset = Vector2.Zero;
	}
}
