namespace GameLib.UI {
	/// <summary>DialogWindow でアイテムが選択されたときのイベントの Delegate。　void OkEventHandler(DialogWindow sender, DialogWindowOKEventArgs e)</summary>
	public delegate void OkEventHandler(object sender, EventArgs.DialogWindowOKEventArgs e);
	/// <summary>DialogWindow でアイテムが選択されなかったときのイベントの Delegate。　void NgEventHandler(DialogWindow sender, DialogWindowNGEventArgs e)</summary>
	public delegate void NgEventHandler(object sender, EventArgs.DialogWindowNGEventArgs e);
	/// <summary>DialogWindowItem のフォーカスが変わった時のイベント Delegate。　void ToggleSelectedEventHandler(DialogWindowItem sender, EventArgs e)</summary>
	public delegate void ToggleSelectedEventHandler(object sender, System.EventArgs e);
}
