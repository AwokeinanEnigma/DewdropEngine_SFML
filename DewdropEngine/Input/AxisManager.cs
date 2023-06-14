using DewDrop.Utilities;
using SFML.Window;

namespace DewDrop.UserInput;

public class AxisManager
{
    public enum InputDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private Dictionary<Keyboard.Key, InputDirection> _axisMap;
    private Dictionary<InputDirection, bool> _axisState;

    private int _horizontalAxis;
    private int _verticalAxis;
    
    public Vector2 Axis => _axis;
    
    private Vector2 _axis;

    public AxisManager(Dictionary<Keyboard.Key, InputDirection> axisMap)
    {
        if (axisMap == null)
        {
            throw new ArgumentNullException(nameof(axisMap));
        } 
        _axisMap = axisMap;
        
        _axisState = new Dictionary<InputDirection, bool>();
            
        foreach (InputDirection direction in Enum.GetValues(typeof(InputDirection)))
            if (!_axisState.ContainsKey(direction))
                _axisState.Add(direction, false);

        _axis = new Vector2(0,0);
        
        Input.Instance.OnKeyPressed += InstanceOnOnButtonPressed;
        Input.Instance.OnKeyReleased += InstanceOnOnButtonReleased;
    }
    
    public AxisManager()
    {
        _axisMap = new Dictionary<Keyboard.Key, InputDirection>();
            
        _axisMap.Add(Keyboard.Key.W, InputDirection.Up);
        _axisMap.Add(Keyboard.Key.S, InputDirection.Down);
        _axisMap.Add(Keyboard.Key.A, InputDirection.Left);
        _axisMap.Add(Keyboard.Key.D, InputDirection.Right);

        _axisState = new Dictionary<InputDirection, bool>();
            
        foreach (InputDirection direction in Enum.GetValues(typeof(InputDirection)))
            if (!_axisState.ContainsKey(direction))
                _axisState.Add(direction, false);

        _axis = new Vector2(0,0);
        
        Input.Instance.OnKeyPressed += InstanceOnOnButtonPressed;
        Input.Instance.OnKeyReleased += InstanceOnOnButtonReleased;
    }

    private void InstanceOnOnButtonReleased(object? sender, Keyboard.Key e)
    {
        if (_axisMap.ContainsKey(e))
        {
            _axisState[_axisMap[e]] = false;
        }
    }

    private void InstanceOnOnButtonPressed(object? sender, Keyboard.Key e)
    {
        if (_axisMap.ContainsKey(e))
        {
            _axisState[_axisMap[e]] = true;
        }
        _horizontalAxis = ( _axisState[InputDirection.Left] ? -1 : 0) + (_axisState[InputDirection.Right] ? 1 : 0);
        _verticalAxis = (_axisState[InputDirection.Up] ? -1 : 0) + (_axisState[InputDirection.Down] ? 1 : 0);
        
        _axis.X = _horizontalAxis;
        _axis.Y = _verticalAxis;
    }
}