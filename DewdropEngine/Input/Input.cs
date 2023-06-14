#region

using DewDrop.Utilities;
using SFML.System;
using SFML.Window;

#endregion

namespace DewDrop.UserInput;

/// <summary>
///     Receives input from the user and invokes input events.
/// </summary>
public class Input
{
    #region Properties

    public static Input Instance { get; private set; }

    /// <summary>
    ///     Controls if events and key monitoring is enabled.
    /// </summary>
    public static bool RecieveInput
    {
        get => Instance._recieveInput;
        set => Instance._recieveInput = value;
    }

    public static bool MouseDown
    {
        get => Instance._leftMousePressed;
        set => Instance._leftMousePressed = value;
    }

    #endregion

    #region Private

    private Dictionary<Keyboard.Key, bool> _keyStatuses;
    private Dictionary<DButtons, bool> _buttonStatuses;
    private bool _recieveInput = true;
    private ControllerType _controllerType = ControllerType.Keyboard;
    private bool _leftMousePressed;

    #endregion

    #region Events

    /// <summary>
    ///     Called when a keyboard key is pressed.
    /// </summary>
    public event EventHandler<Keyboard.Key> OnKeyPressed;

    /// <summary>
    ///     Called when a keyboard key is released.
    /// </summary>
    public event EventHandler<Keyboard.Key> OnKeyReleased;

    /// <summary>
    ///     Called when a recognized button is pressed.
    /// </summary>
    public event EventHandler<DButtons> OnButtonPressed;

    /// <summary>
    ///     Called when a recognized button is released.
    /// </summary>
    public event EventHandler<DButtons> OnButtonReleased;

    /// <summary>
    ///     Called when the left mouse button is clicked.
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnMouseClick;

    /// <summary>
    ///     Called when the left mouse button is clicked.
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnMouseReleased;

    #endregion

    #region Indexers

    // I could change this to a static class if C# would fucking allow static indexers.
    public bool this[Keyboard.Key key]
    {
        get => _keyStatuses[key];
        set => _keyStatuses[key] = value;
    }

    public bool this[DButtons key]
    {
        get => _buttonStatuses[key];
        set => _buttonStatuses[key] = value;
    }

    #endregion

    #region Dictionaries

    public readonly Dictionary<Keyboard.Key, DButtons> SFMLtoDButtonsMap = new()
    {
        { Keyboard.Key.E, DButtons.Select },
        { Keyboard.Key.Escape, DButtons.Back }
    };

    //https://en.sfml-dev.org/forums/index.php?topic=16748.msg120279#msg120279
    public readonly Dictionary<uint, DButtons> ControllertoDButtonsMap = new()
    {
        { 0, DButtons.Select },
        { 1, DButtons.Back }
    };

    #endregion

    public Input()
    {
        if (Instance != null)
        {
            throw new Exception("Input already exists!");
        }

        _keyStatuses = new Dictionary<Keyboard.Key, bool>();
        _buttonStatuses = new Dictionary<DButtons, bool>();

        // add keys
        foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
            // for some reason, semicolons throw a duplicate key exception
            // this is to prevent that
            if (!_keyStatuses.ContainsKey(key))
                _keyStatuses.Add(key, false);

        foreach (DButtons key in Enum.GetValues(typeof(DButtons)))

            if (!_buttonStatuses.ContainsKey(key))
                _buttonStatuses.Add(key, false);

        Instance = this;
    }

    #region Window attachment

    /// <summary>
    ///     Hooks into the window and allows for input to be read.
    /// </summary>
    /// <param name="window">The window to read input to.</param>
    public void AttachToWindow(Window window)
    {
        window.SetKeyRepeatEnabled(false);
        window.KeyPressed += WindowOnKeyPressed;
        window.KeyReleased += WindowOnKeyReleased;
        window.JoystickConnected += WindowOnJoystickConnected;
        window.JoystickDisconnected += WindowOnJoystickDisconnected;
        window.JoystickButtonPressed += WindowOnJoystickButtonPressed;
        window.JoystickButtonReleased += WindowOnJoystickButtonReleased;
        window.MouseButtonPressed += WindowOnMouseButtonPressed;
        window.MouseButtonReleased += WindowOnMouseButtonReleased;
    }

    /// <summary>
    ///     Detaches from the window and stops reading input.
    /// </summary>
    /// <param name="window">The window to detach from.</param>
    public void DetachFromWindow(Window window)
    {
        window.KeyPressed -= WindowOnKeyPressed;
        window.KeyReleased -= WindowOnKeyReleased;
        window.JoystickConnected -= WindowOnJoystickConnected;
        window.JoystickDisconnected -= WindowOnJoystickDisconnected;
        window.JoystickButtonPressed -= WindowOnJoystickButtonPressed;
        window.JoystickButtonReleased -= WindowOnJoystickButtonReleased;
        ;
        window.MouseButtonPressed -= WindowOnMouseButtonPressed;
        window.MouseButtonReleased -= WindowOnMouseButtonReleased;
    }

