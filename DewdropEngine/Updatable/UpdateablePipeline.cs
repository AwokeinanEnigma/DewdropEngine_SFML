namespace DewDrop.Updatable; 

public class UpdateablePipeline {
	List<IUpdateable> _updateables;
	bool _sort;
	public UpdateablePipeline () {
		_updateables = new List<IUpdateable>();
	}
	public void Add (IUpdateable updateable) {
		_sort = true;
		_updateables.Add(updateable);
	}
	public void AddAll (IEnumerable<IUpdateable> updateables) {
		_sort = true;
		_updateables.AddRange(updateables);
	}
	public void RemoveAll (IEnumerable<IUpdateable> updateables) {
		_updateables.RemoveAll(updateables.Contains);
	}
	public void Remove (IUpdateable updateable) {
		_updateables.Remove(updateable);
	}
	public void Update () {
		if (_sort) {
			_updateables.Sort((x, y) => x.Priority.CompareTo(y.Priority));
			_sort = false;
		}
		foreach (IUpdateable updateable in _updateables) {
			updateable.Update();
		}
	}
}
