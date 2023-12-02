#region

using DewDrop.Utilities;
using SFML.Window;

#endregion
namespace DewDrop.UserInput;

/// <summary>
///     Defines a class that manages the axis input of the player.
/// </summary>
public class AxisManager {
	public static AxisManager Instance { get; private set; }

    /// <summary>
    ///     Contains various directions.
    /// </summary>
    public enum InputDirection {
		Up,
		Down,
		Left,
		Right
	}

	// contains the mapping of keys to directions
	readonly Dictionary<Keyboard.Key, InputDirection> _axisMap;
	// contains the state of each direction, if it's pressed or not
	readonly Dictionary<InputDirection, bool> _axisState;

	int _horizontalAxis;
	int _verticalAxis;

    /// <summary>
    ///     The axis the player is trying to move into.
    /// </summary>
    public Vector2 Axis => _axis;

	Vector2 _axis;

    /// <summary>
    ///     This event is invoked before the axis is updated with the previous axis value.
    /// </summary>
    public static event Action<Vector2> PreAxisChanged;

    /// <summary>
    ///     This event is invoked after the axis is updated with the new axis value.
    /// </summary>
    public static event Action PostAxisChanged;


    /// <summary>
    ///     Creates a new AxisManager with the given axis map.
    /// </summary>
    /// <param name="axisMap">A dictionary containing keys that correspond to a specific direction.</param>
    /// <exception cref="ArgumentNullException">Thrown if you ( for any reason ) pass a null axisMap</exception>
    public AxisManager (Dictionary<Keyboard.Key, InputDirection> axisMap) {
		Instance = this;

		_axisMap = axisMap ?? throw new ArgumentNullException(nameof(axisMap));
		_axisState = new Dictionary<InputDirection, bool>();

		// map all directions to dict
		foreach (InputDirection direction in Enum.GetValues(typeof(InputDirection))) {
			if (!_axisState.ContainsKey(direction))
				_axisState.Add(direction, false);
		}

		// create axis
		_axis = new Vector2(0, 0);

		// hook into input events
		Input.OnKeyPressed += InstanceOnOnButtonPressed;
		Input.OnKeyReleased += InstanceOnOnButtonReleased;
	}

    /// <summary>
    ///     Creates a new AxisManager with the default axis map.
    /// </summary>
    public AxisManager () {
		Instance = this;

		_axisMap = new Dictionary<Keyboard.Key, InputDirection>();

		_axisMap.Add(Keyboard.Key.W, InputDirection.Up);
		_axisMap.Add(Keyboard.Key.S, InputDirection.Down);
		_axisMap.Add(Keyboard.Key.A, InputDirection.Left);
		_axisMap.Add(Keyboard.Key.D, InputDirection.Right);

		_axisMap.Add(Keyboard.Key.Up, InputDirection.Up);
		_axisMap.Add(Keyboard.Key.Down, InputDirection.Down);
		_axisMap.Add(Keyboard.Key.Left, InputDirection.Left);
		_axisMap.Add(Keyboard.Key.Right, InputDirection.Right);

		_axisState = new Dictionary<InputDirection, bool>();

		foreach (InputDirection direction in Enum.GetValues(typeof(InputDirection))) {
			if (!_axisState.ContainsKey(direction))
				_axisState.Add(direction, false);
		}

		_axis = new Vector2(0, 0);

		Input.OnKeyPressed += InstanceOnOnButtonPressed;
		Input.OnKeyReleased += InstanceOnOnButtonReleased;
	}

	void InstanceOnOnButtonReleased (object? sender, Keyboard.Key e) {
		if (_axisMap.ContainsKey(e)) {
			_axisState[_axisMap[e]] = false;
		}

		RemapAxis();
	}

	void InstanceOnOnButtonPressed (object? sender, Keyboard.Key e) {
		if (_axisMap.ContainsKey(e)) {
			_axisState[_axisMap[e]] = true;
		}

		RemapAxis();
	}

	// update our axis
	void RemapAxis () {
		PreAxisChanged?.Invoke(_axis);

		_horizontalAxis = (_axisState[InputDirection.Left] ? -1 : 0) + (_axisState[InputDirection.Right] ? 1 : 0);
		_verticalAxis = (_axisState[InputDirection.Up] ? -1 : 0) + (_axisState[InputDirection.Down] ? 1 : 0);

		_axis.X = _horizontalAxis;
		_axis.Y = _verticalAxis;

		PostAxisChanged?.Invoke();
	}
}