    #endregion

    #region Mouse

    /// <summary>
    ///     Sets the position of the mouse relative to the game window.
    /// </summary>
    /// <param name="position">The position to set the mouse to.</param>
    public static void SetMousePosition(Vector2f position)
    {
        // This is stupid, let me explain:
        // We want a pixel location of where the mouse is relative to the game's window
        // Here's the problem: The scale of the screen
        float scaleFactor = Engine.FrameBufferScale;
        Mouse.SetPosition((Vector2i)(position * scaleFactor)); // * scaleFactor;
    }

    /// <summary>
    ///     Gets the position of the mouse relative to the game window.
    /// </summary>
    /// <returns>The position of the mouse relative to the game window.</returns>
    public static Vector2 GetMousePosition()
    {
        // had a really long winded thing written but i'll shorten it
        // the mouse position is not relative to the game's window
        // what is (69, 69) in game space is not the same in monitor space
        // this function is translating monitor space to window space.
        /*if (Engine.Fullscreen) {
            VideoMode desktopMode;
            desktopMode = VideoMode.DesktopMode;
            float fullScreenMin = Math.Min(desktopMode.Width / Engine.SCREEN_WIDTH, desktopMode.Height / Engine.SCREEN_HEIGHT);
            return (Vector2f)Mouse.GetPosition(Engine.Window) / fullScreenMin;
        }*/

        return (Vector2)Mouse.GetPosition(Engine.Window) / Engine.FrameBufferScale;
    }

    private void WindowOnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        OnMouseClick?.Invoke(this, e);
        if (e.Button == Mouse.Button.Left)
        {
            _leftMousePressed = true;
        }
    }

    private void WindowOnMouseButtonReleased(object? sender, MouseButtonEventArgs e)
    {
        OnMouseReleased?.Invoke(this, e);
        if (e.Button == Mouse.Button.Left)
        {
            _leftMousePressed = false;
        }

    }

    #endregion

    #region Controller

    private void WindowOnJoystickButtonReleased(object? sender, JoystickButtonEventArgs e)
    {
        if (!_recieveInput)
            return;

        Debug.Log("button released");
        if (_controllerType == ControllerType.Xbox360)
        {
            if (ControllertoDButtonsMap.ContainsKey(e.Button))
            {
                Debug.Log(ControllertoDButtonsMap[e.Button]);
                OnButtonReleased?.Invoke(this, ControllertoDButtonsMap[e.Button]);

            }
        }

    }

    private void WindowOnJoystickButtonPressed(object? sender, JoystickButtonEventArgs e)
    {
        if (!_recieveInput)
            return;

        Debug.Log("button pressed");
        if (_controllerType == ControllerType.Xbox360)
        {
            if (ControllertoDButtonsMap.ContainsKey(e.Button))
            {
                Debug.Log(ControllertoDButtonsMap[e.Button]);
                OnButtonPressed?.Invoke(this, ControllertoDButtonsMap[e.Button]);
            }
        }
    }

    private void WindowOnJoystickDisconnected(object? sender, JoystickConnectEventArgs e)
    {
        _controllerType = ControllerType.Keyboard;
        Debug.Log("disconnected");
    }

    private void WindowOnJoystickConnected(object? sender, JoystickConnectEventArgs e)
    {
        Joystick.Update();
        Debug.Log(e);
        Joystick.Identification identification = Joystick.GetIdentification(e.JoystickId);
        Debug.Log(identification.Name);
        if (identification.Name.Contains("XBOX"))
        {
            _controllerType = ControllerType.Xbox360;
        }
    }

    #endregion

    #region Keyboard

    private void WindowOnKeyReleased(object? sender, KeyEventArgs e)
    {
        if (!_recieveInput)
            return;
        OnKeyReleased?.Invoke(this, e.Code);
        _keyStatuses[e.Code] = false;

        if (SFMLtoDButtonsMap.ContainsKey(e.Code))
        {
            OnButtonReleased?.Invoke(this, SFMLtoDButtonsMap[e.Code]);
        }
    }

    private void WindowOnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (!_recieveInput)
            return;
        OnKeyPressed?.Invoke(this, e.Code);
        _keyStatuses[e.Code] = true;

        if (SFMLtoDButtonsMap.ContainsKey(e.Code))
        {
            OnButtonPressed?.Invoke(this, SFMLtoDButtonsMap[e.Code]);
        }
    }

    #endregion
}