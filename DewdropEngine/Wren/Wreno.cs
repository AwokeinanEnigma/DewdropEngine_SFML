using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using System.Text;
using System.Text.RegularExpressions;
namespace DewDrop.Wren;

/// <summary>
/// A quick and easy WrenVM wrapper.
/// Tip: Don't initalize these in your Scene's constructor, use TransitionIn instead!
/// </summary>
public class Wreno : IDisposable {
	readonly WrenVM _wren;
	readonly Dictionary<string, WrenFunctionHandle> _handles;
	readonly Dictionary<Type, Action<int, object>> _typeMap;
	WrenEventHandler[] _eventHandlers;
	bool _disposed;
	string _script;
	static Dictionary<string, Type> _EventHandlers = new Dictionary<string, Type>() {
		["e_OnButtonPressed"] = typeof(WrenOnButtonPressedHandler),
		["e_OnButtonReleased"] = typeof(WrenOnButtonReleasedHandler),
		["e_OnKeyReleased"] = typeof(WrenOnKeyReleasedHandler),
		["e_OnKeyPressed"] = typeof(WrenOnKeyPressedHandler)
	};

	/// <summary>
	/// Initializes a new instance of the Wreno class.
	/// </summary>
	/// <param name="config">The WrenConfig to use for initialization.</param>
	/// <param name="script">The Wren script to run.</param>
	public Wreno (WrenConfig config, string script) {
		_wren = new WrenVM(config);
		//_wren.BindForeignClass += BindForeignClass;
		_handles = new Dictionary<string, WrenFunctionHandle>();
		_wren.Write +=  Write;
		_wren.Error += WriteError;
		_typeMap = new Dictionary<Type, Action<int, object>>
		{	
			[typeof(float)] = (i, v) => { _wren.SetSlotDouble(i , (float)v); },
			[typeof(int)] = (i, v) => { _wren.SetSlotDouble(i , (int)v); },
			[typeof(string)] = (i, v) => _wren.SetSlotString(i , (string)v),
			[typeof(bool)] = (i, v) => _wren.SetSlotBool(i , (bool)v),
			[typeof(double)] = (i, v) => _wren.SetSlotDouble(i , (double)v),
			[typeof(byte[])] = (i, v) => _wren.SetSlotBytes(i , (byte[])v)
		};
		_script = script;
		GenerateEventHandlers ();
	}
	
	void GenerateEventHandlers () {
		Regex gex = new Regex(@"e_(\w+)");
		MatchCollection matches = gex.Matches(_script);
		_eventHandlers = new WrenEventHandler[matches.Count];
		for (int i = 0; i < matches.Count; i++) {
			Match m = matches[i];
			if (m.Success) {
				string eventName = m.Groups[i].Value;
				Outer.Log($"Found event handler {eventName}");
				if (_EventHandlers.TryGetValue(eventName, out var type)) {
					_eventHandlers[i] = (WrenEventHandler)Activator.CreateInstance(type, this);
				}
			}
		}
	}
	
	#region Variable

	public T GetVariable<T> (string variable, string module = WrenVM.MainModule) {
		_wren.EnsureSlots(1);
		_wren.GetVariable(module, variable, 0);
		return _wren.GetSlotForeign<T>(0);
		
	}
	
	public void SetVariable<T>  (T value, string module = WrenVM.MainModule) {
		_wren.EnsureSlots(1);
		_wren.SetSlotNewForeign(0, value);
	}
	
	
	#endregion
	
	#region Function Calls
	// So the thing with handles is that they're costly. Making one is a direct call to the Wren API, and that's a direct call to C land.
	// Generally, in programming, you don't want to make more than you need to. Here we have a dictionary of handles, and if we need to make a new one, we'll make a new one.
	// If we don't, we'll just use the one we already have.
	WrenFunctionHandle GetFunction (string function, string call = "call(_)") {
		WrenFunctionHandle handle;
		if (!_handles.ContainsKey(function)) {
			handle = _wren.MakeCallHandle(call);
			_handles.Add(function, handle);
		}
		else {
			handle = _handles[function];
		}

		return handle;
	}
	
	/// <summary>
	/// Calls a Wren function with a given value.
	/// </summary>
	/// <param name="function">The name of the function to call.</param>
	/// <param name="value">The value to pass to the function.</param>
	public void CallFunction (string function, string value) {
		WrenFunctionHandle handle = GetFunction(function);
		_wren.EnsureSlots(2);
		_wren.GetVariable(WrenVM.MainModule, function, 0);
		_wren.SetSlotString(1, value);
		_wren.Call(handle);
	}

