using System.Collections;

	public class ClearPaintListCommand : ICommand
	{
		private IList _list;
		private List<object> _previousState;

		public ClearPaintListCommand(IList list)
		{
			_list = list;
			_previousState = new List<object>(_list.Cast<object>());
		}

		public void Execute()
		{
			_list.Clear();
		}

		public void Undo()
		{
			foreach (var item in _previousState)
			{
				_list.Add(item);
			}
		}

		public void Redo()
		{
			Execute();
		}
	
}
