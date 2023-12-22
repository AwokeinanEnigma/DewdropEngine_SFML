using DewDrop.Exceptions;
using DewDrop.Utilities;
namespace DewDrop.Internal; 

public class Transform {
	public const int MaxChildren = 5012;
	public Transform[] Children;
	public bool Visible = true;
	public bool DrawRegardlessOfVisibility = false;
	public bool IsBeingDrawn = false;
	public int ChildCount => _availableIndex;
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
			if ((int)_lastPosition.Z != (int)_position.Z) {
				GameObjectRegister.ForceSort();
			}
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
	bool _destroyed;
	#region Position and Rotation

	void UpdatePosition () {
		if (_destroyed)
			return;
		
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
		if (_destroyed)
			return;
		
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
	public void SetActive (bool active) {
	foreach (Transform child in Children) {
		if (child != null) {
			child.GameObject.Active = active;
		}
	}
}
	public void AttachChild (Transform gameObject) {
		if (_destroyed)
			return;
		
		if (_availableIndex >= MaxChildren) {
			throw new TooManyChildrenException($"Too many children on object '{GameObject.Name}'");
		}
		
		gameObject.GameObject.UpdateSlot = GameObject.UpdateSlot + 1;
		Children[_availableIndex] = gameObject;
		_availableIndex++;
	}
	public void DetachChild (Transform gameObject) {
		if (_destroyed)
			return;
		
		for (int i = 0; i < MaxChildren; i++) {
			if (Children[i] == gameObject) {
				_availableIndex--;
				Children[i] = null;
				return;
			}
		} 
	}

	#endregion
	
	public void Destroy (bool sceneWipe) {
		_destroyed = true;
		if (Parent != null) {
			Parent.DetachChild(this);
			_parent = null;
		}
		
		for (int i = 0; i < MaxChildren; i++) {
			Transform child = Children[i];
			if (child != null) {
				child.GameObject.Destroy(sceneWipe);
				Children[i] = null;
			}
		}
		Children = null;
		Position = Vector3.Zero;
		Rotation = 0;
		Size = Vector2.Zero;
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