	/// <summary>
	/// Calls a Wren function with no parameters.
	/// </summary>
	/// <param name="function">The name of the function to call.</param>
	public void CallFunction (string function) {
		WrenFunctionHandle handle =  GetFunction(function);
		_wren.EnsureSlots(1);
		_wren.GetVariable(WrenVM.MainModule, function, 0);
		_wren.Call(handle);
		
	}

	/// <summary>
	/// Calls a Wren function with a given value.
	/// </summary>
	/// <param name="function">The name of the function to call.</param>
	/// <param name="value">The value to pass to the function.</param>
	public void CallFunction (string function, float value) {
		WrenFunctionHandle handle = GetFunction(function);
		_wren.EnsureSlots(2);
		_wren.GetVariable(WrenVM.MainModule, function, 0);
		_wren.SetSlotDouble(1, value);
		_wren.Call(handle);
		
	}
	/// <summary>
	/// Calls a Wren function with an array of values.
	/// </summary>
	/// <param name="function">The name of the function to call.</param>
	/// <param name="values">The values to pass to the function.</param>
	public void CallFunction (string function, object[] values) 
	{
		StringBuilder callBuilder = new StringBuilder("call(");
		for (int i = 0; i < values.Length; i++) {
			callBuilder.Append("_,");
		}
		callBuilder.Length -= 1; // Remove the last comma
		callBuilder.Append(")");
		string call = callBuilder.ToString();
		
		var fnHandle = GetFunction(function, call);
		// +1 for the variable call
		_wren.EnsureSlots(values.Length +1);
		_wren.GetVariable(WrenVM.MainModule, function, 0);
		for (int i = 0; i < values.Length; i++)
		{
			var type = values[i].GetType();
			if (_typeMap.TryGetValue(type, out var action))
			{
				action(i + 1,  values[i]);
			}
			else
			{
				Outer.Log($"Unknown type {type}");
			}
		}

		_wren.Call(fnHandle);
	}

	#endregion

	#region Logging

	void Write (WrenVM vm, string text) {
		if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(text)) 
			Outer.LogESL(text);
	}
	void WriteError(WrenVM vm, WrenErrorType type, string module, int line, string message) {
		Outer.LogError($"Wren Error: {message} at {module}:{line}", new Exception(message));
	}

	#endregion

	#region AutoMap

	/// <summary>
	/// Maps a type to this Wreno.
	/// </summary>
	/// <typeparam name="T">The type to map to this Wreno.</typeparam>
	public void AutoMap<T> () {
		_wren.AutoMap<T>();
	}
	/// <summary>
	/// Maps an array of types to this Wreno.
	/// </summary>
	/// <param name="types">The types to map.</param>
	public void Automap (Type[] types) {
		_wren.AutoMap(types);
	}

	#endregion

	#region Run
	/// <summary>
	/// Sets the Wren script to run on this Wreno.
	/// </summary>
	/// <param name="script">The Wren script to run.</param>
	public void SetScript (string script) {
		_script = script;
		GenerateEventHandlers();
	}
	
	/// <summary>
	/// Executes the script the Wreno is using. 
	/// </summary>
	/// <returns>
	/// Returns true if the script runs successfully, false otherwise.
	/// </returns>
	public bool Run () {
		if (_wren.Interpret(_script) == WrenInterpretResult.Success) {
			return true;
		}
		return false;
	}
	
	public void DoThings () {
	}
	#endregion
	
	#region Dispose
	// called by the system to clean up, meaning we can only get rid of unmanaged stuff
	~Wreno () {
		// free only unmanaged resources
		Dispose(false);
	}

	/// <summary>
	///     Here, you must dispose of unmanaged and managed resources.
	/// </summary>
	/// <param name="disposing">
	///     If true, then we can get rid of managed and unmanaged resources. If false, we can only get rid
	///     of unmanaged resources.
	/// </param>
	protected virtual void Dispose (bool disposing) {
		if (!_disposed && disposing) {
			_wren.Write -=  Write;
			_wren.Error -= WriteError;
			// if you do this after wren has been disposed, it'll throw  System.ObjectDisposedException: Safe handle has been closed.
			foreach (var handle in _handles.Values) {
				handle.Close();
				handle.Dispose();
			}
			
			for (int i = 0; i < _eventHandlers.Length; i++) {
				_eventHandlers[i].Dispose();
			}
			
			_wren.CollectGarbage();
			_wren.Close();
			_wren.Dispose();
		}
		_disposed = true;
	}

	/// <summary>
	///   Disposes of all managed and unmanaged resources.
	/// </summary>
	public void Dispose () {
		//
		Dispose(true);

		// we manually disposed, we don't need to finalize
		GC.SuppressFinalize(this);
	}


	#endregion
}
