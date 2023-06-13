#region

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
            return viewRect;
        }
    }

    public Vector2 FinalCenter => GetCenter();

    public Vector2 FinalTopLeft => GetCenter() - Engine.Screen_Size / 2;

    public Vector2 Center
    {
        get => viewCenter;
        set
        {
            previousViewCenter = viewCenter;
            viewCenter = value;
            if (previousViewCenter != viewCenter && OnMove != null)
            {
                OnMove(this, new Vector2(view.Center));
            }
        }
    }

    public Vector2 TopLeft => viewCenter - (Engine.Screen_Size / 2).Vector2f;

    public Nothing FollowActor
    {
        get => followActor;
        set => followActor = value;
    }

    public Vector2 Offset
    {
        get => offset;
        set => offset = value;
    }

    public MoveMode MoveToMode
    {
        get => moveToMode;
        set => moveToMode = value;
    }

    private ViewManager()
    {
        window = Engine.RenderTexture;
        view = new View(new Vector2f(0f, 0f), new Vector2f(320f, 180f));
        window.SetView(view);
        viewCenter = Vector2.Zero;
        offset = Vector2.Zero;
        shakeOffset = Vector2.Zero;
        viewRect = default;
        SetViewRect();
    }

    private Vector2 GetCenter()
    {
        return Vector2.Truncate(viewCenter + offset + shakeOffset);
    }

    private void SetViewRect()
    {
        viewRect.Left = view.Center.X - view.Size.X / 2f;
        viewRect.Top = view.Center.Y - view.Size.Y / 2f;
        viewRect.Width = view.Size.X;
        viewRect.Height = view.Size.Y;
    }

    private float CalculateSmoothMovement(float progress)
    {
        return (float)(1.0 - (Math.Cos(progress * 2.0 * 3.141592653589793) / 2.0 + 0.5)) * (moveToSpeed - 0.5f) + 0.5f;
    }

    public void Update()
    {
        if (shakeProgress < shakeDuration)
        {
            float num = shakeIntensity.x * (1f - shakeProgress / (float)shakeDuration);
            float num2 = shakeIntensity.y * (1f - shakeProgress / (float)shakeDuration);
            shakeOffset.x = -num + 1 * num * 2f;
            shakeOffset.y = -num2 + 1 * num2 * 2f;
            shakeProgress++;
        }

        if (followActor != null)
        {
            if (!isMovingTo)
            {
                previousViewCenter = viewCenter;
                viewCenter = followActor.Position;
            }
            else
            {
                moveToPosition = followActor.Position;
            }
        }

        if (isMovingTo)
        {
            float num3 = Vector2.Magnitude(moveFromPosition - moveToPosition);
            float num4 = Vector2.Magnitude(viewCenter - moveToPosition);
            float num5 = num3 > 0f ? 1f - num4 / num3 : 1f;
            float num6 = 1f;
            switch (moveToMode)
            {
                case MoveMode.Linear:
                    num6 = moveToSpeed;
                    break;
                case MoveMode.Smoothed:
                    num6 = CalculateSmoothMovement(num5);
                    break;
                case MoveMode.ExpIn:
                    num6 = (1f - num5) * (moveToSpeed - 0.5f) + 0.5f;
                    break;
                case MoveMode.ExpOut:
                    num6 = num5 * (moveToSpeed - 0.5f) + 0.5f;
                    break;
            }

            previousViewCenter = viewCenter;
            viewCenter += Vector2.Normalize(moveToPosition - moveFromPosition) * Math.Max(0.1f, num6);
            if (num4 - num6 <= 0.5f)
            {
                viewCenter = moveToPosition;
                isMovingTo = false;
                if (OnMoveToComplete != null)
                {
                    OnMoveToComplete(this);
                }
            }
        }

        view.Center = Vector2.Truncate(viewCenter + offset + shakeOffset);
        if (previousViewCenter != viewCenter && OnMove != null)
        {
            OnMove(this, new Vector2(view.Center));
        }
    }

    public void MoveTo(Nothing actor, float speed)
    {
        if (actor != null)
        {
            followActor = actor;
            MoveTo(actor, speed);
        }
    }

    public void MoveTo(Vector2 position, float speed)
    {
        if (speed > 0f)
        {
            moveFromPosition = viewCenter;
            moveToPosition = position;
            moveToSpeed = speed;
            isMovingTo = true;
            return;
        }

        moveFromPosition = viewCenter;
        moveToPosition = position;
        viewCenter = moveToPosition;
        moveToSpeed = 0f;
        isMovingTo = false;
    }

    public void MoveTo(float x, float y, float speed)
    {
        MoveTo(new Vector2(x, y), speed);
    }

    public void CancelMoveTo()
    {
        isMovingTo = false;
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
        viewCenter = new Vector2(view.Center);
        shakeOffset = Vector2.Zero;
    }

    public void Shake(Vector2 intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = (int)(duration * 60f);
        shakeProgress = 0;
        shakeOffset = Vector2.Zero;
    }

    private static ViewManager instance;

    private View view;

    private RenderTarget window;

    private Nothing followActor;

    private FloatRect viewRect;

    private Vector2 previousViewCenter;

    private Vector2 viewCenter;

    private Vector2 offset;

    private Vector2 shakeOffset;

    private Vector2 shakeIntensity;

    private int shakeDuration;

    private int shakeProgress;

    private bool isMovingTo;

    private Vector2 moveFromPosition;

    private Vector2 moveToPosition;

    private float moveToSpeed;

    private MoveMode moveToMode;

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