#region

using DewDrop.Entities;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

#endregion

namespace DewDrop.GUI;

public class ViewManager {
	public static ViewManager Instance {
		get {
			if (instance == null) {
				instance = new ViewManager();
			}

			return instance;
		}
	}

	public event OnMoveHandler OnMove;

	public event OnMoveToCompleteHandler OnMoveToComplete;

	public View View { get; }

	public FloatRect Viewrect {
		get {
			SetViewRect();
			return _viewRect;
		}
	}

	public Vector2 FinalCenter => GetCenter();

	public Vector2 FinalTopLeft => GetCenter() - Engine.Screen_Size/2;

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

	public Vector2 TopLeft => _viewCenter - (Engine.Screen_Size/2).Vector2f;

	public Entity EntityFollow { get; set; }

	public Vector2 Offset { get; set; }

	public MoveMode MoveToMode { get; set; }


	static ViewManager instance;

	readonly RenderTarget window;

	FloatRect _viewRect;

	Vector2 _previousViewCenter;

	Vector2 _viewCenter;

	Vector2 _shakeOffset;

	Vector2 _shakeIntensity;

	int _shakeDuration;

	int _shakeProgress;

	bool _isMovingTo;

	Vector2 _moveFromPosition;

	Vector2 _moveToPosition;

	float _moveToSpeed;


	ViewManager () {
		window = Engine.RenderTexture;
		View = new View(new Vector2f(0f, 0f), new Vector2f(320f, 180f));
		window.SetView(View);
		_viewCenter = Vector2.Zero;
		Offset = Vector2.Zero;
		_shakeOffset = Vector2.Zero;
		_viewRect = default;
		SetViewRect();
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
				_viewCenter = EntityFollow.Position;
			} else {
				_moveToPosition = EntityFollow.Position;
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

	public void MoveTo (Entity entity, float speed) {
		if (entity != null) {
			EntityFollow = entity;
			MoveTo(entity, speed);
		}
	}

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

	public void MoveTo (float x, float y, float speed) {
		MoveTo(new Vector2(x, y), speed);
	}

	public void CancelMoveTo () {
		_isMovingTo = false;
	}

	public void Move (float x, float y) {
		Move(new Vector2(x, y));
	}

	public void Move (Vector2 offset) {
		View.Move(offset.TranslateToV2F());
	}

	public void UseView () {
		// TODO, FOR ANYONE WHO'S WONDERING ABOUT THE VIEW GENERATED JUNK
		// THIS IS THE CAUSE!
		// THERE'S NOTHING YOU CAN DO ABOUT IT!
		// BOTHER SFML DEVS!
		window.SetView(View);
	}

	public void UseDefault () {
		window.SetView(window.DefaultView);
	}

	public void Reset () {
		View.Reset(new FloatRect(0f, 0f, 320f, 180f));
		_viewCenter = new Vector2(View.Center);
		_shakeOffset = Vector2.Zero;
	}

	public void Shake (Vector2 intensity, float duration) {
		_shakeIntensity = intensity;
		_shakeDuration = (int)(duration*60f);
		_shakeProgress = 0;
		_shakeOffset = Vector2.Zero;
	}

	public enum MoveMode {
		Linear,
		Smoothed,
		ExpIn,
		ExpOut
	}

	public delegate void OnMoveHandler (ViewManager sender, Vector2 newCenter);

	public delegate void OnMoveToCompleteHandler (ViewManager sender);
}
