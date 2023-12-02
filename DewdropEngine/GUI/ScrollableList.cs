#region

using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

#endregion

namespace DewDrop.GUI;

public class ScrollableList : Renderable {
	public string SelectedItem {
		get => _options[_selectedIndex];
		set => _options[_selectedIndex] = value;
	}

	public override bool Visible {
		get => _visible;
		set {
			UpdateVisibility(value);
			_visible = value;
		}
	}

	public int ItemCount => _options.Length;

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
		public SelectActionDelegate OnSelect;
		public string Text;
	}

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
	
	public void RefreshPressed()
	{
		_pressed = false;
	}
	public override void Draw (RenderTarget target) {
		// if we're self controlled, we need to check for input
		// this is done as draw because its updated every frame
		if (_selfControlled) {
			if (AxisManager.Instance.Axis.Y > 0.5f && _clock.ElapsedTime.AsSeconds() > 0.2f) {
				SelectNext();
				_clock.Restart();
			} else if (AxisManager.Instance.Axis.Y < -0.5f && _clock.ElapsedTime.AsSeconds() > 0.2f) {
				SelectPrevious();
				_clock.Restart();
			}
		}
		for (int i = 0; i < _texts.Length; i++) {
			if (_texts[i].Visible)
				_texts[i].Draw(target);
		}
	}

	public void OnButtonPressed (object? sender, DButtons button) {
		if (button == DButtons.Select && !_pressed) {
			bool? boolean = _actions[_selectedIndex].OnSelect?.Invoke(this); //true;
			if (boolean != null && boolean.Value) {
				_pressed = boolean.Value;
			}
		}
	}
	public void UpdateText (int index, string newText) {
		if (index < 0 || index >= _texts.Length) {
			Outer.LogError("Index out of range!", new IndexOutOfRangeException("You can't update a text that doesn't exist!"));
			return;
		}
		_texts[index].Text = newText;
	}

	public void UpdateVisibility (bool visible) {
		for (int i = 0; i < _texts.Length; i++) {
			_texts[i].Visible = visible;
		}
	}

	public void Hide () {
		UpdateVisibility(false);
	}

	public void Show () {
		UpdateVisibility(true);
	}

	public void SelectItem (int index) {
		if (index < 0 || index >= _texts.Length) {
			Outer.LogError("Index out of range!", new IndexOutOfRangeException("You can't select a text that doesn't exist!"));
			return;
		}

		// make sure we don't go out of bounds with the min
		// and make sure we don't go below 0 with the max
		_selectedIndex = Math.Min(_options.Length - 1, Math.Max(0, index));
	}

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

	public void UpdateIndicators () {
		foreach (var text in _texts) {
			text.Color = Color.White;
		}
		_texts[_selectedIndex - _topIndex].Color = new Color(255, 89, 209);
	}

	public bool SelectPrevious () {
		if (_selectedIndex - 1 >= 0) {
			_selectedIndex--;
			if (_selectedIndex < _topIndex) {
				_topIndex--;
				UpdateDisplayTexts();
			}
			UpdateIndicators();
			return true;
		}
		return false;
	}
	public bool SelectNext () {
		if (_selectedIndex + 1 < _options.Length) {
			_selectedIndex++;
			if (_selectedIndex > _topIndex + _displayCount - 1) {
				_topIndex++;
				UpdateDisplayTexts();
			}
			UpdateIndicators();
			return true;
		}
		return false;
	}

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
