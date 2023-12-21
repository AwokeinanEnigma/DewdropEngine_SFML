using DewDrop.Exceptions;
using System.Collections;
namespace DewDrop.GameObject; 

public class ComponentHolder : IEnumerable<Component> {
	readonly GameObject _gameObject;
	const int MaxComponents = 32;
	Component[] Components { get; set; }
	int _availableIndex;
	
	public ComponentHolder (GameObject gameObject)
	{
		_gameObject = gameObject;
		Components = new Component[MaxComponents];
	}
	
	public void AddComponent (Component component) {
		if (_availableIndex >= MaxComponents) {
			throw new TooManyComponentsException($"Too many components on object '{component.GameObject.Name}'");
		}
		Components[_availableIndex] = component;
		// sort components by importance
		SortComponents();
		_availableIndex++;
	}
	
	void SortComponents () {
		for (int i = 0; i < _availableIndex; i++) {
			for (int j = i; j > 0; j--) {
				if (Components[j].Importance > Components[j - 1].Importance) {
					(Components[j], Components[j - 1]) = (Components[j - 1], Components[j]);
				}
			}
		}
	}
	
	public void RemoveComponent (Component component) {
		for (int i = 0; i < _availableIndex; i++) {
			if (Components[i] == component) {
				Components[i] = null;
				return;
			}
		}
		throw new ComponentNotFoundException("Component not found");
	}
	
	public T GetComponent<T> () where T : Component {
		for (int i = 0; i < _availableIndex; i++) {
			if (Components[i] is T) {
				return (T)Components[i];
			}
		}
		throw new ComponentNotFoundException("Component not found");
	}
	
	public T AddComponent<T> () where T : Component, new() {
		T component = new T();
		component.GameObject = _gameObject;
		AddComponent(component);
		return component;
	}
	
	public void Awake () {
		for (int i = 0; i < _availableIndex; i++) {
			Components[i].Awake();
		}
	}
	
	public void Start () {
		for (int i = 0; i < _availableIndex; i++) {
			Components[i].Start();
		}
	}
	
	public void Update () {
		for (int i = 0; i < _availableIndex; i++) {
			Components[i].Update();
		}
	}
	
	public void Draw () {
		for (int i = 0; i < _availableIndex; i++) {
			Components[i].Draw();
		}
	}
	
	public void Destroy () {
		for (int i = 0; i < _availableIndex; i++) {
			Components[i].Destroy();
		}
	}
	public IEnumerator<Component> GetEnumerator () {
		return (IEnumerator<Component>)Components.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator(); }
}
