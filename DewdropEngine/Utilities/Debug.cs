#region

using DewDrop.Wren;
using System.Runtime.CompilerServices;

#endregion

namespace DewDrop.Utilities;

/// <summary>
/// Provides logging functionality for different levels of log messages.
/// </summary>
public static class Outer {
	/// <summary>
	/// Initializes the Outer class and sets the verbosity level.
	/// </summary>
	internal static void Initialize () {
		SetVerbosity(LogLevel.Engine);
	}

	#region Logging methods

	static LogLevel _Verbosity = LogLevel.Debug;

	// this is for writing the logs to a file in the case of a crash
	static readonly List<string> _AllLogs = new List<string>();

	/// <summary>
	/// Enum representing different levels of log messages.
	/// </summary>
	enum LogLevel {
		System,
		Assert,
		Error,
		Warning,
		Info,
		Debug,
		Lua,
		Engine,
		Trace
	}

	static readonly Dictionary<LogLevel, ConsoleColor> _LogColors = new Dictionary<LogLevel, ConsoleColor> {
		[LogLevel.System] = ConsoleColor.White,
		[LogLevel.Assert] = ConsoleColor.DarkRed,
		[LogLevel.Error] = ConsoleColor.Red,
		[LogLevel.Warning] = ConsoleColor.Yellow,
		[LogLevel.Info] = ConsoleColor.White,
		[LogLevel.Debug] = ConsoleColor.Gray,
		[LogLevel.Lua] = ConsoleColor.Magenta,
		[LogLevel.Engine] = ConsoleColor.Green,
		[LogLevel.Trace] = ConsoleColor.Cyan
	};

	/// <summary>
	/// Logs a message with the System log level.
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void Log (
		object? message,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		LogInternal(LogLevel.System, message, callerFilePath, callerLineNumber);
	}

	/// <summary>
	/// Logs a message with the Engine log level.
	/// </summary>
	/// <param name="message">The message to log.</param>
	internal static void LogEngine (
		object? message,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		LogInternal(LogLevel.Engine, message, callerFilePath, callerLineNumber);
	}


	/// <summary>
	/// Logs an assertion message and throws an exception if the condition is false.
	/// </summary>
	/// <param name="condition">The condition of the assertion.</param>
	/// <param name="message">The message to log if the assertion fails.</param>
	[WrenBlackList]
	public static void LogAssertion (
		bool condition,
		object? message,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		message ??= "Assertion failed.";
		if (!condition) {
			LogInternal(LogLevel.Assert, message, callerFilePath, callerLineNumber);
			throw new ArgumentException("Assertion failed!");
		}

	}

	/// <summary>
	/// Logs an error message and throws the provided exception.
	/// </summary>
	/// <param name="message">The error message to log.</param>
	/// <param name="exception">The exception to throw.</param>
	public static void LogError (
		object? message,
		Exception? exception,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0
	) {
		LogInternal(LogLevel.Error, message, callerFilePath, callerLineNumber);
		if (exception != null) {
			throw exception;
		}
	}

	/// <summary>
	/// Logs a warning message.
	/// </summary>
	/// <param name="message">The warning message to log.</param>
	public static void LogWarning (
		object? message,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		LogInternal(LogLevel.Warning, message, callerFilePath, callerLineNumber);
	}

	/// <summary>
	/// Logs an info message.
	/// </summary>
	/// <param name="message">The info message to log.</param>
	public static void LogInfo (
		object? message,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		LogInternal(LogLevel.Info, message, callerFilePath, callerLineNumber);
	}

	/// <summary>
	/// Logs a Lua message.
	/// </summary>
	/// <param name="message">The Lua message to log.</param>
	public static void LogEsl (
		object? message,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		LogInternal(LogLevel.Lua, message, callerFilePath, callerLineNumber);
	}

	/// <summary>
	/// Logs a debug message.
	/// </summary>
	/// <param name="message">The debug message to log.</param>
	public static void LogDebug (
		object? message,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		LogInternal(LogLevel.Debug, message, callerFilePath, callerLineNumber);
	}

	public static void SLog (
		string message) {
		LogInternal(LogLevel.System, message);
	}

	internal static void SLogEngine (
		string message) {
		LogInternal(LogLevel.Engine, message);
	}

	public static void SLogAssertion (
		bool condition,
		string message = "Assertion failed.") {
		if (!condition) {
			LogInternal(LogLevel.Assert, message);
			//throw new Exception("Assertion failed!");
		}
	}

	public static void SLogError (
		string message) {
		LogInternal(LogLevel.Error, message);
	}

	public static void SLogWarning (
		string message) {
		LogInternal(LogLevel.Warning, message);
	}

	public static void SLogInfo (
		string message) {
		LogInternal(LogLevel.Info, message);
	}

	public static void SLogEsl (
		string message) {
		LogInternal(LogLevel.Lua, message);
	}

	public static void SLogDebug (
		string message) {
		LogInternal(LogLevel.Debug, message);
	}


	/// <summary>
	/// Sets the verbosity level for logging.
	/// </summary>
	/// <param name="level">The level to set the verbosity to.</param>
	static void SetVerbosity (
		LogLevel level,
		[CallerFilePath] string callerFilePath = "",
		[CallerLineNumber] int callerLineNumber = 0) {
		LogInternal(LogLevel.System, $"Current log level is {_Verbosity}, setting it to {level}.", callerFilePath, callerLineNumber);
		_Verbosity = level;
	}

	static void LogInternal (LogLevel logLevel, object? message, string callerFilePath, int callerLineNumber) {
		if (_Verbosity < logLevel) {
			return;
		}

		string callsite = $"{Path.GetFileName(callerFilePath)}:{callerLineNumber}";
		string dateTimeNow = DateTime.UtcNow.ToString("HH:mm:ss");

		Console.ForegroundColor = _LogColors[logLevel];
		Console.WriteLine($"{logLevel}, {dateTimeNow}, {callsite}>>> {message}");
		Console.ResetColor();
		_AllLogs.Add($"{dateTimeNow} [{logLevel}] {callsite}>>> {message}");

	}

	static void LogInternal (LogLevel logLevel, string message) {
		if (_Verbosity < logLevel) {
			return;
		}

		string callsite = $"???:???";
		string dateTimeNow = DateTime.UtcNow.ToString("HH:mm:ss");

		Console.ForegroundColor = _LogColors[logLevel];
		Console.WriteLine($"{logLevel}, {dateTimeNow}, {callsite}>>> {message}");
		Console.ResetColor();
		_AllLogs.Add($"{dateTimeNow} [{logLevel}] {callsite}>>> {message}");

	}
	/// <summary>
	/// Dumps all logged messages to a file.
	/// </summary>
	public static void DumpLogs () {
		StreamWriter streamWriter = new StreamWriter("datadump.log");
		_AllLogs.ForEach(x => streamWriter.WriteLine(x));
		streamWriter.Close();
	}
}

#endregion
