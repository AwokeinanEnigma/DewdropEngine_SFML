#region

using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace DewDrop.GUI;

/// <summary>
/// Represents a scrollable list of selectable items.
/// </summary>
public class ScrollableList : Renderable {
	
	/// <summary>
	/// Gets or sets the currently selected item in the list.
	/// </summary>
	public string SelectedItem { get; set; }

	/// <summary>
	/// Gets the total number of items in the list.
	/// </summary>
	public int ItemCount => _options.Length;

	/// <summary>
	/// Controls whether the ScrollableList is visible. 
	/// </summary>
	public override bool Visible {
		get => _visible;
		set {
			UpdateVisibility(value);
			_visible = value;
		}
	}
	
	int _selectedIndex;
	int _topIndex;

	readonly TextRenderer[] _texts;
	readonly string[] _options;
	readonly int _displayCount;
	readonly SelectAction[] _actions;
	readonly Clock _clock;
	readonly bool _selfControlled;
	bool _pressed;
	
	
	public struct SelectAction {
		public delegate bool SelectActionDelegate (ScrollableList list);
		public SelectActionDelegate Select;
		public string Text;
	}

	/// <summary>
	/// Initializes a new instance of the ScrollableList class with specified position, depth, options, displayable options, line height, font data, and self-controlled flag.
	/// </summary>
	/// <param name="position">The position of the ScrollableList.</param>
	/// <param name="depthToRenderAt">The depth at which to render the ScrollableList.</param>
	/// <param name="options">The options for the ScrollableList.</param>
	/// <param name="displayableOptions">The number of displayable options in the ScrollableList.</param>
	/// <param name="lineHeight">The height of each line in the ScrollableList.</param>
	/// <param name="data">The font data for the ScrollableList.</param>
	/// <param name="selfControlled">Indicates whether the ScrollableList is self-controlled.</param>
	public ScrollableList (Vector2 position, int depthToRenderAt, SelectAction[] options, int displayableOptions, float lineHeight, FontData data, bool selfControlled = true) {
		_clock = new Clock();
		_selfControlled = selfControlled;
		_options = new string[options.Length];
		for (int i = 0; i < options.Length; i++) {
			_options[i] = options[i].Text;
		}
		_actions = options;

		_position = position;
		_origin = Vector2.Zero;
		_depth = depthToRenderAt;
		_size = new Vector2(320, 180);
		_displayCount = displayableOptions;
		_visible = true;

		int renderers = Math.Min(_options.Length, _displayCount);
		_texts = new TextRenderer[renderers];
		for (int i = 0; i < renderers; i++) {
			_texts[i] = new TextRenderer(new Vector2(position.X, position.Y + lineHeight*i), depthToRenderAt, data, _options[i]);
			_texts[i].Color = Color.White;
		}

		if (_selfControlled)
		{
			Input.OnButtonPressed += OnButtonPressed;
		}
		UpdateIndicators();
		UpdateVisibility(true);
	}
	
	
	public override void Draw (RenderTarget target) {
		// if we're self controlled, we need to check for input
		// this is done as draw because its updated every frame
		if (_selfControlled) {
			if (Input.Instance.Axis.Y > 0.5f && _clock.ElapsedTime.AsSeconds() > 0.2f) {
				SelectNext();
				_clock.Restart();
			} else if (Input.Instance.Axis.Y < -0.5f && _clock.ElapsedTime.AsSeconds() > 0.2f) {
				SelectPrevious();
				_clock.Restart();
			}
		}
		for (int i = 0; i < _texts.Length; i++) {
			if (_texts[i].Visible)
				_texts[i].Draw(target);
		}
	}

	/// <summary>
	/// Changes the text at the specified index.
	/// </summary>
	/// <param name="index">The index of the text to update.</param>
	/// <param name="newText">The new text to display.</param>
	public void ChangeText (int index, string newText) {
		if (index < 0 || index >= _texts.Length) {
			Outer.LogError("Index out of range!", new IndexOutOfRangeException("You can't update a text that doesn't exist!"));
			return;
		}
		_texts[index].Text = newText;
	}
	
	/// <summary>
	/// Updates the visibility of the ScrollableList.
	/// </summary>
	/// <param name="visible">Indicates whether the ScrollableList is visible.</param>
	void UpdateVisibility (bool visible) {
		for (int i = 0; i < _texts.Length; i++) {
			_texts[i].Visible = visible;
		}
	}

	/// <summary>
	/// Hides the ScrollableList.
	/// </summary>
	public void Hide () {
		UpdateVisibility(false);
	}

	/// <summary>
	/// Shows the ScrollableList.
	/// </summary>
	public void Show () {
		UpdateVisibility(true);
	}
	
	void OnButtonPressed (object? sender, DButtons button) {
		if (button != DButtons.Select || _pressed) return;
		 
		bool? boolean = _actions[_selectedIndex].Select?.Invoke(this); //true;
		if (boolean != null && boolean.Value) {
			_pressed = boolean.Value;
		}
	}

	/// <summary>
	/// Selects an item in the ScrollableList by index.
	/// </summary>
	public void SelectItem (int index) {
		if (index < 0 || index >= _texts.Length) {
			Outer.LogError("Index out of range!", new IndexOutOfRangeException("You can't select a text that doesn't exist!"));
			return;
		}

		// make sure we don't go out of bounds with the min
		// and make sure we don't go below 0 with the max
		_selectedIndex = Math.Min(_options.Length - 1, Math.Max(0, index));
	}

	/// <summary>
	/// Updates the displayed texts in the ScrollableList.
	/// </summary>
	public void UpdateDisplayTexts () {
		for (int i = 0; i < _displayCount; i++) {
			int num = _topIndex + i;
			if (num < _options.Length) {
				_texts[i].Text = _options[num];
			} else if (i < _texts.Length) {
				_texts[i].Text = string.Empty;
			}
		}
	}

	/// <summary>
	/// Updates the indicators in the ScrollableList.
	/// </summary>
	public void UpdateIndicators () {
		foreach (var text in _texts) {
			text.Color = Color.White;
		}
		_texts[_selectedIndex - _topIndex].Color = new Color(255, 89, 209);
	}

	/// <summary>
	/// Selects the previous item in the ScrollableList.
	/// </summary>
	/// <returns>True if the previous item was successfully selected, false otherwise.</returns>
	public bool SelectPrevious () {
		if (_selectedIndex - 1 < 0) return false;
		_selectedIndex--;
		if (_selectedIndex < _topIndex) {
			_topIndex--;
			UpdateDisplayTexts();
		}
		UpdateIndicators();
		return true;
	}
	
	/// <summary>
	/// Selects the next item in the ScrollableList.
	/// </summary>
	/// <returns>True if the next item was successfully selected, false otherwise.</returns>
	public bool SelectNext () {
		if (_selectedIndex + 1 >= _options.Length) return false;
		_selectedIndex++;
		if (_selectedIndex > _topIndex + _displayCount - 1) {
			_topIndex++;
			UpdateDisplayTexts();
		}
		UpdateIndicators();
		return true;
	}
	
	/// <summary>
	/// Disposes of the ScrollableList and its resources.
	/// </summary>
	/// <param name="disposing">Indicates whether the ScrollableList is currently being disposed.</param>
	protected override void Dispose (bool disposing) {
		base.Dispose(disposing);
		if (!_disposed && disposing) {
			// dispose of each text
			for (int i = 0; i < _texts.Length; i++) {
				_texts[i].Dispose();
			}
		}
		Input.OnButtonPressed -= OnButtonPressed;
	}
}
