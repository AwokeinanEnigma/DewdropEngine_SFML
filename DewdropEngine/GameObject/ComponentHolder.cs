using DewDrop.Exceptions;
using SFML.Graphics;
using System.Collections;
namespace DewDrop.GameObject; 

public class ComponentHolder : IEnumerable<Component> {
	GameObject _gameObject;
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
		component.GameObject = _gameObject;
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
	
	public T? GetComponent<T> () where T : Component {
		for (int i = 0; i < _availableIndex; i++) {
			if (Components[i] is T) {
				return (T)Components[i];
			}
		}
		return null;
		//throw new ComponentNotFoundException("Component not found");
	}
	
	public T AddComponent<T> () where T : Component, new() {
		T component = new T();
		component.GameObject = _gameObject;
		AddComponent(component);
		return component;
	}
	
	public void RemoveComponent<T> () where T : Component {
		for (int i = 0; i < _availableIndex; i++) {
			if (Components[i] is T) {
				Components[i].Destroy();
				Components[i] = null;
				_availableIndex--;
				return;
			}
		}
		//throw new ComponentNotFoundException("Component not found");
	}
	
	public T AddOrGetComponent<T> () where T : Component, new() {
		T component = GetComponent<T>();
		if (component == null) {
			component = AddComponent<T>();
		}
		return component;
	}
	
	
	public void Clone (ComponentHolder componentHolder) {
		for (int i = 0; i < _availableIndex; i++) {
			componentHolder.AddComponent((Component)Activator.CreateInstance(Components[i].GetType()));
		}
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
			if (Components[i].Active) 
				Components[i].Update();
		}
	}
	
	public void Draw (RenderTarget target) {
		for (int i = 0; i < _availableIndex; i++) {
			if (Components[i].Active) 
				Components[i].Draw(target);
		}
	}
	
	public void Destroy (bool sceneWipe) {
		for (int i = 0; i < _availableIndex; i++) {
			Components[i].Destroy();
			Components[i] = null;
		}
		_availableIndex = 0;
		Components = null;
		_gameObject = null;
	}
	public IEnumerator<Component> GetEnumerator () {
		return (IEnumerator<Component>)Components.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator(); }
}
