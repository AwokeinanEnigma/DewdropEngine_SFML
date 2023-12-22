using DewDrop.Exceptions;
using DewDrop.Utilities;
namespace DewDrop.GameObject; 

public class Transform {
	public const int MaxChildren = 32;
	public readonly Transform[] Children;
	public bool Visible = true;
	public bool DrawRegardlessOfVisibility = false;
	public bool IsBeingDrawn = false;
	public bool IsRoot => Parent == null;
	public Vector2 Origin;
	
	public GameObject GameObject { get; set; }
	public Transform Parent {
		get => _parent; 
		set => SetParent(value);
	}
	public Transform () {
		Children = new Transform[MaxChildren];
		Position = Vector3.Zero;
		Rotation = 0;
		Size = Vector2.Zero;
	}
	public Vector3 Position {
		get => _position; set {
			_lastPosition = _position;
			_position = value; 
			UpdatePosition(); 
		}
	}
	public float Rotation {
		get => _rotation; 
		set { 
			_lastRotation = _rotation;
			_rotation = value; 
			UpdateRotation(); 
		}
		
	}
	public Vector2 Size { get; set; }
	
	Vector3 _position;
	Vector3 _lastPosition;
	float _lastRotation;
	float _rotation;
	Transform _parent;
	int _availableIndex;
	
	#region Position and Rotation

	void UpdatePosition () {
		// we want to position our children relative to our position. so if our position is 1, and their position is 1, their actual position is 2
		for (int i = 0; i < MaxChildren; i++) {
			Transform child = Children[i];
			if (child != null) {
				// only move by the difference between our last position and our current position
				child.Position += (_position - _lastPosition);
			}
		}
	}
	void UpdateRotation () {
		// we want to rotate our children relative to our rotation. so if our rotation is 1, and their rotation is 1, their actual rotation is 2
		for (int i = 0; i < MaxChildren; i++) {
			Transform child = Children[i];
			if (child != null) {
				// only move by the difference between our last position and our current position
				child.Rotation += (_rotation - _lastRotation);
			}
		}
	}
	
	#endregion
	#region Child Management

	public void AttachChild (Transform gameObject) {
		if (_availableIndex >= MaxChildren) {
			throw new TooManyChildrenException($"Too many children on object '{GameObject.Name}'");
		}

		Children[_availableIndex] = gameObject;
		_availableIndex++;
	}
	public void DetachChild (Transform gameObject) {
		for (int i = 0; i < MaxChildren; i++) {
			if (Children[i] == gameObject) {
				_availableIndex--;
				Children[i] = null;
				return;
			}
		} 
	}

	#endregion
	
	public void Destroy () {
		if (Parent != null) {
			Parent.DetachChild(this);
			_parent = null;
		}
		
		for (int i = 0; i < MaxChildren; i++) {
			Transform child = Children[i];
			if (child != null) {
				child.GameObject.Destroy();
			}
		}
	}

	public void Clone (Transform transform) {
		transform.Position = Position;
		transform.Rotation = Rotation;
		transform.Size = Size;
	}
	public void SetParent (Transform gameObject) {
		if (Parent != null) {
			_parent.DetachChild(gameObject);
			_parent = null;
		}
		if (this == gameObject) {
			Outer.LogError("You cannot set parent a GameObject to itself!", null);
			return;
		}
		_parent = gameObject ?? throw new ArgumentNullException(gameObject.ToString());
		_parent.AttachChild(this); 
	}
}
